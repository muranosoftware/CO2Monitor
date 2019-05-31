using System;
using System.Collections.Generic;
using System.Text;

using CO2Monitor.Core.Interfaces;

namespace CO2Monitor.Infrastructure.RemoteServices
{
    public class FakeRemoteCO2FanController : IRemoteCO2FanController
    {
        private FanCommand _command;

        public FanCommand GetCommand(string address)
        {
            return _command;
        }

        public FanCommand GetState(string address)
        {
            return _command;
        }

        public void SetCommamd(string address, FanCommand command)
        {
            _command = command;
        }
    }
}
