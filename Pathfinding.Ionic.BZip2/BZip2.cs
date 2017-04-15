using System;

namespace Pathfinding.Ionic.BZip2
{
	internal static class BZip2
	{
		public static readonly int BlockSizeMultiple = 100000;

		public static readonly int MinBlockSize = 1;

		public static readonly int MaxBlockSize = 9;

		public static readonly int MaxAlphaSize = 258;

		public static readonly int MaxCodeLength = 23;

		public static readonly char RUNA = '\0';

		public static readonly char RUNB = '\u0001';

		public static readonly int NGroups = 6;

		public static readonly int G_SIZE = 50;

		public static readonly int N_ITERS = 4;

		public static readonly int MaxSelectors = 2 + 900000 / BZip2.G_SIZE;

		public static readonly int NUM_OVERSHOOT_BYTES = 20;

		internal static readonly int QSORT_STACK_SIZE = 1000;

		internal static T[][] InitRectangularArray<T>(int d1, int d2)
		{
			T[][] array = new T[d1][];
			for (int i = 0; i < d1; i++)
			{
				array[i] = new T[d2];
			}
			return array;
		}
	}
}
