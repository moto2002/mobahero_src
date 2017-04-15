using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class P2CCaption
	{
		[ProtoMember(1)]
		public int id
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public string uid
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public string captionStr
		{
			get;
			set;
		}
	}
}
