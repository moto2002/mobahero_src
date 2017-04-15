using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class RichGiftInfo
	{
		[ProtoMember(1)]
		public int itemType
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int itemcount
		{
			get;
			set;
		}
	}
}
