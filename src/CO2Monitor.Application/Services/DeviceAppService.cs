using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CO2Monitor.Core.Interfaces.Services;
using CO2Monitor.Application.Interfaces;
using CO2Monitor.Application.ViewModels;
using CO2Monitor.Application.ViewModelMappings;
using CO2Monitor.Core.Shared;

namespace CO2Monitor.Application.Services {
	public class DeviceAppService : IDeviceAppService {
		private readonly Dictionary<Type, IDeviceViewModelMappingBase> _mappings;
		private readonly IDeviceManagerService _deviceManager;
		
		public DeviceAppService(IEnumerable<IDeviceViewModelMappingBase> mappings, IDeviceManagerService deviceManager) {
			_mappings = mappings.ToDictionary(x => x.ViewModelType);
			_deviceManager = deviceManager;
		}

		public TDeviceViewModel Create<TDeviceViewModel>(TDeviceViewModel deviceViewModel) 
			where TDeviceViewModel : DeviceViewModel => 
			GetMapping<TDeviceViewModel>().Create(deviceViewModel);

		public bool Delete<TDeviceViewModel>(TDeviceViewModel deviceViewModel)
			where TDeviceViewModel : DeviceViewModel =>
			GetMapping<TDeviceViewModel>().Delete(deviceViewModel);

		public TDeviceViewModel Edit<TDeviceViewModel>(TDeviceViewModel deviceViewModel)
			where TDeviceViewModel : DeviceViewModel =>
			GetMapping<TDeviceViewModel>().Update(deviceViewModel);

		public void CreateDeviceExtension(int deviceId, DeviceExtensionViewModel extensionViewModel) {
			Type extType = _deviceManager.GetDeviceExtensionsTypes().FirstOrDefault(x => x.Name == extensionViewModel.Type);

			if (extType == null) {
				throw new CO2MonitorArgumentException($"Unknown extension type [{extensionViewModel.Type}]");
			}

			_deviceManager.CreateDeviceExtension(extType, deviceId, extensionViewModel.Parameter);
		}

		public async Task ExecuteAction(int deviceId, ActionViewModel actionViewModel, string argument) {
			await _deviceManager.ExecuteAction(deviceId, actionViewModel.Path, argument);
		}

		public IEnumerable<string> GetDeviceExtensionsTypes() => _deviceManager.GetDeviceExtensionsTypes().Select(x => x.Name);

		public IEnumerable<TDeviceViewModel> List<TDeviceViewModel>(Expression<Func<TDeviceViewModel, bool>> predicate) 
			where TDeviceViewModel : DeviceViewModel =>
			GetMapping<TDeviceViewModel>().List(predicate);

		private IDeviceViewModelMapping<TDeviceViewModel> GetMapping<TDeviceViewModel>() where TDeviceViewModel : DeviceViewModel {
			if (!_mappings.ContainsKey(typeof(TDeviceViewModel))) {
				throw new NotImplementedException($"Can not find mapping for [{typeof(TDeviceViewModel).FullName}]");
			}

			return (IDeviceViewModelMapping<TDeviceViewModel>)_mappings[typeof(TDeviceViewModel)];
		}
	}
}
