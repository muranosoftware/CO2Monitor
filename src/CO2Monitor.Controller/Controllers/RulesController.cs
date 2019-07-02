using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using CO2Monitor.Core.Interfaces;
using CO2Monitor.Core.Entities;
using CO2Monitor.Core.Shared;


namespace CO2Monitor.Controller.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RulesController : ControllerBase
    {

        private readonly IDeviceManagerService _deviceManager;
        private readonly ILogger<DevicesController> _logger;
        public RulesController(ILogger<DevicesController> logger, IDeviceManagerService deviceManager)
        {
            _deviceManager = deviceManager;
            _logger = logger;
        }

        [HttpGet()]
        public IEnumerable<ActionRule> GetActionRules()
        {
            return _deviceManager.RuleRepository.List();
        }

        [HttpPost()]
        public IActionResult CreateActionRule([FromBody, Required] ActionRule rule, [FromQuery] string actionArgument)
        {
            try
            {
                rule.ActionArgument = actionArgument; //new Value(rule.Action.Argument, actionArgument);

                return Ok(_deviceManager.RuleRepository.Add(rule));
            }
            catch (CO2MonitorException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete()]
        public IActionResult DeleteActionRule([FromQuery, Required] int id)
        {
            try
            {
                if (_deviceManager.RuleRepository.Delete(x => x.Id == id))
                    return Ok();
                else
                    return NotFound(id);
            }
            catch (CO2MonitorException ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}