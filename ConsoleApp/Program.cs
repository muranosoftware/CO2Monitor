using System;
using System.Linq;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using Microsoft.Extensions.DependencyInjection;

using CO2Monitor.Core.Interfaces;

using CO2Monitor.Core.Entities;
using CO2Monitor.Infrastructure.Services;
using CO2Monitor.Infrastructure.Devices;
using System.Runtime.Serialization;

namespace ConsoleApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            var di = new DeviceInfo(new[] { new DeviceStateFieldDeclaration("foo", new ValueDeclaration(ValueTypes.Float)) },
                                    new[] { new DeviceActionDeclaration("turn", new ValueDeclaration(ValueTypes.Void)) },
                                    Array.Empty<DeviceEventDeclaration>());

            var f = JsonConvert.SerializeObject(di);
        }
    }
}
