using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class MagicBottleData
	{
		[ProtoMember(1)]
		public long magicbottleid
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int curlevel
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public long curexp
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public int drawaward
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public string userid
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public int generalbottle
		{
			get;
			set;
		}

		[ProtoMember(7)]
		public int classicbottle
		{
			get;
			set;
		}

		[ProtoMember(8)]
		public int expbottlecount
		{
			get;
			set;
		}

		public DateTime timerecord
		{
			get;
			set;
		}

		public long todayexp
		{
			get;
			set;
		}

		public string nickname
		{
			get;
			set;
		}

		public int icon
		{
			get;
			set;
		}

		public int level
		{
			get;
			set;
		}
	}
}
