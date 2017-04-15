using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class RuneModel
	{
		[ProtoMember(1)]
		public int RuneId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int Postion
		{
			get;
			set;
		}
	}
}
