using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using MoreLinq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using CO2Monitor.Core.Entities;
using CO2Monitor.Core.Shared;
using CO2Monitor.Core.Interfaces.Devices;

namespace CO2Monitor.Infrastructure.Devices {
	public class FloatToGyrColorDeviceExtension : IDeviceExtension {
		[JsonConverter(typeof(StringEnumConverter))]
		public enum GyrColor {
			Green,
			Yellow,
			Red,
		}

		private static readonly VariantDeclaration GyrColorEnum = new VariantDeclaration(VariantType.Enum, Enum.GetNames(typeof(GyrColor)).Select(x => x.ToLower()).ToList());

		private IReadOnlyDictionary<DeviceStateFieldDeclaration, Func<Variant>> _fields;

		private IReadOnlyDictionary<DeviceActionDeclaration, Action<Variant>> _actions;

		private IReadOnlyList<DeviceEventDeclaration> _events;

		private DeviceEventDeclaration _levelChangedDeclaration;

		private string _field;
		private bool _disableValidation;
		private double _yellowLevel = 1000.0;
		private double _redLevel = 1200.0;

		[JsonConstructor]
		private FloatToGyrColorDeviceExtension() { }

		public FloatToGyrColorDeviceExtension(string field, IExtendableDevice device) {
			if (device == null) {
				throw new CO2MonitorArgumentException(nameof(device));
			}

			if (string.IsNullOrWhiteSpace(field)) {
				throw new CO2MonitorArgumentException(nameof(field), "Can not be null or whitespace");
			}

			if (device.Info.Fields.FirstOrDefault(x => x.Name == field && x.Type.Type == VariantType.Float) == null) {
				throw new CO2MonitorArgumentException(nameof(field), $"Device {{ Id = {device.Id}, Name = {device.Name} }} does not contain float field [{field}]");
			}

			FieldName = field;

			if (device.Extensions.Any(x => x.Name == Name)) {
				throw new CO2MonitorException($"Device {{ Id = {device.Id}, Name = {device.Name} }} has already contains [{Name}] extension!");
			}
		}

		public string Name { get; set; } = "FloatToGyrColor";

		public string FieldName {
			get => _field;
			set {
				Name = value + "ToGyrColor";
				_field = value;

				CreateFieldAndActionDictionariesAndEventsList(_field);
			}
		}

		public DeviceInfo BaseInfo => new DeviceInfo(_fields.Keys, _actions.Keys, _events);

		public string State {
			get {
				var json = new JObject();
				_fields.ForEach(f => json.Add(new JProperty(f.Key.Name, f.Value().String)));
				return json.ToString();
			}
		}

		public double YellowLevel {
			get => _yellowLevel;
			set {
				if (!_disableValidation && value >= _redLevel) {
					throw new CO2MonitorArgumentException();
				}

				_yellowLevel = value;
				SettingsChanged?.Invoke(this, new PropertyChangedEventArgs("YellowLevel"));
			}
		}

		public double RedLevel {
			get => _redLevel;
			set {
				if (!_disableValidation && value <= _yellowLevel) {
					throw new CO2MonitorArgumentException();
				}

				_redLevel = value;
				SettingsChanged?.Invoke(this, new PropertyChangedEventArgs("RedLevel"));
			}
		}

		public GyrColor? Level { get; set; }

		[OnDeserializing]
		private void OnDeserializing(StreamingContext context) => _disableValidation = true;

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context) => _disableValidation = false;

		public event PropertyChangedEventHandler SettingsChanged;
		public event DeviceEventHandler EventRaised;

		public void Dispose() { }

		public async Task Execute(IDevice device) {
			if (device is IRemoteDevice remote && remote.Status == RemoteDeviceStatus.NotAccessible) {
				Level = null;
				return;
			}

			var fieldDeclaration = new DeviceStateFieldDeclaration(_field, VariantDeclaration.Float);
			if (!device.Info.Fields.Contains(fieldDeclaration)) {
				throw new CO2MonitorException("Device does not contains field " + fieldDeclaration);
			}

			Variant field = await device.GetField(fieldDeclaration);
			GyrColor curColor = FloatToGyrColor(field.Float);
			if (Level != curColor) {
				Level = curColor;
				EventRaised?.Invoke(this, _levelChangedDeclaration, new Variant(GyrColorEnum, curColor.ToString().ToLower()));
			}
		}

		public Task ExecuteAction(DeviceActionDeclaration deviceActionDeclaration, Variant value) {
			if (!_actions.ContainsKey(deviceActionDeclaration)) {
				throw new InvalidOperationException();
			}

			_actions[deviceActionDeclaration](value);
			return Task.CompletedTask;
		}

		public Task<Variant> GetField(DeviceStateFieldDeclaration fieldDeclaration) {
			if (!_fields.ContainsKey(fieldDeclaration)) {
				throw new InvalidOperationException();
			}

			return Task.FromResult(_fields[fieldDeclaration]());
		}

		private void CreateFieldAndActionDictionariesAndEventsList(string field) {
			_fields = new Dictionary<DeviceStateFieldDeclaration, Func<Variant>> {
				{
					new DeviceStateFieldDeclaration(field + "Level", GyrColorEnum),
					() => Level == null ? new Variant() : new Variant(GyrColorEnum, Level.ToString().ToLower())
				},
				{
					new DeviceStateFieldDeclaration(field + "RedLevel", VariantDeclaration.Float),
					() => new Variant(RedLevel)
				},
				{
					new DeviceStateFieldDeclaration(field + "YellowLevel", VariantDeclaration.Float),
					() => new Variant(YellowLevel)
				},
			};

			_actions = new Dictionary<DeviceActionDeclaration, Action<Variant>> {
				{
					new DeviceActionDeclaration(field + "SetRedLevel", VariantDeclaration.Float),
					val => {
						if (val.Declaration != VariantDeclaration.Float) {
							throw new CO2MonitorArgumentException(); }
						RedLevel = val.Float;
					}
				},
				{
					new DeviceActionDeclaration(field + "SetYellowLevel", VariantDeclaration.Float),
					val => {
						if (val.Declaration != VariantDeclaration.Float) {
							throw new CO2MonitorArgumentException(); }
						YellowLevel = val.Float;
					}
				},
			};

			_levelChangedDeclaration = new DeviceEventDeclaration(field + "LevelChanged", GyrColorEnum);

			_events = new[] { _levelChangedDeclaration };
		}

		private GyrColor FloatToGyrColor(double val) =>
			val < YellowLevel ? GyrColor.Green : (val >= RedLevel ? GyrColor.Red : GyrColor.Yellow);
	}
}
