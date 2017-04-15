using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Pathfinding
{
	public class AstarProfiler
	{
		public class ProfilePoint
		{
			public Stopwatch watch = new Stopwatch();

			public int totalCalls;

			public long tmpBytes;

			public long totalBytes;
		}

		private static Dictionary<string, AstarProfiler.ProfilePoint> profiles = new Dictionary<string, AstarProfiler.ProfilePoint>();

		private static DateTime startTime = DateTime.UtcNow;

		public static AstarProfiler.ProfilePoint[] fastProfiles;

		public static string[] fastProfileNames;

		private AstarProfiler()
		{
		}

		[Conditional("ProfileAstar")]
		public static void InitializeFastProfile(string[] profileNames)
		{
		}

		[Conditional("ProfileAstar")]
		public static void StartFastProfile(int tag)
		{
		}

		[Conditional("ProfileAstar")]
		public static void EndFastProfile(int tag)
		{
		}

		[Conditional("UNITY_PRO_PROFILER")]
		public static void EndProfile()
		{
		}

		[Conditional("ProfileAstar")]
		public static void StartProfile(string tag)
		{
		}

		[Conditional("ProfileAstar")]
		public static void EndProfile(string tag)
		{
		}

		[Conditional("ProfileAstar")]
		public static void Reset()
		{
		}

		[Conditional("ProfileAstar")]
		public static void PrintFastResults()
		{
		}

		[Conditional("ProfileAstar")]
		public static void PrintResults()
		{
		}
	}
}
