using ProtoBuf;
using System;
using System.Collections.Generic;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class MapItemInfo
	{
		[ProtoMember(1)]
		public int unitId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public string typeId
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public string uniTypeId
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public byte group
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public string locationid
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public int controlPlayerId
		{
			get;
			set;
		}

		[ProtoMember(7)]
		public string callSkillId
		{
			get;
			set;
		}

		[ProtoMember(8)]
		public byte callSkillLev
		{
			get;
			set;
		}

		[ProtoMember(9)]
		public int callUnitId
		{
			get;
			set;
		}

		[ProtoMember(10)]
		public string hieffId
		{
			get;
			set;
		}

		[ProtoMember(11)]
		public SVector3 burnPos
		{
			get;
			set;
		}

		[ProtoMember(12)]
		public List<int> targetUnitIds
		{
			get;
			set;
		}
	}
}
