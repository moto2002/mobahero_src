using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class ResourceData
	{
		[ProtoMember(1)]
		public string FileName
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int Version
		{
			get;
			set;
		}
	}
}
