using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class TaskData
	{
		[ProtoMember(1)]
		public int TaskId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public string LastTime
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public int Count
		{
			get;
			set;
		}
	}
}
