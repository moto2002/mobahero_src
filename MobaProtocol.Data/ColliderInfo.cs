using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class ColliderInfo
	{
		[ProtoMember(1)]
		public byte code
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public string name
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public float x
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public float z
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public float radius
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public float width
		{
			get;
			set;
		}

		[ProtoMember(7)]
		public float lenght
		{
			get;
			set;
		}

		[ProtoMember(8)]
		public float rotation
		{
			get;
			set;
		}

		[ProtoMember(9)]
		public int type
		{
			get;
			set;
		}

		[ProtoMember(10)]
		public int pivot
		{
			get;
			set;
		}
	}
}
