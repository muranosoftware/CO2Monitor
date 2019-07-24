using System;
using System.ComponentModel;
using CO2Monitor.Core.Entities;
using System.Threading.Tasks;

namespace CO2Monitor.Core.Interfaces.Devices {
	public delegate void DeviceEventHandler(IBaseDevice sender, DeviceEventDeclaration eventDeclaration, Variant data, int? senderId = null);

	public interface IBaseDevice : IDisposable {
		string Name { get; set; }

		DeviceInfo BaseInfo { get; }

		/// <summary>
		/// JSON
		/// </summary>
		string State { get; }

		event PropertyChangedEventHandler SettingsChanged;

		Task<Variant> GetField(DeviceStateFieldDeclaration fieldDeclaration);

		Task ExecuteAction(DeviceActionDeclaration deviceActionDeclaration, Variant value);

		event DeviceEventHandler EventRaised;
	}
}
