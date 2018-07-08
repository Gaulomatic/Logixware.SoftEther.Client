using System;

namespace Logixware.SoftEther.Client.Daemon
{
	// ReSharper disable once InconsistentNaming
	public class IPv6Route
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