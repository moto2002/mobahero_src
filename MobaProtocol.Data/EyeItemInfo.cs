using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class EyeItemInfo
	{
		[ProtoMember(1)]
		public float originalPosX;

		[ProtoMember(2)]
		public float originalPosY;

		[ProtoMember(3)]
		public float originalPosZ;

		[ProtoMember(4)]
		public float liveTime;
	}
}
