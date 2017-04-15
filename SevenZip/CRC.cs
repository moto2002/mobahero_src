using System;

namespace SevenZip
{
	internal class CRC
	{
		public static readonly uint[] Table;

		private uint _value = 4294967295u;

		static CRC()
		{
			CRC.Table = new uint[256];
			for (uint num = 0u; num < 256u; num += 1u)
			{
				uint num2 = num;
				for (int i = 0; i < 8; i++)
				{
					if ((num2 & 1u) != 0u)
					{
						num2 = (num2 >> 1 ^ 3988292384u);
					}
					else
					{
						num2 >>= 1;
					}
				}
				CRC.Table[(int)((UIntPtr)num)] = num2;
			}
		}

		public void Init()
		{
			this._value = 4294967295u;
		}

		public void UpdateByte(byte b)
		{
			this._value = (CRC.Table[(int)((byte)this._value ^ b)] ^ this._value >> 8);
		}

		public void Update(byte[] data, uint offset, uint size)
		{
			for (uint num = 0u; num < size; num += 1u)
			{
				this._value = (CRC.Table[(int)((byte)this._value ^ data[(int)((UIntPtr)(offset + num))])] ^ this._value >> 8);
			}
		}

		public uint GetDigest()
		{
			return this._value ^ 4294967295u;
		}

		private static uint CalculateDigest(byte[] data, uint offset, uint size)
		{
			CRC cRC = new CRC();
			cRC.Update(data, offset, size);
			return cRC.GetDigest();
		}

		private static bool VerifyDigest(uint digest, byte[] data, uint offset, uint size)
		{
			return CRC.CalculateDigest(data, offset, size) == digest;
		}
	}
}
