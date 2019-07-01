using System;
using System.Collections.Generic;
using System.Text;

namespace CO2Monitor.Core.Interfaces
{
    public interface IDeviceFactory
    {
        IEnumerable<Type> GetDeviceTypes();

        IDevice CreateDevice(string name);

        T CreateDevice<T>() where T : IDevice;
    }
}
