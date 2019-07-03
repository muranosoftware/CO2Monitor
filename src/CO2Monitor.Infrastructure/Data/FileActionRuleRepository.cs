using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using CO2Monitor.Core.Entities;
using CO2Monitor.Core.Interfaces;
using CO2Monitor.Infrastructure.Helpers;

namespace CO2Monitor.Infrastructure.Data {
	public class FileActionRuleRepository : IActionRuleRepository {
		class ActionRuleData {
			public int RuleIdSeq { get; set; }

			public IDictionary<int, ActionRule> Rules { get; set; } = new ConcurrentDictionary<int, ActionRule>();

			[MethodImpl(MethodImplOptions.Synchronized)]
			public int GetNextId() {
				return ++RuleIdSeq;
			}
		}

		private const string ConfigurationFile = "Rules.json";
		private readonly JsonSerializerSettings _jsonSettings;
		readonly ActionRuleData _data;

		public FileActionRuleRepository() {
			_jsonSettings = new JsonSerializerSettings {
				TypeNameHandling = TypeNameHandling.Auto,
				SerializationBinder = new TypeBinder(),
			};

			_data = File.Exists(ConfigurationFile) ? JsonConvert.DeserializeObject<ActionRuleData>(File.ReadAllText(ConfigurationFile), _jsonSettings) : new ActionRuleData();
		}

		public ActionRule Add(ActionRule rule) {
			rule.Id = _data.GetNextId();

			_data.Rules.Add(rule.Id, rule);

			Save();

			return rule;
		}

		public bool Delete(Predicate<ActionRule> predicate) {
			var found = false;
			List<ActionRule> rules = _data.Rules.Values.Where((x) => predicate(x)).ToList();
			if (rules.Count > 0)
				found = true;

			foreach (ActionRule d in rules) {
				//_logger.LogInformation(
				_data.Rules.Remove(d.Id);
			}

			Save();

			return found;
		}

		public IEnumerable<ActionRule> List(Predicate<ActionRule> predicate) {
			if (predicate != null)
				return _data.Rules.Values.Where(predicate.Invoke);
			else
				return _data.Rules.Values;
		}
	
		public void Update(ActionRule rule) {
			Save();
		}

		private void Save() {
			var json = JsonConvert.SerializeObject(_data, _jsonSettings);
			File.WriteAllText(ConfigurationFile, json);
		}
	}
}
