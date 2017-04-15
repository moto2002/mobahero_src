using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class RetaMsg
	{
		[ProtoMember(1)]
		public byte retaCode
		{
			get;
			set;
		}
	}
}
