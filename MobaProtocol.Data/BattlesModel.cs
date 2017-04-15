using ProtoBuf;
using System;
using System.Collections.Generic;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class BattlesModel
	{
		[ProtoMember(1)]
		public int BattleId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public List<BattleSceneModel> List
		{
			get;
			set;
		}
	}
}
