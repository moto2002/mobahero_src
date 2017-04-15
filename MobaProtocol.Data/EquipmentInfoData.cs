using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class EquipmentInfoData
	{
		[ProtoMember(1)]
		public long EquipmentId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int ModelId
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public int Level
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public int Grade
		{
			get;
			set;
		}

		[ProtoMember(5)]
		public int Count
		{
			get;
			set;
		}

		[ProtoMember(6)]
		public int isweared
		{
			get;
			set;
		}
	}
}
