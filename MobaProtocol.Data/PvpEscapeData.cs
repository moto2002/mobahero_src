using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	[Serializable]
	public class PvpEscapeData
	{
		[ProtoMember(1)]
		public string userId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int totalEscape
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public int totalEscapeToday
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public long cdClearAt
		{
			get;
			set;
		}
	}
}
