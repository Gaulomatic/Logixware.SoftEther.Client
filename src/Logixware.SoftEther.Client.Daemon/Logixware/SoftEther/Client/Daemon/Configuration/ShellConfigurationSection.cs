using System;

namespace Logixware.SoftEther.Client.Daemon.Configuration
{
	public class ShellConfigurationSection
	{
		public String Type { get; set; }
		public String Path { get; set; }
		public Int32? Timeout { get; set; } = 5000;
	}
}
