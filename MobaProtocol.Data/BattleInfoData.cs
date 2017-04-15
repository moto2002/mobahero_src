using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class BattleInfoData
	{
		[ProtoMember(1)]
		public string battleid
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int wincount
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public int losecount
		{
			get;
			set;
		}
	}
}
