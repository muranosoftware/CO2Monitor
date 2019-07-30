using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CO2Monitor.Application.ViewModels;
using CO2Monitor.Core.Interfaces.Notifications;
using CO2Monitor.Application.Interfaces;
using System.ComponentModel.DataAnnotations;
using CO2Monitor.Core.Shared;

namespace CO2Monitor.Controller.ApiControllers {
	[Route("api/[controller]")]
	[ApiController]
	public class DevicesController : ControllerBase {
		private readonly IDeviceAppService _deviceAppService;
		private readonly IEventNotificationService _notificationService;
		
		public DevicesController(IDeviceAppService deviceAppService, IEventNotificationService eventNotificationService) {
			_deviceAppService = deviceAppService;
			_notificationService = eventNotificationService;
		}

		[HttpGet]
		public IEnumerable<DeviceViewModel> GetDevices() => _deviceAppService.List<DeviceViewModel>();

		[HttpDelete()]
		public IActionResult DeleteDevice([FromBody, Required] DeviceViewModel deviceViewModel) {
			if (_deviceAppService.Delete(deviceViewModel)) {
				_notificationService.Notify($"Device {{ Id = {deviceViewModel.Id} }} has been deleted via web api");
				return Ok();
			} else {
				return NotFound();
			}
		}

		[HttpGet("timers")]
		public IEnumerable<ScheduleTimerViewModel> GetTimers() => _deviceAppService.List<ScheduleTimerViewModel>();
		
		[HttpPost("timers")]
		public IActionResult CreateTimer([FromBody, Required] ScheduleTimerViewModel scheduleTimerViewModel) {
			ScheduleTimerViewModel timer = _deviceAppService.Create(scheduleTimerViewModel);
			_notificationService.Notify($"New Timer has been created {{ Name = {timer.Id} Id = {timer.Name} AlarmTime = {timer.AlarmTime} }} via web api");
			return Ok(timer);
		}

		[HttpPatch("timers")]
		public IActionResult PatchTimer([FromBody, Required] ScheduleTimerViewModel scheduleTimerViewModel) {
			ScheduleTimerViewModel timer = _deviceAppService.Edit(scheduleTimerViewModel);
			_notificationService.Notify($"Timer has been edited {{ Name = {timer.Name} Id = {timer.Id} AlarmTime = {timer.AlarmTime} }}  via web api");
			return Ok(timer);
		}

		[HttpGet("remote")]
		public IEnumerable<RemoteDeviceViewModel> GetRemote() => _deviceAppService.List<RemoteDeviceViewModel>();

		[HttpPost("remote")]
		public IActionResult CreateRemoteDevice([FromBody, Required] RemoteDeviceViewModel remoteDeviceViewModel) {
			RemoteDeviceViewModel remote = _deviceAppService.Create(remoteDeviceViewModel);
			_notificationService.Notify($"New remote device has been created {{ Name = {remote.Name} Id = {remote.Id} }} via web api");
			return Ok(remote);
		}

		[HttpPut("action")]
		public async Task<IActionResult> ExecuteAction([FromQuery, Required] int deviceId, [FromBody, Required] ActionViewModel actionViewModel, [FromQuery] string argument) {
			try {
				await _deviceAppService.ExecuteAction(deviceId, actionViewModel, argument);
				return Ok();
			} catch (CO2MonitorArgumentException ex) {
				return BadRequest(new { ex.Argument, ex.Message });
			}
		}

		[HttpGet("extensionTypes")]
		public IEnumerable<string> GetExtensionTypes() => _deviceAppService.GetDeviceExtensionsTypes();

		[HttpPost("extensions")]
		public IActionResult CreateExtension([FromQuery, Required] int deviceId, [FromBody, Required] DeviceExtensionViewModel deviceExtensionViewModel) {
			try {
				_deviceAppService.CreateDeviceExtension(deviceId, deviceExtensionViewModel);
				_notificationService.Notify($"Extension {{ Type = {deviceExtensionViewModel.Type}, Paramemter = {deviceExtensionViewModel.Parameter} }} has been added to device {{ Id = {deviceId} }} via web api");
				return Ok();
			} catch (CO2MonitorArgumentException ex) {
				return BadRequest(new { ex.Argument, ex.Message });
			} catch (CO2MonitorConflictException ex) {
				return new ConflictObjectResult(new { ex.Message });
			}
		}
	}
}