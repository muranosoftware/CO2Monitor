using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CO2Monitor.Core.Interfaces
{
    public enum FanCommand
    {
        Off,
        On
    }

    public enum FanLed
    {
        Green,
        Yellow,
        Red,
    }

    public interface IRemoteCO2FanController
    {
        Task SetCommamd(string address, FanCommand command);

        Task SetLed(string address, FanLed led);

        //FanCommand GetState(string address);

        //Task<FanCommand> GetCommand(string address);
    }
}
