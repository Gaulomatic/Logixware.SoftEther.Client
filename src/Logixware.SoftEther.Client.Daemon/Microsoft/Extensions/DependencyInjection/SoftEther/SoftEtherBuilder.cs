using System;

namespace Microsoft.Extensions.DependencyInjection.Mail
{
	public interface ISoftEtherBuilder
	{
		IServiceCollection Services { get; }
	}

	public class SoftEtherBuilder : ISoftEtherBuilder
	{
		public SoftEtherBuilder(IServiceCollection services)
		{
			this.Services = services ?? throw new ArgumentNullException(nameof(services));
		}

		public IServiceCollection Services { get; }
	}
}
