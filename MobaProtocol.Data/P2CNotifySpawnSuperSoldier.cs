using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class P2CNotifySpawnSuperSoldier
	{
		[ProtoMember(1)]
		public byte group
		{
			get;
			set;
		}
	}
}
