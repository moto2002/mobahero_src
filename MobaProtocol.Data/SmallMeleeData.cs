using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class SmallMeleeData
	{
		[ProtoMember(1)]
		public string UserId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public string NickName
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public int Count
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public int IconId
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public DateTime CDTime
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public int HonorValue
		{
			get;
			set;
		}

		[ProtoMember(7)]
		public string[] TeamInfo
		{
			get;
			set;
		}

		[ProtoMember(8)]
		public int Power
		{
			get;
			set;
		}
	}
}
