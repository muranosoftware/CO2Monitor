using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using CO2Monitor.Core.Entities;
using CO2Monitor.Core.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace CO2Monitor.Controller.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CO2StatController : ControllerBase
    {
        private readonly IMeasurementRepository _repository;
        public CO2StatController(IMeasurementRepository repository)
        {
            _repository = repository;
        }

        [HttpGet()]
        public IEnumerable<CO2Measurement> GetAll()
        {
            return _repository.List();
        }

        [HttpGet("from/{from}/to/{to}")]
        public IEnumerable<CO2Measurement> GetInRange([FromRoute, Required] DateTime from, [FromRoute, Required] DateTime to)
        {
            return _repository.List(from, to);
        }

    }
}