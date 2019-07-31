﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using CO2Monitor.Domain.Interfaces.Services;

namespace CO2Monitor.Controller.ApiControllers {
	[Route("api/[controller]")]
	[ApiController]
	public class LogController : ControllerBase {
		private readonly ILogViewer _logViewer;

		public LogController(ILogViewer logViewer) {
			_logViewer = logViewer;
		}

		[HttpGet]
		public IEnumerable<LogRecord> GetLogRecords([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] uint limit = 1000) => _logViewer.GetRecords(from, to, limit);
	}
}