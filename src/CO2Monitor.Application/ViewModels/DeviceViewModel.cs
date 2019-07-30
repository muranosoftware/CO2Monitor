namespace CO2Monitor.Application.ViewModels {
	public class DeviceViewModel {
		public DeviceViewModel(int id, 
		                       string name,
		                       string type,
		                       DeviceInfoViewModel info,
		                       bool isRemote,
		                       bool isExtendable,
		                       string state) {
			Id = id;
			Name = name;
			Type = type;
			Info = info;
			IsRemote = isRemote;
			IsExtendable = isExtendable;
			State = state;
		}

		protected DeviceViewModel() { } // for AutoMapper

		public int Id { get; private set; }

		public string Name { get; private set; }

		public string Type { get; private set; }

		public DeviceInfoViewModel Info { get; private set; }

		public bool IsRemote { get; private set; }

		public bool IsExtendable { get; private set; }

		public string State { get; private set; }
	}
}
