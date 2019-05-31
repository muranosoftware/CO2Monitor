using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using CO2Monitor.Core.Entities;
using CO2Monitor.Core.Interfaces;
using Microsoft.Extensions.Hosting;

namespace CO2Monitor.Controller.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CO2MonitorController : ControllerBase
    {
        private readonly ICO2ControllerService _controllerService;

        public CO2MonitorController(IEnumerable<IHostedService> hostedServicies)
        {
            _controllerService = hostedServicies.OfType<ICO2ControllerService>().First(); 
        }

        [HttpGet("sensorAddress")]
        public string GetSensorAddress()
        {
            return _controllerService.CO2DriverAddress;
        }

        [HttpPut("sensorAddress/{address}")]
        public IActionResult SetSensorAddress([FromRoute, Required] string address)
        {
            _controllerService.CO2DriverAddress = address;
            return Ok(address);
        }

        [HttpGet("fanAddress")]
        public string GetFanAddress()
        {
            return _controllerService.CO2FanDriverAddress;
        }

        [HttpPut("fanAddress/{address}")]
        public IActionResult SetFanAddress([FromRoute, Required] string address)
        {
            _controllerService.CO2FanDriverAddress = address;
            return Ok(address);
        }

        [HttpGet("pollingRate")]
        public float GetPollingRate()
        {
            return _controllerService.PollingRate;
        }

        [HttpPut("pollingRate/{rate}")]
        public float SetPollingRate([FromRoute, Required] float address)
        {
            return _controllerService.PollingRate;
        }

        [HttpGet("level/{level}")]
        public int GetLevel([Required, RegularExpression("^(normal|mid|high)$")] string level)
        {
            level = char.ToUpper(level[0]) + level.Substring(1);
            return _controllerService.GetLevel(Enum.Parse<CO2Levels>(level));
        }

        [HttpPut("level/{level}/{value}")]
        public IActionResult SetLevel([Required, RegularExpression("^(normal|mid|high)$")] string level, [Required] int value)
        {
            level = char.ToUpper(level[0]) + level.Substring(1);
            _controllerService.SetLevel(Enum.Parse<CO2Levels>(level), value);
            return Ok();
        }


        [HttpGet("latestMeasurement")]
        public CO2Measurement GetLatestMeasurement()
        {
            return _controllerService.GetLatestMeasurement();
        }
    }
}