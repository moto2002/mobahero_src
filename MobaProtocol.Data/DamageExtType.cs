using System;

namespace MobaProtocol.Data
{
	public static class DamageExtType
	{
		public const byte Normal = 0;

		public const byte Crit = 1;

		public const byte Miss = 2;

		public static byte GetDamageInfo(bool isCrit, bool isMiss)
		{
			byte b = 0;
			if (isCrit)
			{
				b |= 1;
			}
			if (isMiss)
			{
				b |= 2;
			}
			return b;
		}

		public static bool CheckCrit(byte val)
		{
			return (val & 1) > 0;
		}

		public static bool CheckMiss(byte val)
		{
			return (val & 2) > 0;
		}
	}
}
