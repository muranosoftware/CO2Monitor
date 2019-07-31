using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using CO2Monitor.Domain.Entities;

namespace CO2Monitor.Domain.Interfaces.Services {
	public interface IActionRuleRepository {
		IEnumerable<ActionRule> List(Expression<Func<ActionRule, bool>> predicate = null);

		ActionRule Add(ActionRule rule);

		bool Delete(Expression<Func<ActionRule, bool>> predicate);

		bool Update(ActionRule rule);
	}
}
