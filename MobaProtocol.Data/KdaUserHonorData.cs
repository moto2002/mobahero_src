using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class KdaUserHonorData
	{
		public string userid
		{
			get;
			set;
		}

		[ProtoMember(1)]
		public KillType killtype
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public string picname
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public DateTime timerecord
		{
			get;
			set;
		}
	}
}
