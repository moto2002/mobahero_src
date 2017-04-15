using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class UseEquip
	{
		[ProtoMember(1)]
		public string UserId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public string HeroId
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public int Postion
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public string EquipId
		{
			get;
			set;
		}
	}
}
