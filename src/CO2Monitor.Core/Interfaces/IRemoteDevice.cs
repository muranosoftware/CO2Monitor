using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using CO2Monitor.Core.Entities;

namespace CO2Monitor.Core.Interfaces
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum RemoteDeviceStatus
    {
        NotAccessible,
        Ok,
    }

    public interface IRemoteDevice : IDevice
    {
        string Address { get; set; }

        float PollingRate { get; set; }

        DateTime? LatestSuccessfullAccess { get; }

        RemoteDeviceStatus Status { get; }

        string State { get; }

        void UpdateInfo();
    }
}
