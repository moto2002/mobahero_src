using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class StartCDInfo
	{
		[ProtoMember(1)]
		public int unitId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public string skillId
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public float CD
		{
			get;
			set;
		}
	}
}
