using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class C2PCaption
	{
		[ProtoMember(1)]
		public string captionStr
		{
			get;
			set;
		}
	}
}
