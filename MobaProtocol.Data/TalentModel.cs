using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class TalentModel
	{
		[ProtoMember(1)]
		public int TanlentId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int Count
		{
			get;
			set;
		}
	}
}
