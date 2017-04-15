using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class DrawCardCountData
	{
		[ProtoMember(1)]
		public long userId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public byte itemType
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public int itemId
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public int itemCount
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public int prob
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public byte cardIndex
		{
			get;
			set;
		}

		[ProtoMember(7)]
		public bool isDrawed
		{
			get;
			set;
		}

		[ProtoMember(8)]
		public int level
		{
			get;
			set;
		}
	}
}
