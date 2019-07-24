using System;
using System.Threading.Tasks;

namespace CO2Monitor.Core.Interfaces.Services {
	public interface IWorkDayCalendarService {
		Task<bool> IsWorkDay(DateTime date);
	}
}
