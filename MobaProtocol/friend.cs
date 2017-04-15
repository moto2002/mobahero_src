using ProtoBuf;
using System;

namespace MobaProtocol
{
	[ProtoContract]
	public class friend
	{
		[ProtoMember(1)]
		public long id
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public long friendId
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public sbyte status
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public long summId
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public DateTime time
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public long applyId
		{
			get;
			set;
		}
	}
}
