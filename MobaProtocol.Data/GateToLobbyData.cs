using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class GateToLobbyData
	{
		[ProtoMember(1)]
		public byte code;

		[ProtoMember(2)]
		public BOPBDictionary data;

		[ProtoMember(3)]
		public byte channel;

		[ProtoMember(4)]
		public string strdata;
	}
}
