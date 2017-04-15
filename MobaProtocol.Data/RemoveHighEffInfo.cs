using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class RemoveHighEffInfo
	{
		[ProtoMember(2)]
		public short _highEffId;

		[ProtoMember(1)]
		public short _unitId
		{
			get;
			set;
		}

		public int unitId
		{
			get
			{
				return (int)this._unitId;
			}
			set
			{
				this._unitId = (short)value;
			}
		}

		public string highEffId
		{
			get
			{
				return Name.getString((int)this._highEffId);
			}
			set
			{
				this._highEffId = (short)Name.getId(value);
			}
		}
	}
}
