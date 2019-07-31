using System;
using AutoMapper;
using CO2Monitor.Domain.Interfaces.Devices;
using CO2Monitor.Domain.Interfaces.Services;
using CO2Monitor.Application.ViewModels;

namespace CO2Monitor.Application.ViewModelMappings {
	public class DeviceViewModelMapping : DeviceViewModelMappingBase<DeviceViewModel, IDevice> {
		public DeviceViewModelMapping(IMapper mapper, IDeviceRepository repo) : base(mapper, repo) { }

		public override DeviceViewModel Create(DeviceViewModel vm) => throw new NotSupportedException();
	}
}
