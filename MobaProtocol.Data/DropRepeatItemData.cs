using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class DropRepeatItemData
	{
		[ProtoMember(1)]
		public int itemType
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int itemId
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public int itemCount
		{
			get;
			set;
		}
	}
}
