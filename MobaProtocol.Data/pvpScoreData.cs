using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class pvpScoreData
	{
		[ProtoMember(1)]
		public int effectScoreType
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public double newHopeValue
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public double newRealityValue
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public double newScoreValue
		{
			get;
			set;
		}
	}
}
