using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using CO2Monitor.Core.Interfaces;

namespace CO2Monitor.Controller.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CO2DriverController : ControllerBase
    {
        private readonly ICO2ControllerService _controllerService;

        public CO2DriverController(ICO2ControllerService controllerService)
        {
            _controllerService = controllerService;
        }

        [HttpGet("address")]
        public string GetAddress()
        {
            return _controllerService.CO2DriverAddress;
        }

        [HttpPut("address")]
        public IActionResult SetAddress([FromQuery, Required] string adress)
        {
            _controllerService.CO2DriverAddress = adress;
            return Ok(adress);
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

    }
}