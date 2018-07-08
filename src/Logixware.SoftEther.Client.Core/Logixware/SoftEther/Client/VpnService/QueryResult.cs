using System;
using System.Collections.Generic;

namespace Logixware.SoftEther.Client.VpnService
{
	public abstract class QueryResult
	{
		private readonly ICommandLineInterface _Cli;
		private readonly Dictionary<String, String> _Data;

		protected QueryResult(ICommandLineInterface cli, Dictionary<String, String> data)
		{
			this._Cli = cli ?? throw new ArgumentNullException(nameof(cli));
			this._Data = data ?? throw new ArgumentNullException(nameof(data));
		}

		protected ICommandLineInterface Cli => this._Cli;
		protected Dictionary<String, String> Data => this._Data;

		protected String Get(String key)
		{
			return !this._Data.ContainsKey(key) ? String.Empty : this._Data[key];
		}

		public override Boolean Equals(Object obj)
		{
			if (obj == null)
			{
				return false;
			}

			if (!(obj is QueryResult __Other))
			{
				return false;
			}

			if (this._Data.Count != __Other._Data.Count)
			{
				return false;
			}

			foreach (var __Pair in this._Data)
			{
				if (__Other._Data.TryGetValue(__Pair.Key, out var __Value))
				{
					if (__Value != __Pair.Value)
					{
						return false;
					}
				}
				else
				{
					return false;
				}
			}

			return true;
		}

		public override Int32 GetHashCode()
		{
			return this._Data.GetHashCode();
		}
	}
}