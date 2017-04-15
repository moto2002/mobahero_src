using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class heroRecordInfo
	{
		[ProtoMember(1)]
		public string heroid
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public bool win
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public bool mvp
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public string battleid
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public PlayerCounter playercounter
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public DateTime gametime
		{
			get;
			set;
		}

		[ProtoMember(7)]
		public bool isTeamFight
		{
			get;
			set;
		}

		[ProtoMember(8)]
		public long pvpLogId
		{
			get;
			set;
		}
	}
}
