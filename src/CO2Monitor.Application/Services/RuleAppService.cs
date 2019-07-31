using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using CO2Monitor.Domain.Entities;
using CO2Monitor.Domain.Interfaces.Services;
using CO2Monitor.Application.ViewModels;
using CO2Monitor.Application.Interfaces;

namespace CO2Monitor.Application.Services {
	public class RuleAppService: IRuleAppSevice {
		IActionRuleRepository _repo;
		IMapper _mapper;

		public RuleAppService(IActionRuleRepository repo, IMapper mapper) {
			_repo = repo;
			_mapper = mapper;
		}

		public RuleViewModel Create(RuleViewModel ruleViewModel) {
			ActionRule rule = _repo.Add(_mapper.Map<RuleViewModel, ActionRule>(ruleViewModel));
			return _mapper.Map<ActionRule, RuleViewModel>(rule);
		}

		public bool Delete(RuleViewModel ruleViewModel) => _repo.Delete(x => x.Id == ruleViewModel.Id);

		public IEnumerable<RuleViewModel> List(Expression<Func<RuleViewModel, bool>> predicate = null) {
			return _repo.List(predicate != null ? _mapper.MapExpression<Expression<Func<RuleViewModel, bool>>,
		                                                                Expression<Func<ActionRule, bool>>>(predicate) : null)
			            .Select(x => _mapper.Map<ActionRule, RuleViewModel>(x));
		}

		public bool Update(RuleViewModel ruleViewModel) => _repo.Update(_mapper.Map<RuleViewModel, ActionRule>(ruleViewModel));
	}
}
