using ProtoBuf;
using System;
using System.Collections.Generic;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class RuneInfoData
	{
		[ProtoMember(1)]
		public bool IsUse
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public List<RuneModel> List
		{
			get;
			set;
		}
	}
}
