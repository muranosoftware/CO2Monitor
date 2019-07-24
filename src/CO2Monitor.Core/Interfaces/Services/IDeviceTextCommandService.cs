using System.Threading.Tasks;

namespace CO2Monitor.Core.Interfaces.Services {
	public class TextCommand {
		public string UserId { get; set; }
		public string UserName { get; set; }
		public string Command { get; set; }
		public string ChannelId { get; set; }
		public bool IsGroupChannel { get; set; }
	}

	public class TextCommandResult {
		public string Text { get; set; }
		public object[] Attachments { get; set; }
	}

	public interface IDeviceTextCommandService {
		Task<TextCommandResult> ExecuteCommand(TextCommand command);
	}
}
