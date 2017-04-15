using ProtoBuf;
using System;
using System.Collections.Generic;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class PvpTeamInfo
	{
		[ProtoMember(1)]
		public Dictionary<byte, List<PvpTeamMember>> Teams
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public byte _tempdata
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public Dictionary<string, PlayerCounter> unitCounters
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public GroupTeamInfo[] teamInfos
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public long gameTime
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public double CurrLadderScore
		{
			get;
			set;
		}

		[ProtoMember(7)]
		public string sceneid
		{
			get;
			set;
		}

		[ProtoMember(8)]
		public long gamestarttime
		{
			get;
			set;
		}

		[ProtoMember(9)]
		public string mvpuserid
		{
			get;
			set;
		}

		[ProtoMember(10)]
		public bool isSelfDefine
		{
			get;
			set;
		}
	}
}
