using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class Position
	{
		[ProtoMember(1)]
		public float x
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public float y
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public float z
		{
			get;
			set;
		}
	}
}
