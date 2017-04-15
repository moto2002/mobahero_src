using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class PveLoginRetaMsg
	{
		[ProtoMember(1)]
		public byte retaCode
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public byte[] data
		{
			get;
			set;
		}
	}
}
