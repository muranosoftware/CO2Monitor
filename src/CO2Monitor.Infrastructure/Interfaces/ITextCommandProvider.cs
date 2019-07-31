using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace CO2Monitor.Infrastructure.Interfaces {
	public delegate void TextCommandHandler(string userId, string userName, string message, string channelId, bool isGroupChannel);

	public interface ITextCommandProvider : IHostedService {
		event TextCommandHandler NewCommand;

		Task SendTextMessage(string channelId, string message, object[] attachments = null);

		Task SendFileMessage(string channelId, Stream stream, string title = "file");
	}
}
