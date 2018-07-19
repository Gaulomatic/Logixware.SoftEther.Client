using System;

using Logixware.SoftEther.Client.Daemon.Services;

namespace Logixware.SoftEther.Client.Daemon
{
	public struct ReachableResult
	{
		private readonly VirtualNetworkService _Network;
		private readonly ConfigurationState _Configuration;
		private readonly ConnectionState _Connection;
		private readonly ReachableState _Reachable;

		public ReachableResult
		(
			VirtualNetworkService network,
			ConfigurationState configuration,
			ConnectionState connection,
			ReachableState reachable
		)
		{
			this._Network = network;
			this._Configuration = configuration;
			this._Connection = connection;
			this._Reachable = reachable;
		}

		public VirtualNetworkService Network => this._Network;
		public ConfigurationState Configuration => this._Configuration;
		public ConnectionState Connection => this._Connection;
		public ReachableState Reachable => this._Reachable;

		public override String ToString()
		{
			return $"{this._Network.Configuration.Name}, {this._Configuration}, {this._Connection}, {this._Reachable}";
		}
	}
}