using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using CO2Monitor.Core.Entities;
using CO2Monitor.Core.Interfaces.Services;
using CO2Monitor.Core.Interfaces.Notifications;

namespace CO2Monitor.Controller.ApiControllers {
	[Route("api/[controller]")]
	[ApiController]
	public class RulesController : ControllerBase {
		private readonly IDeviceManagerService _deviceManager;
		private readonly IEventNotificationService _notificationService;
		
		public RulesController(IDeviceManagerService deviceManager, IEventNotificationService notificationService) {
			_deviceManager = deviceManager;
			_notificationService = notificationService;
		}

		[HttpGet()]
		public IEnumerable<ActionRule> GetActionRules() => _deviceManager.RuleRepository.List();

		[HttpPost()]
		public IActionResult CreateActionRule([FromBody, Required] ActionRule rule) {
			ActionRule result = _deviceManager.RuleRepository.Add(rule);
			_notificationService.Notify($"New rule {{ Name = {result.Name}, Id = {result.Id} }} has been created via web-api");
			return Ok(result);
		}

		[HttpPatch()]
		public IActionResult EditActionRule([FromBody, Required] ActionRule rule) {
			if (!_deviceManager.RuleRepository.Update(rule)) {
				return NotFound();
			}

			_notificationService.Notify($"Rule {{ Name = {rule.Name}, Id = {rule.Id} }} has been edited via web-api");
			return Ok(rule);
		}

		[HttpDelete()]
		public IActionResult DeleteActionRule([FromQuery, Required] int id) {
			if (!_deviceManager.RuleRepository.Delete(x => x.Id == id)) {
				return NotFound(id);
			}

			_notificationService.Notify($"Rule {{ Id = {id} }} has been deleted via web-api");
			return Ok();
		}
	}
}