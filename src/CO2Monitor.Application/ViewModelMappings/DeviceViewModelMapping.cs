using System;
using CO2Monitor.Core.Interfaces.Devices;
using CO2Monitor.Core.Interfaces.Services;
using CO2Monitor.Application.ViewModels;
using AutoMapper;

namespace CO2Monitor.Application.ViewModelMappings {
	public class DeviceViewModelMapping : DeviceViewModelMappingBase<DeviceViewModel, IDevice> {
		public DeviceViewModelMapping(IMapper mapper, IDeviceRepository repo) : base(mapper, repo) { }

		public override DeviceViewModel Create(DeviceViewModel vm) => throw new NotSupportedException();
	}
}
