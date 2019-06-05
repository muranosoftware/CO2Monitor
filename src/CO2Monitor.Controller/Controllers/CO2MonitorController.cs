using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using CO2Monitor.Core.Entities;
using CO2Monitor.Core.Shared;
using CO2Monitor.Core.Interfaces;
using Microsoft.Extensions.Hosting;

namespace CO2Monitor.Controller.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CO2MonitorController : ControllerBase
    {
        private readonly ICO2ControllerService _controllerService;

        public CO2MonitorController(ICO2ControllerService controllerService)
        {
            _controllerService = controllerService; 
        }

        [HttpGet("sensorAddress")]
        public string GetSensorAddress()
        {
            return _controllerService.CO2DriverAddress;
        }

        [HttpPut("sensorAddress")]
        public IActionResult SetSensorAddress([FromQuery, Required] string value)
        {
            try
            {
                _controllerService.CO2DriverAddress = value;
                return Ok(value);
            }
            catch (CO2MonitorArgumentException ex)
            {
                return UnprocessableEntity(ex.Message);
            }
        }

        [HttpGet("fanAddress")]
        public string GetFanAddress()
        {
            return _controllerService.CO2FanDriverAddress;
        }

        [HttpPut("fanAddress")]
        public IActionResult SetFanAddress([FromQuery, Required] string value)
        {
            try
            {
                _controllerService.CO2FanDriverAddress = value;
                return Ok(value);
            }
            catch (CO2MonitorArgumentException ex)
            {
                return UnprocessableEntity(ex.Message);
            }
        }

        [HttpGet("pollingRate")]
        public float GetPollingRate()
        {
            return _controllerService.PollingRate;
        }

        [HttpPut("pollingRate")]
        public IActionResult SetPollingRate([FromQuery, Required] float value)
        {
            try
            {
                _controllerService.PollingRate = value;
                return Ok(value);
            }
            catch (CO2MonitorArgumentException ex)
            {
                return UnprocessableEntity(ex.Message);
            }
        }

        [HttpGet("level/{level}")]
        public int GetLevel([Required, RegularExpression("^(normal|mid|high)$")] string level)
        {
            level = char.ToUpper(level[0]) + level.Substring(1);
            return _controllerService.GetLevel(Enum.Parse<CO2Levels>(level));
        }

        [HttpPut("level/{level}")]
        public IActionResult SetLevel([Required, RegularExpression("^(normal|mid|high)$")] string level, [FromQuery, Required] int value)
        {
            try
            {
                level = char.ToUpper(level[0]) + level.Substring(1);
                _controllerService.SetLevel(Enum.Parse<CO2Levels>(level), value);
                return Ok(level);
            }
            catch (CO2MonitorArgumentException ex)
            {
                return UnprocessableEntity(ex.Message);
            };
        }


        [HttpGet("latestMeasurement")]
        public async Task<IActionResult> GetLatestMeasurement()
        {
            try
            {
                return Ok(await _controllerService.GetLatestMeasurement());
            }
            catch (CO2MonitorRemoteServiceException)
            {
                return NotFound();
            }
        }
    }
}