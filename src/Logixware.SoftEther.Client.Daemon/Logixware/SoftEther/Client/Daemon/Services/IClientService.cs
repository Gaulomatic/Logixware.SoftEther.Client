using System.Threading;
using System.Threading.Tasks;

namespace Logixware.SoftEther.Client.Daemon.Services
{
	public interface IClientService
	{
		Task StartAsync(CancellationToken cancellationToken);
		Task StopAsync(CancellationToken cancellationToken);
	}
}