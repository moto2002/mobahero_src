using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class PvpStartGameInfo
	{
		[ProtoMember(1)]
		public string serverIp
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int serverPort
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public string serverName
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public int roomId
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public int newUid
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public string uid
		{
			get;
			set;
		}

		[ProtoMember(7)]
		public string newKey
		{
			get;
			set;
		}

		[ProtoMember(8)]
		public int randomcount
		{
			get;
			set;
		}

		[ProtoMember(9)]
		public byte state
		{
			get;
			set;
		}

		[ProtoMember(10)]
		public string gameserverKey
		{
			get;
			set;
		}

		[ProtoMember(11)]
		public byte group
		{
			get;
			set;
		}
	}
}
