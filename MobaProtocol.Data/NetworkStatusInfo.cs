using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class NetworkStatusInfo
	{
		[ProtoMember(1)]
		public int packageLossByCrc
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int roundTripTime
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public int roundTripTimeVariance
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public int resentReliableCommands
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public int windowSize
		{
			get;
			set;
		}
	}
}
