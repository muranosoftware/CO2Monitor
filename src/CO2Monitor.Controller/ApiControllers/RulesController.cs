using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using CO2Monitor.Application.Interfaces;
using CO2Monitor.Application.ViewModels;
using CO2Monitor.Domain.Interfaces.Services;

namespace CO2Monitor.Controller.ApiControllers {
	[Route("api/[controller]")]
	[ApiController]
	public class RulesController : ControllerBase {
		private readonly IRuleAppSevice _ruleAppSevice;
		private readonly IEventNotificationService _notificationService;
		
		public RulesController(IRuleAppSevice ruleAppSevice, IEventNotificationService notificationService) {
			_ruleAppSevice = ruleAppSevice;
			_notificationService = notificationService;
		}

		[HttpGet]
		public IEnumerable<RuleViewModel> GetActionRules() => _ruleAppSevice.List();

		[HttpPost]
		public IActionResult CreateActionRule([FromBody, Required] RuleViewModel rule) {
			RuleViewModel result = _ruleAppSevice.Create(rule);
			_notificationService.Notify($"New rule {{ Name = {result.Name}, Id = {result.Id} }} has been created via web-api");
			return Ok(result);
		}

		[HttpPatch]
		public IActionResult EditActionRule([FromBody, Required] RuleViewModel rule) {
			if (!_ruleAppSevice.Update(rule)) {
				return NotFound();
			}

			_notificationService.Notify($"Rule {{ Name = {rule.Name}, Id = {rule.Id} }} has been edited via web-api");
			return Ok(rule);
		}

		[HttpDelete]
		public IActionResult DeleteActionRule([FromBody, Required] RuleViewModel rule) {
			if (!_ruleAppSevice.Delete(rule)) {
				return NotFound(rule);
			}

			_notificationService.Notify($"Rule {{ Id = {rule.Id} }} has been deleted via web-api");
			return Ok();
		}
	}
}