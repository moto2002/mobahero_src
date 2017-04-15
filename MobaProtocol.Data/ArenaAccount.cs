using ProtoBuf;
using System;
using System.Collections.Generic;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class ArenaAccount
	{
		[ProtoMember(1)]
		public int GetArenaMoney
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public List<EquipmentInfoData> DropList
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public int GetCoin
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public int GetDiamond
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public int ReqEnergy
		{
			get;
			set;
		}
	}
}
