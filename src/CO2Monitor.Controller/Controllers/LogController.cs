using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using CO2Monitor.Core.Entities;
using CO2Monitor.Core.Interfaces;

namespace CO2Monitor.Controller.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly ILogViewer _logViewer;
        public LogController(ILogViewer logViewer)
        {
            _logViewer = logViewer;
        }

        [HttpGet]
        public IEnumerable<LogRecord> GetLogRecords([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] int limit = 1000 )
        {
            return _logViewer.GetRecords(from, to, limit);
        }
    }
}