using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class PvpServerData
	{
		[ProtoMember(1)]
		public string serverGuid
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int serverId
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public int roomCount
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public int roleCount
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public int playerCount
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public string serverStates
		{
			get;
			set;
		}

		[ProtoMember(7)]
		public int threadCrashCount
		{
			get;
			set;
		}

		[ProtoMember(8)]
		public string servername
		{
			get;
			set;
		}
	}
}
