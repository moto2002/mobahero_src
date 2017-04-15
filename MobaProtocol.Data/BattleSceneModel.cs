using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class BattleSceneModel
	{
		[ProtoMember(1)]
		public long SceneId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public byte Star
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public int DayCount
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public int DayRestCount
		{
			get;
			set;
		}
	}
}
