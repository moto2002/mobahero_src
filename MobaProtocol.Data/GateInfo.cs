using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class GateInfo
	{
		[ProtoMember(1)]
		public string IpAddress
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int TcpPort
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public int UdpPort
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public int AreaId
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public int PeerCount
		{
			get;
			set;
		}
	}
}
