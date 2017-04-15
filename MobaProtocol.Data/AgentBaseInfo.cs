using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class AgentBaseInfo
	{
		[ProtoMember(1)]
		public string NickName
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public long UserId
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public int head
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public int headFrame
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public int Level
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public int Ladder
		{
			get;
			set;
		}

		[ProtoMember(7)]
		public string TeamName
		{
			get;
			set;
		}

		[ProtoMember(8)]
		public int BotLevel
		{
			get;
			set;
		}

		[ProtoMember(9)]
		public int CharmRankvalue
		{
			get;
			set;
		}
	}
}
