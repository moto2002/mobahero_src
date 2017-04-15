using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class RuneChangeModel
	{
		[ProtoMember(1)]
		public int ModelId
		{
			get;
			set;
		}

		[ProtoMember(2)]
		public int Postion
		{
			get;
			set;
		}

		[ProtoMember(3)]
		public long EquipmentId
		{
			get;
			set;
		}
	}
}
