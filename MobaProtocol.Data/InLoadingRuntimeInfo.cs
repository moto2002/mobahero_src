using ProtoBuf;
using System;
using System.Collections.Generic;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class InLoadingRuntimeInfo
	{
		[ProtoMember(1)]
		public Dictionary<int, byte> loadProcessDic
		{
			get;
			set;
		}
	}
}
