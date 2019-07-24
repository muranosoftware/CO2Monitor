﻿using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using CO2Monitor.Core.Shared;
using CO2Monitor.Core.Entities;
using CO2Monitor.Core.Interfaces.Services;
using CO2Monitor.Core.Interfaces.Devices;
using CO2Monitor.Core.Interfaces.Notifications;
using CO2Monitor.Infrastructure.Helpers;

namespace CO2Monitor.Infrastructure.Services {
	public class DeviceTextCommandService : IDeviceTextCommandService {
		private const string CommandButtonsFile = "Buttons.json";

		private static readonly string ActionPattern = @"^\s*\w+\s+\w+\s*\.\s*\w+\s*\(\s*\w*\s*\)\s*$"; // action DeviceName.Action(argument)

		private readonly Dictionary<string, (string description, Func<TextCommand, Task<string>> command)> _predefinedCommands;

		private readonly Dictionary<string, string> _commandButtons;

		private readonly ILogger<DeviceTextCommandService> _logger;
		private readonly IDeviceManagerService _deviceManagerService;
		private readonly ITextCommandProvider _commandProvider;
		private readonly IPlotService _plotService;
		private readonly IDeviceStateRepository _stateRepository;
		private readonly IEventNotificationService _notificationService;

		public DeviceTextCommandService(ILogger<DeviceTextCommandService> logger, 
		                                IDeviceManagerService deviceManagerService, 
		                                ITextCommandProvider commandProvider, 
		                                IPlotService plotService, 
		                                IDeviceStateRepository stateRepository, 
		                                IEventNotificationService notificationService) {
			_logger = logger;
			_deviceManagerService = deviceManagerService;
			_plotService = plotService;
			_commandProvider = commandProvider;
			_stateRepository = stateRepository;
			_notificationService = notificationService;

			InitCommandsAndAlieses(out _predefinedCommands, out _commandButtons);

			_commandProvider.NewCommand += OnNewCommand;
		}

		private async void OnNewCommand(string userId, string userName, string command, string channelId, bool isGroupChannel) {
			var cmd = new TextCommand { UserId = userId, UserName = userName, Command = command, ChannelId = channelId, IsGroupChannel = isGroupChannel };

			string response = await ExecuteCommand(cmd, false);
			if (!string.IsNullOrWhiteSpace(response))
				await _commandProvider.SendTextMessage(cmd.ChannelId, response, GetDefaultAttachments());
		}

		object[] GetDefaultAttachments() {
			if (_commandButtons.Count == 0)
				return null;

			return new object[] {
				new {
					text = "User defined commands",
					callback_id = "wopr_game",
					attachment_type = "default",
					actions = _commandButtons.Select(x => new { name = "action", text = x.Key, type = "button", value = x.Value }).ToArray()
				}
			};
		}

		async Task<TextCommandResult> IDeviceTextCommandService.ExecuteCommand(TextCommand command) {
			string result = await ExecuteCommand(command, false);
			return new TextCommandResult { Text = result, Attachments = GetDefaultAttachments() };
		}

		private async Task<string> ExecuteCommand(TextCommand cmd, bool ignoreAlies) {
			if (string.IsNullOrWhiteSpace(cmd.Command))
				return null;

			var parser = new CommandLineParser(cmd.Command);
			
			if (_predefinedCommands.ContainsKey(parser.Commnad.ToLower()))
				return await _predefinedCommands[parser.Commnad.ToLower()].command(cmd);

			if (!ignoreAlies && _commandButtons.ContainsKey(parser.Commnad))
				return await ExecuteCommand(new TextCommand {
					Command = _commandButtons[parser.Commnad],
					UserId = cmd.UserId,
					UserName = cmd.UserName,
					ChannelId = cmd.ChannelId,
					IsGroupChannel = cmd.IsGroupChannel
				}, true);

			return $"Unknown command [{cmd.Command}]. Try 'help' for help.";
		}

		private void InitCommandsAndAlieses(out Dictionary<string, (string, Func<TextCommand, Task<string>>)> predefinedCommands, out Dictionary<string, string> buttons) {
			predefinedCommands = new Dictionary<string, (string description, Func<TextCommand, Task<string>> command)> {
				{ "help", ("this help", (cmd) => Task.FromResult(GetHelp())) },
				{ "list", ("list devices", (cmd) => Task.FromResult(ListDevices())) },
				{ "details", ("get details of device. Usage: details <name>", (cmd) => Task.FromResult(GetDeviceDetails(cmd))) },
				{ "action", ("execute device action. Usage: action <device>.<action>([argument])", (cmd) => ExecuteAction(cmd)) },
				{ "plot", ("plot device fields. Usage: plot [--from yyyy.MM.dd-HH:mm] [--time <number>h | <number>d| <number>w] <device> [field1 field2 ...]", (cmd) => Plot(cmd)) },
				{ "buttons", ("list of buttons", (cmd) => Task.FromResult(GetButtonsList())) },
				{ "newbutton", ("create new button. Usage: newbutton <button name> <command>", (cmd) => CreateButton(cmd)) },
				{ "delbutton", ("delete button. Usage: delbuttons <button>", (cmd) => Task.FromResult(DelButton(cmd))) }
			};

			buttons = File.Exists(CommandButtonsFile)
				          ? JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(CommandButtonsFile))
				          : new Dictionary<string, string>();
		}

		private string GetHelp() {
			var sb = new StringBuilder();
			sb.Append("List of commands:" + Environment.NewLine);
			foreach (KeyValuePair<string, (string description, Func<TextCommand, Task<string>> command)> comm in _predefinedCommands)
				sb.AppendFormat("{0} - {1}\r\n", comm.Key, comm.Value.description);

			sb.Append(GetButtonsList());
		
			return sb.ToString();
		}

		private string GetButtonsList() {
			var sb = new StringBuilder();
			sb.Append("List of buttons:" + Environment.NewLine);
			foreach (KeyValuePair<string, string> comm in _commandButtons)
				sb.AppendFormat("{0} - {1}\r\n", comm.Key, comm.Value);

			return sb.ToString();
		}

		private string ListDevices() {
			var sb = new StringBuilder();
			sb.Append("Id   Name" + Environment.NewLine);

			foreach (IDevice device in _deviceManagerService.DeviceRepository.List<IDevice>())
				sb.AppendFormat("{0,-4} {1}" + Environment.NewLine, device.Id, device.Name);

			return sb.ToString(); 
		}

		private string GetDeviceDetails(TextCommand cmd) {
			var parser = new CommandLineParser(cmd.Command);

			if (parser.Arguments.Count != 1)
				return "Invalid usage. Try 'help' for help.";

			string deviceName = parser.Arguments[1];

			IDevice device = _deviceManagerService.DeviceRepository.List<IDevice>(x => string.Compare(x.Name, deviceName, true, CultureInfo.InvariantCulture) == 0).FirstOrDefault();

			if (device == null)
				return $"Can not find device with name [{deviceName}]";

			var sb = new StringBuilder();
			sb.AppendFormat("Id = {0} , Name = {1}, Type = {2},"
			                + Environment.NewLine + "State = {3}" + Environment.NewLine,
			                device.Id, device.Name, device is IRemoteDevice ? "Remote" : "Internal", device.State);
			sb.Append("Fields:" + Environment.NewLine);
			foreach (DeviceStateFieldDeclaration f in device.Info.Fields)
				sb.Append(f + Environment.NewLine);
			sb.Append("Actions:" + Environment.NewLine);
			foreach (DeviceActionDeclaration a in device.Info.Actions)
				sb.Append(a + Environment.NewLine);
			sb.Append("Events:" + Environment.NewLine);
			foreach (DeviceEventDeclaration e in device.Info.Events)
				sb.Append(e + Environment.NewLine);

			return sb.ToString();
		}

		private async Task<string> Plot(TextCommand cmd) {
			var cmdLine = new CommandLineParser(cmd.Command);

			if (cmdLine.Arguments.Count < 2)
				return "Invalid usage. Try 'help' for help.";

			string deviceName = cmdLine.Arguments[0];

			IDevice device = _deviceManagerService.DeviceRepository.List<IDevice>(x => string.Compare(x.Name, deviceName, true) == 0).FirstOrDefault();

			if (device == null)
				return $"Can not found device with name [{deviceName}].";

			DateTime from = DateTime.Today;
			DateTime to = DateTime.Now;

			TimeSpan dt = to - from;
			
			if (cmdLine.Options.ContainsKey("time")) {
				string time = cmdLine.Options["time"];
				TimeSpan timeUnit;
				switch (time.Last()) {
					case 'h':
						timeUnit = TimeSpan.FromHours(1);
						break;
					case 'd':
						timeUnit = TimeSpan.FromDays(1);
						break;
					case 'w':
						timeUnit = TimeSpan.FromDays(7);
						break;
					default:
						return $"Unknown time unit [{time.Last()}]. Only h (hours), d (days) and w (weeks) are supported.";
				}
				try {
					uint count = uint.Parse(time.Substring(0, time.Length - 1));
					dt = count * timeUnit;
				} catch (FormatException) {
					return $"Invalid time option format [{time}]. Should be <number><time unit> without spaces.";
				} catch (OverflowException) {
					return "Time option value is too big.";
				}

				if (dt > TimeSpan.FromDays(30))
					dt = TimeSpan.FromDays(30);

				from = to - dt;
			}

			if (cmdLine.Options.ContainsKey("from")) {
				try {
					from = DateTime.ParseExact(cmdLine.Options["from"], "yyyy.MM.dd-HH:mm", CultureInfo.InvariantCulture);
				} catch (FormatException) {
					return $"Invalid from option format [{from}]. Should be yyyy.MM.dd-HH:mm.";
				}
			}
	
			if (from > DateTime.Now)
				return "I can't predict future =(";

			to = from + dt;

			if (to > DateTime.Now)
				to = DateTime.Now;

			var plotData = new List<TimeSeries>();

			for (int i = 1; i < cmdLine.Arguments.Count; i++) {
				DeviceStateFieldDeclaration field = device.Info.Fields.FirstOrDefault(x => string.Compare(x.Name, cmdLine.Arguments[i], true) == 0);
				if (field == null)
					return $"Can not found device{{ Name = {deviceName} }} field [{cmdLine.Arguments[i]}].";

				var series = new TimeSeries { Name = field.Name };

				IEnumerable<DeviceStateMeasurement> measurments = _stateRepository.List(x => x.DeviceId == device.Id && from <= x.Time && x.Time <= to);

				switch (field.Type.Type) {
					case VariantType.Float:
						series.Data = DeviceStateFieldExporter.GetFloatFieldValues(measurments, field.Name).ToList();
						break;
					case VariantType.Enum:
						series.Data = DeviceStateFieldExporter.GetEnumFieldValues(measurments, field.Name, field.Type.EnumValues).ToList();
						series.YAxisLabels = field.Type.EnumValues;
						break;
					default:
						return $"Can not plot [{field.Type.Type.ToString().ToLower()}] field [{field.Name}].";
				}
				plotData.Add(series);
			}

			if (plotData.Count == 0)
				return "Nothing to plot.";

			if (plotData.All(x => x.Data.Count == 0))
				return $"No data from {from:yyyy.MM.dd-HH:mm} to {to:yyyy.MM.dd-HH:mm}";

			TimeSpan pollingRate = TimeSpan.FromSeconds(60);
			if (device is IRemoteDevice)
				pollingRate = TimeSpan.FromSeconds(((IRemoteDevice)device).PollingRate);

			using (var stream = new MemoryStream()) {
				_plotService.Plot(device.Name, plotData, stream, pollingRate);
				var filename = $"{plotData.Aggregate(device.Name, (acc, x) => acc + "_" + x.Name)}_{from:yyyy.MM.dd-HH:mm}_{to:yyyy.MM.dd-HH:mm}.png";
				await _commandProvider.SendFileMessage(cmd.ChannelId, stream, filename);
			}

			return string.Empty;
		}

		private async Task<string> ExecuteAction(TextCommand cmd) {
			if (!Regex.IsMatch(cmd.Command, ActionPattern))
				return "Invalid syntax. Try 'help' for help.";

			string[] splits = cmd.Command.Split(new[] { '.', '(', ')', ' ' }, StringSplitOptions.RemoveEmptyEntries);
			string deviceName = splits[1];
			string action = splits[2];
			string argument = splits[3];

			IDevice device = _deviceManagerService.DeviceRepository.List<IDevice>(x => string.Compare(x.Name, deviceName, true) == 0).FirstOrDefault();

			if (device == null)
				return $"Can not found device with name [{deviceName}]";

			DeviceActionDeclaration deviceAction = device.Info.Actions.FirstOrDefault(x => string.Compare(x.Path, action, true) == 0);

			if (deviceAction == null)
				return $"Device {{ Name = {deviceName} }} does not contain action [{action}].";
			try {
				await _deviceManagerService.ExecuteAction(device.Id, action, argument);
				await _notificationService.Notify($"Device {{ Name = {deviceName}, Id = {device.Id} }} action {action}({argument}) has been executed by {cmd.UserName} via bot.");
				return "Ok.";
			} catch (CO2MonitorException ex) {
				_logger.LogError(ex, "Can not execute action");
				return $"Error: {ex.Message}.";
			}
		}

		private async Task<string> CreateButton(TextCommand cmd) {
			string[] words = cmd.Command.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
			if (words.Length < 3)
				return "Invalid usage. Try 'help' for help.";
			var button = words[1].ToLower();
			if (!(button.All(x => char.IsLetterOrDigit(x) || x == '_')))
				return "Button name must consist of letters, digits and '_'";

			if (_predefinedCommands.Keys.Contains(button))
				return "You Button name can not same as predefined commands.";
			if (_commandButtons.Keys.Contains(button))
				return $"Button [{button}] has already exited";

			var command = cmd.Command.Substring(cmd.Command.IndexOf(words[1]) + words[1].Length);
			_commandButtons.Add(button, command);
			File.WriteAllText(CommandButtonsFile, JsonConvert.SerializeObject(_commandButtons));
			await _notificationService.Notify($"Button {{ Name = {button}, Command = {command} }} was created by {cmd.UserName} via bot.");
			return "Ok.";
		}

		private string DelButton(TextCommand cmd) {
			string[] words = cmd.Command.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
			if (words.Length < 2)
				return "Invalid usage. Try 'help' for help.";

			string alies = words[1].ToLower();
			if (!_commandButtons.ContainsKey(alies))
				return $"Can not find button [{alies}].";

			_commandButtons.Remove(alies);
			File.WriteAllText(CommandButtonsFile, JsonConvert.SerializeObject(_commandButtons));
			return "Ok.";
		}
	}
}