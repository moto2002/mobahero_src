using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class EquipmentData
	{
		[ProtoMember(1)]
		public int EquipmentId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int Power
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public int Intelligence
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public int Agile
		{
			get;
			set;
		}
	}
}
