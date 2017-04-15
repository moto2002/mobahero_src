using ProtoBuf;
using System;
using System.Collections.Generic;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class ClientData
	{
		[ProtoMember(1)]
		public int DeviceType
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public string AppVersion
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public string AppResVersion
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public string AppUpgradeUrl
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public List<ResourceData> ResUpgradeList
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public string BindateMD5
		{
			get;
			set;
		}

		[ProtoMember(7)]
		public string BindateVer
		{
			get;
			set;
		}

		[ProtoMember(8)]
		public bool IsInWhiteList
		{
			get;
			set;
		}

		[ProtoMember(9)]
		public int bindataSize
		{
			get;
			set;
		}

		[ProtoMember(10)]
		public string updateContentUrl
		{
			get;
			set;
		}
	}
}
