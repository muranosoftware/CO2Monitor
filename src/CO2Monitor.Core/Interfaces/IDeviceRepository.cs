using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text;

namespace CO2Monitor.Core.Interfaces
{
    public interface IDeviceRepository : INotifyCollectionChanged
    {
        IEnumerable<T> List<T>(Predicate<T> condition = null) where T : class, IDevice;

        T GetById<T>(int id) where T : class, IDevice;

        T Add<T>(T device) where T : class, IDevice;

        bool Delete<T>(Predicate<T> condition) where T : class, IDevice;

        void Update<T>(T device) where T : class, IDevice;
    }
}
