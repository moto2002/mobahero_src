using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class DetailAchieveData
	{
		[ProtoMember(1)]
		public int taskid
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public bool isGetAward
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public bool isComplete
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public int value
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public int achieveid
		{
			get;
			set;
		}

		public DateTime timeget
		{
			get;
			set;
		}

		public bool isrecord
		{
			get;
			set;
		}

		public bool isrank
		{
			get;
			set;
		}
	}
}
