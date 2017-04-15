using Com.Game.Utils;
using System;
using System.Text;
using UnityEngine;

public class ProfilerHelper : MonoBehaviour
{
	private class Stats
	{
		public int[] collection;

		public long totalMemory;

		public long monoHeapSize;

		public long monoUsedSize;

		public long allocatedSize;

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this.collection.Length; i++)
			{
				stringBuilder.AppendFormat("collection[{0}]={1},", i, this.collection[i]);
			}
			stringBuilder.AppendFormat("totalMemory={0},", this.totalMemory);
			stringBuilder.AppendFormat("monoHeapSize={0},", this.monoHeapSize);
			stringBuilder.AppendFormat("monoUsedSize={0},", this.monoUsedSize);
			stringBuilder.AppendFormat("allocatedSize={0},", this.allocatedSize);
			return stringBuilder.ToString();
		}

		public static ProfilerHelper.Stats operator -(ProfilerHelper.Stats x, ProfilerHelper.Stats y)
		{
			ProfilerHelper.Stats stats = new ProfilerHelper.Stats
			{
				totalMemory = x.totalMemory - y.totalMemory,
				monoHeapSize = x.monoHeapSize - y.monoHeapSize,
				monoUsedSize = x.monoUsedSize - y.monoUsedSize,
				allocatedSize = x.allocatedSize - y.allocatedSize,
				collection = new int[x.collection.Length]
			};
			for (int i = 0; i < x.collection.Length; i++)
			{
				stats.collection[i] = x.collection[i] - y.collection[i];
			}
			return stats;
		}
	}

	private static ProfilerHelper.Stats _oldStats;

	private static ProfilerHelper.Stats _newStats;

	private bool _begin;

	private static ProfilerHelper.Stats Capture()
	{
		ProfilerHelper.Stats stats = new ProfilerHelper.Stats
		{
			totalMemory = GC.GetTotalMemory(false),
			monoHeapSize = (long)((ulong)Profiler.GetMonoHeapSize()),
			monoUsedSize = (long)((ulong)Profiler.GetMonoUsedSize()),
			allocatedSize = (long)((ulong)Profiler.GetTotalAllocatedMemory()),
			collection = new int[GC.MaxGeneration + 1]
		};
		for (int i = 0; i <= GC.MaxGeneration; i++)
		{
			stats.collection[i] = GC.CollectionCount(i);
		}
		return stats;
	}

	public static void Begin()
	{
		ClientLogger.Error("begin");
		ProfilerHelper._oldStats = ProfilerHelper.Capture();
	}

	public static void End()
	{
		ClientLogger.Error("end");
		ProfilerHelper._newStats = ProfilerHelper.Capture();
		ClientLogger.Error("old:" + ProfilerHelper._oldStats.ToString());
		ClientLogger.Error("new:" + ProfilerHelper._newStats.ToString());
		ClientLogger.Error("diff:" + (ProfilerHelper._newStats - ProfilerHelper._oldStats).ToString());
	}

	private void OnGUI()
	{
		if (GUILayout.Button("profiler", new GUILayoutOption[0]))
		{
			this._begin = !this._begin;
			if (this._begin)
			{
				ProfilerHelper.Begin();
			}
			else
			{
				ProfilerHelper.End();
			}
		}
	}
}
