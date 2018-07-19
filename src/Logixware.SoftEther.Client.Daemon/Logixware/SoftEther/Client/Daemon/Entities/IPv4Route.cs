using System;

namespace Logixware.SoftEther.Client.Daemon.Entities
{
	// ReSharper disable once InconsistentNaming
	public class IPv4Route
	{
		public String Network { get; set; }
		public Int32 Prefix { get; set; }
		public String Gateway { get; set; }

		public override String ToString()
		{
			return $"{this.Network}/{this.Prefix}";
		}
	}
}