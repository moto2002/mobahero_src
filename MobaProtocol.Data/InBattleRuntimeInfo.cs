using ProtoBuf;
using System;
using System.Collections.Generic;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class InBattleRuntimeInfo
	{
		[ProtoMember(1)]
		public GroupTeamInfo[] teamInfos
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public UnitRuntimeInfo[] unitInfos
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public Dictionary<int, PlayerCounter> unitCounters
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public Dictionary<string, string> sceneValues
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
		public byte roomState
		{
			get;
			set;
		}

		[ProtoMember(7)]
		public short frameTime
		{
			get;
			set;
		}

		[ProtoMember(8)]
		public ActiveSurrenderInfo[] surrenderInfos
		{
			get;
			set;
		}

		[ProtoMember(9)]
		public string luaTable
		{
			get;
			set;
		}
	}
}
