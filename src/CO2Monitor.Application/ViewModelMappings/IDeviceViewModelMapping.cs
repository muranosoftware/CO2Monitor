using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using CO2Monitor.Application.ViewModels;

namespace CO2Monitor.Application.ViewModelMappings {
	public interface IDeviceViewModelMapping<TDeviceViewModel> : IDeviceViewModelMappingBase 
		where TDeviceViewModel : DeviceViewModel {
		IEnumerable<TDeviceViewModel> List(Expression<Func<TDeviceViewModel, bool>> predicate = null);

		TDeviceViewModel Create(TDeviceViewModel vm);

		TDeviceViewModel Update(TDeviceViewModel vm);

		bool Delete(TDeviceViewModel vm);
	}
}
