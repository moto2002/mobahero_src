using ProtoBuf;
using System;
using System.Collections.Generic;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class KillResult
	{
		[ProtoMember(1)]
		public int attackerId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public string attckerTypeID
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public int targetId
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public List<int> helperList
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public byte killType
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public short mutiKill
		{
			get;
			set;
		}

		[ProtoMember(7)]
		public short nodeadKillCount
		{
			get;
			set;
		}

		[ProtoMember(8)]
		public byte killGroup
		{
			get;
			set;
		}
	}
}
