using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class NotifyTeamTarget
	{
		[ProtoMember(1)]
		public int senderId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public byte signalType
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public int targetId
		{
			get;
			set;
		}
	}
}
