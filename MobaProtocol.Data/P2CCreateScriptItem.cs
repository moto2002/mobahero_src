using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class P2CCreateScriptItem
	{
		[ProtoMember(1)]
		public string tid;

		[ProtoMember(2)]
		public byte group;

		[ProtoMember(3)]
		public long sleepTime;

		[ProtoMember(4)]
		public SVector3 burnPos;
	}
}
