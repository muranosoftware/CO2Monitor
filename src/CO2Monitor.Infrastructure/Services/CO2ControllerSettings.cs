using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;

using CO2Monitor.Core.Interfaces;

namespace CO2Monitor.Infrastructure.Services
{
    internal class CO2ControllerSettings
    {
        public const int defaultNormalLevel = 800;
        public const int defaultMidLevel = 1000;
        public const int defaultHighLevel = 1200;
        public const float defaultPollingRate = 60f;
        
        public float PollingRate { get; set; }

        public string CO2DriverAddress { get; set; }

        public string CO2FanDriverAddress { get; set; }

        public Dictionary<CO2Levels, int> Levels { get; set; }

        public CO2ControllerSettings()
        {
            Levels = new Dictionary<CO2Levels, int>()
            {
                { CO2Levels.Low, 0 },
                { CO2Levels.Normal, defaultNormalLevel },
                { CO2Levels.Mid, defaultMidLevel },
                { CO2Levels.High, defaultHighLevel },
            };

            PollingRate = defaultPollingRate;
        }

        public void Save(string path)
        {
            var json = JsonConvert.SerializeObject(this);

            File.WriteAllText(path, json);
        }

        public static CO2ControllerSettings LoadOrUseDefault(string path)
        {
            if(!File.Exists(path))
                return new CO2ControllerSettings();

            var json = File.ReadAllText(path);

            return JsonConvert.DeserializeObject<CO2ControllerSettings>(json);
        }
    }
}
