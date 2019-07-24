using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CO2Monitor.Core.Entities;
using CO2Monitor.Core.Shared;
using CO2Monitor.Core.Interfaces.Services;
using CO2Monitor.Core.Interfaces.Notifications;

namespace CO2Monitor.Controller.Controllers {
	[Route("api/[controller]")]
	[ApiController]
	public class RulesController : ControllerBase {
		private readonly IDeviceManagerService _deviceManager;
		private readonly IEventNotificationService _notificationService;
		private readonly ILogger<DevicesController> _logger;

		public RulesController(ILogger<DevicesController> logger, IDeviceManagerService deviceManager, IEventNotificationService notificationService) {
			_deviceManager = deviceManager;
			_logger = logger;
			_notificationService = notificationService;
		}

		[HttpGet()]
		public IEnumerable<ActionRule> GetActionRules() {
			return _deviceManager.RuleRepository.List();
		}

		[HttpPost()]
		public async Task<IActionResult> CreateActionRule([FromBody, Required] ActionRule rule) {
			try {
				if (!ModelState.IsValid) {
					return BadRequest(ModelState);
				}

				ActionRule result = _deviceManager.RuleRepository.Add(rule);
				await _notificationService.Notify($"New rule {{ Name = {result.Name}, Id = {result.Id} }} has been created via web-api");
				return Ok(result);
			} catch (CO2MonitorException ex) {
				_logger.LogError(ex.Message);
				return BadRequest(ex.Message);
			}
		}

		[HttpPatch()]
		public async Task<IActionResult> EditActionRule([FromBody, Required] ActionRule rule) {
			try {
				if (!ModelState.IsValid) {
					return BadRequest(ModelState);
				}

				if (!_deviceManager.RuleRepository.Delete(x => x.Id == rule.Id))
					return NotFound();
				ActionRule result = _deviceManager.RuleRepository.Add(rule);
				await _notificationService.Notify($"Rule {{ Name = {result.Name}, Id = {result.Id} }} has been edited via web-api");
				return Ok(result);
			} catch (CO2MonitorException ex) {
				_logger.LogError(ex.Message);
				return BadRequest(ex.Message);
			}
		}

		[HttpDelete()]
		public async Task<IActionResult> DeleteActionRule([FromQuery, Required] int id) {
			try {
				if (_deviceManager.RuleRepository.Delete(x => x.Id == id)) {
					await _notificationService.Notify($"Rule {{ Id = {id} }} has been deleted via web-api");
					return Ok();
				}
				return NotFound(id);
			} catch (CO2MonitorException ex) {
				return BadRequest(ex.Message);
			}
		}
	}
}