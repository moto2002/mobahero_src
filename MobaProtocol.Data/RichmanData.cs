using ProtoBuf;
using System;
using System.Collections.Generic;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class RichmanData
	{
		[ProtoMember(1)]
		public List<int> idlist
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public DropItemData data
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public int count
		{
			get;
			set;
		}
	}
}
