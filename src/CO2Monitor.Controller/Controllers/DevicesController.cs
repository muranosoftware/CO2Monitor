using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CO2Monitor.Core.Shared;
using CO2Monitor.Core.Entities;
using CO2Monitor.Core.Interfaces.Services;
using CO2Monitor.Core.Interfaces.Devices;
using CO2Monitor.Core.Interfaces.Notifications;

namespace CO2Monitor.Controller.Controllers {
	[Route("api/[controller]")]
	[ApiController] 
	public class DevicesController : ControllerBase {
		private readonly IDeviceManagerService _deviceManager;
		private readonly IEventNotificationService _notificationService;
		private readonly ILogger<DevicesController> _logger;

		public DevicesController(ILogger<DevicesController> logger, 
								 IDeviceManagerService deviceManager, 
								 IEventNotificationService notificationService) {
			_logger = logger;
			_deviceManager = deviceManager;
			_notificationService = notificationService;
		}

		[HttpGet()]
		public IEnumerable<IDevice> GetAllDevices() {
			return _deviceManager.DeviceRepository.List<IDevice>();
		}

		[HttpGet("timers")]
		public IEnumerable<IDevice> GetTimers() {
			return _deviceManager.DeviceRepository.List<IScheduleTimer>();
		}

		[HttpPost("timers")]
		public async Task<IActionResult> CreateTimer([FromQuery, Required] string name, [FromQuery, Required] string time) {
			using (_logger.BeginScope("Creating new timer [{0}] at [{1}]", name, time)) {
				try {
					TimeSpan timeSpan = TimeSpan.Parse(time);
					if (timeSpan.TotalHours > 24)
						throw new FormatException();

					IScheduleTimer timer = _deviceManager.CreateTimer(name, timeSpan);
					await _notificationService.Notify($"New Timer has been created {{ Name = {timer.Id} Id = {timer.Name} AlarmTime = {timer.AlarmTime} }} via web api");

					return Ok(timer);
				} catch (CO2MonitorException ex) {
					return BadRequest(ex.Message);
				} catch (FormatException) {
					return BadRequest("Bad time format");
				}
			}
		}

		[HttpPatch("timers")]
		public async Task<IActionResult> PatchTimer([FromQuery, Required] int id, [FromQuery] string name, [FromQuery] string time) {
			try {
				var timer = _deviceManager.DeviceRepository.GetById<IScheduleTimer>(id);
				if (timer == null)
					return NotFound(id);
				if (!string.IsNullOrWhiteSpace(time)) {
					try {
						TimeSpan timeSpan = TimeSpan.Parse(time);
						if (timeSpan.TotalHours > 24)
							throw new FormatException();
						timer.AlarmTime = timeSpan;
					} catch (FormatException) {
						return BadRequest("Bad time format");
					}
				}
				if (!string.IsNullOrWhiteSpace(name))
					timer.Name = name;

				await _notificationService.Notify($"Timer has been edited {{ Name = {timer.Name} Id = {timer.Id} AlarmTime = {timer.AlarmTime} }}  via web api");

				return Ok(timer);
			} catch (CO2MonitorException ex) {
				_logger.LogInformation(ex, "Can not path timer.");
				return BadRequest(ex.Message);
			}
		}

		[HttpDelete()]
		public async Task<IActionResult> DeleteDevice([FromQuery, Required] int id) {
			try {
				if (_deviceManager.DeviceRepository.Delete<IDevice>(x => x.Id == id)) { 
					await _notificationService.Notify($"Device {{ Id = {id} }} has been deleted via web api");
					return Ok();
				} else {
					return NotFound();
				}
			} catch (CO2MonitorException ex) {
				_logger.LogInformation(ex, "Can not delete device.");
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("remote")]
		public IEnumerable<IRemoteDevice> GetRemoteDevices() {
			return _deviceManager.DeviceRepository.List<IRemoteDevice>();
		}

		[HttpPost("remote")]
		public async Task<IActionResult> CreateRemoteDevice([FromQuery, Required] string address, [FromQuery, Required] string name, [FromBody, Required] DeviceInfo deviceInfo) {
			try {
				if (Uri.IsWellFormedUriString(address, UriKind.Absolute))
					throw new CO2MonitorArgumentException("address has bad format!");

				var uri = new Uri(address);

				if (uri.Scheme != "http" && uri.Scheme != "https")
					throw new CO2MonitorArgumentException("address is not http(s) link");

				if (string.IsNullOrEmpty(name))
					throw new CO2MonitorArgumentException("name can not be null or whitespace!");

				if (!ModelState.IsValid) { 
					return BadRequest(ModelState);
				}

				IRemoteDevice remoteDevice = _deviceManager.CreateRemoteDevice(uri, name, deviceInfo);

				await _notificationService.Notify($"New remote device has been created {{ Name = {remoteDevice.Name} Id = {remoteDevice.Id} }} via web api");

				return Ok(remoteDevice);
			} catch (CO2MonitorException ex) {
				_logger.LogInformation(ex, "Can not create remote.");
				return BadRequest(ex.Message);
			}
		}

		[HttpPut("action")]
		public async Task<IActionResult> ExecuteAction([FromQuery, Required] int deviceId, [FromQuery, Required] string action, [FromQuery] string argument) {
			try {
				await _deviceManager.ExecuteAction(deviceId, action, argument);
				await _notificationService.Notify($"Device {{ Id = {deviceId} }} action {action}({argument}) executed via web api");
				return Ok();
			} catch (CO2MonitorException ex) {
				_logger.LogInformation(ex, "Can not execute action via web-api.");
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("extensionTypes")]
		public IEnumerable<string> GetExtensionTypes() {
			return _deviceManager.GetDeviceExtensionsTypes().Select(x => x.Name);
		}

		[HttpPost("extensions")]
		public async Task<IActionResult> CreateExtension([FromQuery, Required] int deviceId, [FromQuery, Required] string type, [FromQuery] string parameter) {
			Type extType = _deviceManager.GetDeviceExtensionsTypes().FirstOrDefault(x => x.Name == type);

			if (type == null)
				return NotFound(type);

			try {
				IDeviceExtension extension = _deviceManager.CreateDeviceExtension(extType, deviceId, parameter);
				await _notificationService.Notify($"Extension {{ Type = {extType}, Paramemter = {parameter} }} has been added to device {{ Id = {deviceId} }} via web api");
				return Ok(extension);
			} catch (CO2MonitorException ex) {
				_logger.LogInformation(ex, "Can not create extension.");
				return BadRequest(ex.Message);
			}
		}
	}
}