using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class ArenaTeamInfo
	{
		[ProtoMember(1)]
		public long HeroId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public string ModelId
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public long Exp
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public int Star
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public int Grade
		{
			get;
			set;
		}
	}
}
