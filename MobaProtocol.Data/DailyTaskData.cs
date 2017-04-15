using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class DailyTaskData
	{
		public string userid
		{
			get;
			set;
		}

		[ProtoMember(1)]
		public int taskid
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int value
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public bool isGetAward
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public bool isComplete
		{
			get;
			set;
		}

		public int type
		{
			get;
			set;
		}

		public DateTime timeget
		{
			get;
			set;
		}

		public DateTime timeupdate
		{
			get;
			set;
		}
	}
}
