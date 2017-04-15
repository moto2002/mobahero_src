using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class TotalAchieveData
	{
		[ProtoMember(1)]
		public int achieveId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int achievepoint
		{
			get;
			set;
		}
	}
}
