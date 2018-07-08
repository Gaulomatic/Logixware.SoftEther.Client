using System;

namespace Logixware.SoftEther.Client.Daemon
{
	public class ConnectionVerificationResult
	{
		public ConnectionVerificationResult(ReachableState reachable, Object details)
		{
			this.Reachable = reachable;
			this.Details = details;
		}

		public ReachableState Reachable { get; }
		public Object Details { get; }

		public override Boolean Equals(Object obj)
		{
			if (obj == null)
			{
				return false;
			}

			if (!(obj is ConnectionVerificationResult __Other))
			{
				return false;
			}

			return Object.Equals(this.Details, __Other.Details) && Object.Equals(this.Details, __Other.Details);
		}

		public override Int32 GetHashCode()
		{
			return HashCode.Combine(this.Details, this.Reachable);
		}
	}
}