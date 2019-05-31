using System;
using System.Collections.Generic;
using System.Text;

namespace CO2Monitor.Core.Interfaces
{
    public enum FanCommand
    {
        TurnOff,
        TurnOn
    }

    public interface IRemoteCO2FanController
    {
        void SetCommamd(string address, FanCommand command);

        FanCommand GetState(string address);

        FanCommand GetCommand(string address);
    }
}
