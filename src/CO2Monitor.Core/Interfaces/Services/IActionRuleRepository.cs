using System;
using System.Collections.Generic;
using CO2Monitor.Core.Entities;

namespace CO2Monitor.Core.Interfaces.Services {
	public interface IActionRuleRepository {
		IEnumerable<ActionRule> List(Predicate<ActionRule> predicate = null);

		ActionRule Add(ActionRule rule);

		bool Delete(Predicate<ActionRule> predicate);

		void Update(ActionRule rule);
	}
}
