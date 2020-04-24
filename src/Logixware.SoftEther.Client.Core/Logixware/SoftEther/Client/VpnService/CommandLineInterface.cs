using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

using Logixware.SoftEther.Client.Shell;

namespace Logixware.SoftEther.Client.VpnService
{
	public class CommandLineInterface : ICommandLineInterface
	{
		private readonly ILogger<CommandLineInterface> _Logger;
		private readonly CommandLineInterfaceConfiguration _Configuration;
		private readonly IShell _Shell;

		public CommandLineInterface
		(
			ILogger<CommandLineInterface> logger,
			IConfiguration configuration,
			IShell shell
		)
		{
			if (configuration == null) throw new ArgumentNullException(nameof(configuration));

			this._Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			this._Shell = shell ?? throw new ArgumentNullException(nameof(shell));

			this._Configuration = configuration.GetSection("VPN:CommandLineInterface")?.Get<CommandLineInterfaceConfiguration>();

			if (this._Configuration == null)
			{
				throw new InvalidOperationException("No command line interface configuration found.");
			}

			if (String.IsNullOrEmpty(this._Configuration.PathToClient))
			{
				throw new InvalidOperationException("No path to the VPN client service found.");
			}

			if (String.IsNullOrEmpty(this._Configuration.PathToCli))
			{
				throw new InvalidOperationException("No path to the VPN client command line interface found.");
			}
		}

		public IEnumerable<Account> GetAccounts()
		{
			foreach (var __Item in this.GetList("AccountList", "VPN Connection Setting Name").Keys)
			{
				yield return this.GetAccount(__Item);
			}
		}

		public Account GetAccount(String name)
		{
			return new Account(this, this.GetSingle($"AccountGet \"{name}\""));
		}

		public AccountStatus GetAccountStatus(String name)
		{
			return new AccountStatus(this, this.GetSingle($"AccountStatusGet \"{name}\""));
		}

		public IEnumerable<Device> GetDevices()
		{
			foreach (var __Item in this.GetList("NicList", "Virtual Network Adapter Name").Values)
			{
				yield return new Device(this, __Item);
			}
		}

		public Device GetDevice(String name)
		{
			return this.GetDevices().SingleOrDefault(i => String.Equals(i.Name, name));
		}

		private Dictionary<String, String> GetSingle(String command)
		{
			return CommandLineInterface.ParseSingleResponse(this.ExecuteCommand(command));
		}

		private Dictionary<String, Dictionary<String, String>> GetList(String command, String key)
		{
			return CommandLineInterface.ParseListResponse(this.ExecuteCommand(command), key);
		}

		public void RestartClient()
		{
			this.StopClient();
			Thread.Sleep(5000);
			this.StartClient();
		}

		public void StartClient()
		{
			var __StartCommand = $"\"{this._Configuration.PathToClient}\" start";
			var __StartExecution = this._Shell.ExecuteCommand(__StartCommand);

			if (__StartExecution.Succeeded)
			{
				var __Message = $"Successfully started the VPN client: {__StartExecution.Result}";
				this._Logger?.Inform(__Message);
			}
			else
			{
				var __Message = $"Error starting the VPN client: {__StartExecution.Result}";
				this._Logger?.Error(__Message);
			}
		}

		public void StopClient()
		{
			var __StopCommand = $"\"{this._Configuration.PathToClient}\" stop";
			var __StopExecution = this._Shell.ExecuteCommand(__StopCommand);

			if (__StopExecution.Succeeded)
			{
				var __Message = $"Successfully stopped the VPN client: {__StopExecution.Result}";
				this._Logger?.Inform(__Message);
			}
			else
			{
				var __Message = $"Error stopping the VPN client: {__StopExecution.Result}";
				this._Logger?.Error(__Message);
			}
		}

		private String ExecuteCommand(String command)
		{
			var __Command = $"\"{this._Configuration.PathToCli}\" localhost /CLIENT /PASSWORD=\"{this._Configuration.CliPassword}\" /CMD {command}";
			var __Execution = this._Shell.ExecuteCommand(__Command, false);

			if (__Execution.Succeeded)
			{
				if (__Execution.Result.Contains("Access has been denied"))
				{
					throw new AccessViolationException("Access to the vpn client has been denied. Password wrong?");
				}

				return __Execution.Result;
			}

			throw new InvalidOperationException(__Execution.Result);
		}

		private static Dictionary<String, String> ParseSingleResponse(String response)
		{
			var __Settings = new Dictionary<String, String>();

			foreach (var __Line in response.Split("\n".ToCharArray()))
			{
				var __Setting = __Line.Split("|".ToCharArray());

				if (__Setting.Length == 2 && !(String.Equals(__Setting[0], "Item") & String.Equals(__Setting[0], "Value")))
				{
					var __Key = __Setting[0].Trim();
					var __Value = __Setting[1].Trim();

					if (!(String.Equals(__Key, "Item", StringComparison.OrdinalIgnoreCase) & String.Equals(__Value, "Value", StringComparison.OrdinalIgnoreCase)))
					{
						__Settings.Add(__Key, __Value);
					}
				}
			}

			return __Settings;
		}

		private static Dictionary<String, Dictionary<String, String>> ParseListResponse(String response, String key)
		{
			var __Items = new Dictionary<String, Dictionary<String, String>>();

			String __CurrentItem = null;

			foreach (var __Line in response.Split("\n".ToCharArray()))
			{
				var __Setting = __Line.Split("|".ToCharArray());

				if (__Setting.Length == 2 && !(String.Equals(__Setting[0], "Item") & String.Equals(__Setting[0], "Value")))
				{
					var __Key = __Setting[0].Trim();
					var __Value = __Setting[1].Trim();

					if (String.Equals(__Key, key))
					{
						__Items.Add(__Value, new Dictionary<String, String>());
						__CurrentItem = __Value;
					}

					if (!(String.Equals(__Key, "Item", StringComparison.OrdinalIgnoreCase) & String.Equals(__Value, "Value", StringComparison.OrdinalIgnoreCase)))
					{
						__Items[__CurrentItem].Add(__Key, __Value);
					}
				}
			}

			return __Items;
		}
	}
}
