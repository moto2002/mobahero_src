using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class ServerInfo
	{
		[ProtoMember(1)]
		public string serverid;

		[ProtoMember(2)]
		public string serverip;

		[ProtoMember(3)]
		public int serverport;

		[ProtoMember(4)]
		public string udpaddress;

		[ProtoMember(5)]
		public string tcpaddress;

		[ProtoMember(6)]
		public string websocketaddress;

		[ProtoMember(7)]
		public string servername;

		[ProtoMember(8)]
		public long loadlevel;

		[ProtoMember(9)]
		public long peercount;

		[ProtoMember(10)]
		public long usercount;

		[ProtoMember(11)]
		public string serverImage;

		[ProtoMember(12)]
		public byte serverState;

		[ProtoMember(13)]
		public int areaId;

		[ProtoMember(14)]
		public string appVer;

		[ProtoMember(15)]
		public string bindataVer;

		[ProtoMember(16)]
		public string bindataMD5;

		[ProtoMember(17)]
		public string bindataURL;

		[ProtoMember(18)]
		public string category;

		[ProtoMember(19)]
		public int bindataZipSize;

		[ProtoMember(20)]
		public string updateContentURL;

		[ProtoMember(21)]
		public string appUpdateURLAndroid;

		[ProtoMember(22)]
		public string appUpdateURLIOS;
	}
}
