using AutoMapper;
using CO2Monitor.Application.ViewModels;
using CO2Monitor.Domain.Entities;

namespace CO2Monitor.Application.AutoMapper {
	public class ActionRuleToRuleViewModelProfile : Profile {
		public ActionRuleToRuleViewModelProfile() {
			CreateMap<ActionCondition, RuleConditionViewModel>();
			CreateMap<RuleConditionViewModel, ActionCondition>();

			CreateMap<ActionRule, RuleViewModel>();
			CreateMap<RuleViewModel, ActionRule>();
		}
	}
}
