using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class SurrenderStartInfo
	{
		[ProtoMember(1)]
		public int code = 0;

		[ProtoMember(2)]
		public ActiveSurrenderInfo info;
	}
}
