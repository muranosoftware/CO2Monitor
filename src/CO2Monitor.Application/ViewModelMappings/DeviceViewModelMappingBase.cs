using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CO2Monitor.Core.Shared;
using CO2Monitor.Core.Interfaces.Devices;
using CO2Monitor.Core.Interfaces.Services;
using CO2Monitor.Application.ViewModels;
using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;

namespace CO2Monitor.Application.ViewModelMappings {
	public abstract class DeviceViewModelMappingBase<TDeviceViewModel, TDevice> : IDeviceViewModelMapping<TDeviceViewModel>
		where TDeviceViewModel : DeviceViewModel
		where TDevice : class, IDevice {
		protected readonly IMapper Mapper;
		private readonly IDeviceRepository _repo;

		protected DeviceViewModelMappingBase(IMapper mapper, IDeviceRepository repo) {
			Mapper = mapper;
			_repo = repo;
		}

		public Type ViewModelType => typeof(TDeviceViewModel);

		public IEnumerable<TDeviceViewModel> List(Expression<Func<TDeviceViewModel, bool>> predicate = null) {
			Expression<Func<TDevice, bool>> exp = null;

			if (predicate != null) { 
				exp = Mapper.MapExpression<Expression<Func<TDevice, bool>>>(predicate);
			}

			return _repo.List(exp).Select(x => {
				x.ToString();
				return Mapper.Map<TDevice, TDeviceViewModel>(x);
			});
		}

		public abstract TDeviceViewModel Create(TDeviceViewModel vm);

		public bool Delete(TDeviceViewModel vm) => _repo.Delete<TDevice>(x => x.Id == vm.Id);

		public TDeviceViewModel Update(TDeviceViewModel vm) {
			TDevice device = _repo.List<TDevice>(x => x.Id == vm.Id).FirstOrDefault();

			if (device is null) {
				throw new CO2MonitorArgumentException($"Device {{ Id = {vm.Id} }} does not Exist!");
			}

			Mapper.Map(vm, device);

			return Mapper.Map<TDevice, TDeviceViewModel>(device);
		}
	}
}
