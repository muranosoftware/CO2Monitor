using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

using CO2Monitor.Core.Entities;
using CO2Monitor.Core.Interfaces;
using CO2Monitor.Infrastructure.Helpers;

namespace CO2Monitor.Infrastructure.Data
{
    public class FileActionRuleRepository : IActionRuleRepository
    {
        class ActionRuleData
        {
            public int RuleIdSeq { get; set; }

            public IDictionary<int, ActionRule> Rules { get; set; } = new ConcurrentDictionary<int, ActionRule>();

            [MethodImpl(MethodImplOptions.Synchronized)]
            public int GetNextId() { return ++RuleIdSeq; }

        }

        private const string _ConfigurationFile = "Rules.json";
        private readonly JsonSerializerSettings _jsonSettings;
        ActionRuleData _data;

        public FileActionRuleRepository(IConfiguration configuration)
        {
            _jsonSettings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                SerializationBinder = new TypeBinder(),
            };


            if (File.Exists(_ConfigurationFile))
                _data = JsonConvert.DeserializeObject<ActionRuleData>(File.ReadAllText(_ConfigurationFile), _jsonSettings);
            else
                _data = new ActionRuleData();
        }

        public ActionRule Add(ActionRule rule)
        {
            rule.Id = _data.GetNextId();

            _data.Rules.Add(rule.Id, rule);

            Save();

            return rule;
        }

        public bool Delete(Predicate<ActionRule> predicate)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ActionRule> List(Predicate<ActionRule> predicate)
        {
            if (predicate != null)
                return _data.Rules.Values.Where(x => predicate.Invoke(x));
            else
                return _data.Rules.Values;
        }
    
        public void Update(ActionRule rule)
        {
            Save();
        }


        private void Save()
        {
            var json = JsonConvert.SerializeObject(_data, _jsonSettings);
            File.WriteAllText(_ConfigurationFile, json);
        }
    }
}
