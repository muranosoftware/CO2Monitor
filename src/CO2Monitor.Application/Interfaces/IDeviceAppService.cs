using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CO2Monitor.Application.ViewModels;

namespace CO2Monitor.Application.Interfaces {
	public interface IDeviceAppService {
		IEnumerable<TDeviceViewModel> List<TDeviceViewModel>(Expression<Func<TDeviceViewModel, bool>> predicate = null) where TDeviceViewModel : DeviceViewModel;

		TDeviceViewModel Create<TDeviceViewModel>(TDeviceViewModel deviceViewModel) where TDeviceViewModel : DeviceViewModel;

		TDeviceViewModel Edit<TDeviceViewModel>(TDeviceViewModel deviceViewModel) where TDeviceViewModel : DeviceViewModel;

		bool Delete<TDeviceViewModel>(TDeviceViewModel deviceViewModel) where TDeviceViewModel : DeviceViewModel;

		void CreateDeviceExtension(int deviceId, DeviceExtensionViewModel extensionViewModel);

		IEnumerable<string> GetDeviceExtensionsTypes();

		Task ExecuteAction(int deviceId, ActionViewModel actionViewModel, string argument);
	}
}
