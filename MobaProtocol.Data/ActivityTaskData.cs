using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class ActivityTaskData
	{
		[ProtoMember(1)]
		public int taskid
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int taskstate
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public int value
		{
			get;
			set;
		}
	}
}
