using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class KdaAbilityGraph
	{
		[ProtoMember(1)]
		public float kill
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public float output
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public float money
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public float assist
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public float defence
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public float survival
		{
			get;
			set;
		}
	}
}
