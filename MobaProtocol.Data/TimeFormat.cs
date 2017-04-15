using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class TimeFormat
	{
		[ProtoMember(1)]
		public int hour;

		[ProtoMember(2)]
		public int minute;

		[ProtoMember(3)]
		public int second;
	}
}
