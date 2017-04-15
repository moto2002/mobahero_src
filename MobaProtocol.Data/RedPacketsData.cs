using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class RedPacketsData
	{
		[ProtoMember(1)]
		public int id;

		[ProtoMember(2)]
		public int stage;

		[ProtoMember(3)]
		public int type;

		[ProtoMember(4)]
		public DateTime start_time;

		[ProtoMember(5)]
		public DateTime end_time;

		[ProtoMember(6)]
		public TimeFormat start_time2;

		[ProtoMember(7)]
		public TimeFormat end_time2;

		[ProtoMember(8)]
		public int countdown;

		[ProtoMember(9)]
		public int timeleft;

		public string drop_child;

		public void init()
		{
			this.id = 0;
			this.stage = 0;
			this.type = 0;
			this.start_time = new DateTime(2000, 1, 1);
			this.end_time = new DateTime(2000, 1, 1);
			this.start_time2 = new TimeFormat();
			this.end_time2 = new TimeFormat();
			this.countdown = 0;
			this.timeleft = 0;
			this.drop_child = "";
		}
	}
}
