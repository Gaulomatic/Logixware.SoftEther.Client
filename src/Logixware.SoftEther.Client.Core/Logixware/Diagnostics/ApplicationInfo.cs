﻿using System;
using System.Diagnostics;
using System.Reflection;

namespace Logixware.Diagnostics
{
	public class ApplicationInfo
	{
		public static ApplicationInfo Current()
		{
			return new ApplicationInfo(Assembly.GetEntryAssembly());
		}

		public ApplicationInfo(Assembly assembly)
		{
			if (assembly == null) throw new ArgumentNullException(nameof(assembly));

			var __FileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
			var __InfoVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
			var __AssemblyVersion = assembly.GetName().Version.ToString();

			this.Version = $"{__AssemblyVersion} ({__FileVersionInfo.FileVersion}) - {__InfoVersion}";
			this.Copyright = __FileVersionInfo.LegalCopyright;
			this.ProductName = __FileVersionInfo.FileDescription;
		}

		public String Version { get; }
		public String Copyright { get; }
		public String ProductName { get; }

		public String GetConsoleWelcomeMessage()
		{
			return $"{this.ProductName}, Version {this.Version}\n{this.Copyright}";
		}

		public ApplicationInfo WriteToConsole()
		{
			Console.WriteLine(this.GetConsoleWelcomeMessage());
			return this;
		}

		public ApplicationInfo WritePidToConsole()
		{
			Console.WriteLine(String.Empty);
			Console.WriteLine($"Staring with PID {Process.GetCurrentProcess().Id}...");
			Console.WriteLine(String.Empty);

			return this;
		}

		public ApplicationInfo SetConsoleTitle()
		{
			Console.Title = this.ProductName;
			return this;
		}
	}
}
