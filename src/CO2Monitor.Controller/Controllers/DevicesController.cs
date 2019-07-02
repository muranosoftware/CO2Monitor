using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using CO2Monitor.Core.Shared;
using CO2Monitor.Core.Interfaces;
using CO2Monitor.Core.Entities;

namespace CO2Monitor.Controller.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DevicesController : ControllerBase
    {
        private readonly IDeviceManagerService _deviceManager;
        private readonly ILogger<DevicesController> _logger;

        public DevicesController(ILogger<DevicesController> logger, IDeviceManagerService deviceManager)
        {
            _deviceManager = deviceManager;
            _logger = logger;
        }

        
        [HttpGet()]
        public IEnumerable<IDevice> GetAllDevices()
        {
            return _deviceManager.DeviceRepository.List<IDevice>();
        }

        [HttpGet("timers")]
        public IEnumerable<IDevice> GetTimers()
        {
            return _deviceManager.DeviceRepository.List<IScheduleTimer>();
        }

        [HttpPost("timers")]
        public IActionResult CreateTimer([FromQuery, Required] string name, [FromQuery, Required] string time)
        {
            using (_logger.BeginScope("Creating new timer [{0}] at [{1}]", name, time))
            {
                try
                {
                    var timeSpan = TimeSpan.Parse(time);
                    if (timeSpan.TotalHours > 24)
                        throw new FormatException();
                    return Ok(_deviceManager.CreateTimer(name, timeSpan));

                }
                catch (CO2MonitorException ex)
                {
                    return BadRequest(ex.Message);
                }
                catch (FormatException)
                {
                    return BadRequest("Bad time format");
                }
            }
        }

        [HttpPatch("timers")]
        public IActionResult PatchTimer([FromQuery, Required] int id, [FromQuery] string name, [FromQuery] string time)
        {
            try
            {
                var timer = _deviceManager.DeviceRepository.GetById<IScheduleTimer>(id);
                if (timer == null)
                    return NotFound(id);
                if (!string.IsNullOrWhiteSpace(time))
                {
                    var timeSpan = TimeSpan.Parse(time);
                    if (timeSpan.TotalHours > 24)
                        throw new FormatException();
                    timer.AlarmTime = timeSpan;
                }

                if (!string.IsNullOrWhiteSpace(name))
                    timer.Name = name;

                return Ok(timer);
            }
            catch (CO2MonitorException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (FormatException)
            {
                return BadRequest("Bad time format");
            }
        }

        [HttpDelete()]
        public IActionResult DeleteDevice([FromQuery, Required] int id)
        {
            try
            {
                if (_deviceManager.DeviceRepository.Delete<IDevice>(x => x.Id == id))
                    return Ok();
                else
                    return NotFound();
            }
            catch (CO2MonitorException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("remote")]
        public IEnumerable<IRemoteDevice> GetRemoteDevices()
        {
            return _deviceManager.DeviceRepository.List<IRemoteDevice>();
        }

        [HttpPost("remote")]
        public IActionResult CreateRemoteDevice([FromQuery, Required] string address, [FromQuery, Required] string name, [FromBody, Required] DeviceInfo deviceInfo)
        {
            try
            {
                if (string.IsNullOrEmpty(address))
                    throw new CO2MonitorArgumentException("address can not be null or whitespace!");

                if (string.IsNullOrEmpty(name))
                    throw new CO2MonitorArgumentException("name can not be null or whitespace!");


                return Ok(_deviceManager.CreateRemoteDevice(address, name, deviceInfo));
            }
            catch (CO2MonitorException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPut("action")]
        public IActionResult ExecuteAction([FromQuery, Required] int deviceId, [FromQuery, Required] string action, [FromQuery] string argument)
        {
            try
            {
                _deviceManager.ExecuteAction(deviceId, action, argument);
                return Ok();
            }
            catch (CO2MonitorException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}