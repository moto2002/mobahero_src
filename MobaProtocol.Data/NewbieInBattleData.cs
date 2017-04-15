using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class NewbieInBattleData
	{
		[ProtoMember(1)]
		public byte msgType
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public byte[] msgBody
		{
			get;
			set;
		}
	}
}
