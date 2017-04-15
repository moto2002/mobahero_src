using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class BuffRuntimeData
	{
		[ProtoMember(1)]
		public string buffId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public byte layer
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public long curTime
		{
			get;
			set;
		}
	}
}
