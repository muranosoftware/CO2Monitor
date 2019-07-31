using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CO2Monitor.Application.ViewModels;

namespace CO2Monitor.Application.Interfaces {
	public interface IRuleAppSevice {
		IEnumerable<RuleViewModel> List(Expression<Func<RuleViewModel, bool>> predicate = null);

		RuleViewModel Create(RuleViewModel ruleViewModel);

		bool Delete(RuleViewModel ruleViewModel);

		bool Update(RuleViewModel ruleViewModel);
	}
}
