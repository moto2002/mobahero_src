using System;
using System.Diagnostics;
using UnityEngine;

public class MobaProfiler
{
	[Conditional("MOBA_NGUI"), Conditional("MOBA_TESTING")]
	public static void StartProfile(string tag)
	{
	}

	[Conditional("MOBA_NGUI"), Conditional("MOBA_TESTING")]
	public static void StartProfile(string tag, UnityEngine.Object go)
	{
	}

	[Conditional("MOBA_TESTING"), Conditional("MOBA_NGUI")]
	public static void EndProfile(string tag = "")
	{
	}
}
