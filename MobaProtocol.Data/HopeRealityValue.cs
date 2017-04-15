using ProtoBuf;
using System;
using System.Collections.Generic;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class HopeRealityValue
	{
		[ProtoMember(1)]
		public int effectScoreType
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public List<double> hopeValue
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public List<double> realityValue
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public double scoreValue
		{
			get;
			set;
		}
	}
}
