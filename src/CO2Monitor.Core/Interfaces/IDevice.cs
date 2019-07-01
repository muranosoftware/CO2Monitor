using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using CO2Monitor.Core.Entities;

namespace CO2Monitor.Core.Interfaces
{
  public interface IDevice : IBaseDevice
    {
        int Id { get; set; }

        bool IsRemote { get; }

        bool IsExtensible { get; }

        IReadOnlyCollection<IDeviceExtention> DeviceExtentions { get; }

        void AddExtention(IDeviceExtention extention);
    }
}
