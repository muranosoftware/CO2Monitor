using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Configuration;
using MoreLinq;
using Newtonsoft.Json;
using CO2Monitor.Domain.Entities;
using CO2Monitor.Domain.Interfaces.Services;
using CO2Monitor.Infrastructure.Helpers;

namespace CO2Monitor.Infrastructure.Data {
	public class FileActionRuleRepository : IActionRuleRepository {
		class ActionRuleData {
			public int RuleIdSeq { get; set; }

			public IDictionary<int, ActionRule> Rules { get; set; } = new ConcurrentDictionary<int, ActionRule>();

			[MethodImpl(MethodImplOptions.Synchronized)]
			public int GetNextId() => ++RuleIdSeq;
		}

		private const string ConfigurationFileKey = "FileActionRuleRepository:File";
		private readonly string _fileName;
		private readonly JsonSerializerSettings _jsonSettings;
		readonly ActionRuleData _data;

		public FileActionRuleRepository(IConfiguration configuration) {
			_jsonSettings = new JsonSerializerSettings {
				TypeNameHandling = TypeNameHandling.Auto,
				SerializationBinder = new TypeBinder(),
			};

			_fileName = configuration.GetValue<string>(ConfigurationFileKey);

			_data = File.Exists(_fileName) ? JsonConvert.DeserializeObject<ActionRuleData>(File.ReadAllText(_fileName), _jsonSettings) : new ActionRuleData();
		}

		public ActionRule Add(ActionRule rule) {
			rule.Id = _data.GetNextId();

			_data.Rules.Add(rule.Id, rule);

			Save();

			return rule;
		}

		public bool Delete(Expression<Func<ActionRule, bool>> predicate) {
			ActionRule [] rules = _data.Rules.Values.Where(predicate.Compile()).ToArray();

			rules.ForEach( r =>	_data.Rules.Remove(r.Id));

			Save();

			return rules.Length > 0;
		}

		public IEnumerable<ActionRule> List(Expression<Func<ActionRule, bool>> predicate = null) =>
			predicate is null ? _data.Rules.Values : _data.Rules.Values.Where(predicate.Compile());

		public bool Update(ActionRule rule) {
			
			if(List(x => x.Id == rule.Id).FirstOrDefault() == null) {
				return false;
			}

			_data.Rules[rule.Id] = rule;
			Save();
			return true;
		}

		private void Save() {
			var json = JsonConvert.SerializeObject(_data, _jsonSettings);
			File.WriteAllText(_fileName, json);
		}
	}
}
