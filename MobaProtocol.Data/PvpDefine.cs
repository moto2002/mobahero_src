using System;

namespace MobaProtocol.Data
{
	public static class PvpDefine
	{
		public const int TeamMax = 3;

		public static byte GetPlayerGroupByIndex(int i)
		{
			byte[] array = new byte[]
			{
				0,
				1,
				3
			};
			return array[i];
		}

		public static byte PlayerGroupToIndex(byte group)
		{
			byte result;
			if (group == 0)
			{
				result = 0;
			}
			else if (group == 1)
			{
				result = 1;
			}
			else
			{
				result = 2;
			}
			return result;
		}

		public static byte GetViewGroup()
		{
			return 2;
		}
	}
}
