using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class ItemDynData
	{
		[ProtoMember(1)]
		public int itemOid
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public string typeId
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public int count
		{
			get;
			set;
		}
	}
}
