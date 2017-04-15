using ProtoBuf;
using System;
using System.Collections.Generic;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class KillTitanSnapshotData
	{
		[ProtoMember(1)]
		public long id
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public string userid
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public long timestamp
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public List<SnapshotInfoData> heroInfoList
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public List<SnapshotInfoData> bossInfoList
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public List<AwardInfoData> awardInfoList
		{
			get;
			set;
		}

		public DateTime dt
		{
			get
			{
				return new DateTime(this.timestamp);
			}
		}
	}
}
