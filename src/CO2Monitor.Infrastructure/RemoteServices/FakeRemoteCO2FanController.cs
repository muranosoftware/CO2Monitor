using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using CO2Monitor.Core.Interfaces;

namespace CO2Monitor.Infrastructure.RemoteServices
{
    public class FakeRemoteCO2FanController : IRemoteCO2FanController
    {
        private FanCommand _command;

        //public Task<FanCommand> GetCommand(string address)
        //{
        //    return _command;
        //}
        //
        //public Task<FanCommand> GetState(string address)
        //{
        //    return _command;
        //}

        public Task SetCommamd(string address, FanCommand command)
        {
            _command = command;
            return Task.CompletedTask;
        }

        public Task SetLed(string address, FanLed led)
        {
            return Task.CompletedTask;
        }
    }
}
