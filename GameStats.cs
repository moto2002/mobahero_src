using MobaHeros;
using MobaProtocol;
using PathologicalGames;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;

public class GameStats : MonoBehaviour
{
	private class PackageStats
	{
		public int InPackedCount;

		public int InUnpackedCount;

		public int OutCount;

		public int InAllCount
		{
			get
			{
				return this.InPackedCount + this.InUnpackedCount;
			}
		}

		public override string ToString()
		{
			return string.Format("{0}({1} packed)/{2}", this.InAllCount, this.InPackedCount, this.OutCount);
		}
	}

	private class ActionStat
	{
		public int countCreated;

		public int countDestroyed;
	}

	private class EffectStat
	{
		public string name;

		public int poolSpawn;

		public int rawSpawn;

		public int despawn;

		public int AllSpawn
		{
			get
			{
				return this.poolSpawn + this.rawSpawn;
			}
		}
	}

	private class DespawnStat
	{
		public int despawn;
	}

	private class PoolStat
	{
		public PrefabPool pool;

		public int maxSpawned;

		public int maxDespawned;

		public int spawnCount;

		public int despawnCount;

		public string name;

		public int instantiateCount;

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("name:{0}, ", this.name).AppendFormat("maxSpawned:{0},", this.maxSpawned).AppendFormat("maxDespawned:{0},", this.maxDespawned).AppendFormat("spawnCount:{0},", this.spawnCount).AppendFormat("despawnCount:{0},", this.despawnCount).AppendFormat("instantiateCount:{0}", this.instantiateCount);
			return stringBuilder.ToString();
		}
	}

	private class PvpCodeProccessInfo
	{
		public HashSet<int> frameIds;

		public List<long> times;

		public PvpCodeProccessInfo()
		{
			this.frameIds = new HashSet<int>();
			this.times = new List<long>(1024);
		}

		public void Add(int frameId, long tick)
		{
			this.frameIds.Add(frameId);
			this.times.Add(tick);
		}

		public override string ToString()
		{
			long num = -1L;
			long num2 = 0L;
			int num3 = 0;
			foreach (long current in this.times)
			{
				if (current > num)
				{
					num = current;
				}
				num3++;
				num2 += current;
			}
			return string.Format("{0}, {1}, {2}, {3}, {4}", new object[]
			{
				this.frameIds.Count,
				this.times.Count,
				(double)num2 / 10000.0,
				(double)num / 10000.0,
				(double)num2 / 10000.0 / (double)num3
			});
		}
	}

	private class WidgetStat
	{
		public int fillNum;

		public int moveNum;
	}

	private static readonly Dictionary<int, GameStats.PackageStats> _stats = new Dictionary<int, GameStats.PackageStats>();

	private static readonly Dictionary<Type, GameStats.ActionStat> _actionStats = new Dictionary<Type, GameStats.ActionStat>();

	private static readonly Dictionary<string, GameStats.EffectStat> _effectStats = new Dictionary<string, GameStats.EffectStat>();

	private static readonly Dictionary<string, GameStats.DespawnStat> _despawnStat = new Dictionary<string, GameStats.DespawnStat>();

	private static readonly Dictionary<string, GameStats.EffectStat> _effectHandleStats = new Dictionary<string, GameStats.EffectStat>();

	private static readonly List<GameStats.PoolStat> _prefabPoolList = new List<GameStats.PoolStat>();

	private static readonly Dictionary<int, GameStats.PvpCodeProccessInfo> _pvpCodeProcessInfo = new Dictionary<int, GameStats.PvpCodeProccessInfo>();

	private static DateTime _startTime;

	private static DateTime _endTime;

	private static bool _isStarted;

	private static Dictionary<UIWidget, GameStats.WidgetStat> _widgetStats = new Dictionary<UIWidget, GameStats.WidgetStat>();

	private static bool _lastRecord = false;

	[Conditional("GAME_STAT")]
	public static void On_ReceivePvpMsg(PvpCode code, bool packed)
	{
		if (!GameStats._stats.ContainsKey((int)code))
		{
			GameStats._stats[(int)code] = new GameStats.PackageStats();
		}
		if (packed)
		{
			GameStats._stats[(int)code].InPackedCount++;
		}
		else
		{
			GameStats._stats[(int)code].InUnpackedCount++;
		}
	}

	[Conditional("GAME_STAT")]
	public static void On_ProcessPvpMsg(PvpCode code, long tick)
	{
		if (!GameStats._pvpCodeProcessInfo.ContainsKey((int)code))
		{
			GameStats._pvpCodeProcessInfo[(int)code] = new GameStats.PvpCodeProccessInfo();
		}
		GameStats._pvpCodeProcessInfo[(int)code].Add(Time.frameCount, tick);
	}

	[Conditional("GAME_STAT")]
	public static void On_SendPvpMsg(PvpCode code)
	{
		if (GameStats._stats.ContainsKey((int)code))
		{
			GameStats._stats[(int)code].OutCount++;
		}
		else
		{
			GameStats._stats[(int)code] = new GameStats.PackageStats
			{
				OutCount = 1
			};
		}
	}

	[Conditional("GAME_STAT")]
	public static void On_CreateAction(Type t)
	{
		if (!GameStats._actionStats.ContainsKey(t))
		{
			GameStats._actionStats[t] = new GameStats.ActionStat();
		}
		GameStats._actionStats[t].countCreated++;
	}

	[Conditional("GAME_STAT")]
	public static void On_DestroyAction(Type t)
	{
		if (!GameStats._actionStats.ContainsKey(t))
		{
			GameStats._actionStats[t] = new GameStats.ActionStat();
		}
		GameStats._actionStats[t].countDestroyed++;
	}

	[Conditional("GAME_STAT")]
	public static void On_SpawnEffect(string resId, bool usePool, Transform trans)
	{
		if (!GameStats._effectStats.ContainsKey(resId))
		{
			GameStats._effectStats[resId] = new GameStats.EffectStat();
		}
		if (usePool)
		{
			GameStats._effectStats[resId].poolSpawn++;
		}
		else
		{
			GameStats._effectStats[resId].rawSpawn++;
		}
	}

	[Conditional("GAME_STAT")]
	public static void On_DespawnEffect(string resId, Transform trans)
	{
		if (!GameStats._despawnStat.ContainsKey(resId))
		{
			GameStats._despawnStat[resId] = new GameStats.DespawnStat();
		}
		GameStats._despawnStat[resId].despawn++;
	}

	[Conditional("GAME_STAT")]
	public static void On_PrefabPoolSpawn(PrefabPool prefabPool)
	{
		GameStats.PoolStat poolStat = GameStats.FindOrCreate(prefabPool);
		poolStat.spawnCount++;
		GameStats.UpdatePrefabPoolStat(prefabPool, poolStat);
	}

	[Conditional("GAME_STAT")]
	public static void On_PrefabPoolDespawn(PrefabPool prefabPool, Transform instance)
	{
		GameStats.PoolStat poolStat = GameStats.FindOrCreate(prefabPool);
		poolStat.despawnCount++;
		GameStats.UpdatePrefabPoolStat(prefabPool, poolStat);
	}

	[Conditional("GAME_STAT")]
	public static void On_PrefabPoolSpawnNew(PrefabPool prefabPool)
	{
		GameStats.PoolStat poolStat = GameStats.FindOrCreate(prefabPool);
		poolStat.instantiateCount++;
		GameStats.UpdatePrefabPoolStat(prefabPool, poolStat);
	}

	[Conditional("GAME_STAT")]
	public static void OnInstantiate(string name)
	{
	}

	[Conditional("GAME_STAT")]
	public static void OnSpawnWithoutInstantiate(string name)
	{
	}

	private static void UpdatePrefabPoolStat(PrefabPool prefabPool, GameStats.PoolStat stat)
	{
		if (stat.maxDespawned < prefabPool.despawnedCount)
		{
			stat.maxDespawned = prefabPool.despawnedCount;
		}
		if (stat.maxSpawned < prefabPool.spawnedCount)
		{
			stat.maxSpawned = prefabPool.spawnedCount;
		}
	}

	private static GameStats.PoolStat FindOrCreate(PrefabPool prefabPool)
	{
		GameStats.PoolStat poolStat = GameStats._prefabPoolList.Find((GameStats.PoolStat x) => prefabPool == x.pool);
		if (poolStat == null)
		{
			poolStat = new GameStats.PoolStat
			{
				name = prefabPool.prefab.name,
				pool = prefabPool
			};
			GameStats._prefabPoolList.Add(poolStat);
		}
		return poolStat;
	}

	[Conditional("GAME_STAT")]
	public static void On_DespawnEffectHandle(ResourceHandle handle)
	{
		if (!GameStats._effectHandleStats.ContainsKey(handle.ResId))
		{
			GameStats._effectHandleStats[handle.ResId] = new GameStats.EffectStat();
		}
		GameStats._effectHandleStats[handle.ResId].despawn++;
	}

	[Conditional("GAME_STAT")]
	public static void On_SpawnEffectHandle(ResourceHandle handle)
	{
		if (!GameStats._effectHandleStats.ContainsKey(handle.ResId))
		{
			GameStats._effectHandleStats[handle.ResId] = new GameStats.EffectStat();
		}
		GameStats._effectHandleStats[handle.ResId].poolSpawn++;
	}

	private static void Report()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat("time passed: {0}\n", (GameStats._endTime - GameStats._startTime).TotalSeconds);
		List<KeyValuePair<int, GameStats.PackageStats>> list = GameStats._stats.ToList<KeyValuePair<int, GameStats.PackageStats>>();
		list.Sort((KeyValuePair<int, GameStats.PackageStats> x, KeyValuePair<int, GameStats.PackageStats> y) => y.Value.InAllCount - x.Value.InAllCount);
		List<KeyValuePair<int, GameStats.PackageStats>>.Enumerator enumerator = list.GetEnumerator();
		while (enumerator.MoveNext())
		{
			StringBuilder arg_95_0 = stringBuilder;
			string arg_95_1 = "{0}=>{1}\n";
			KeyValuePair<int, GameStats.PackageStats> current = enumerator.Current;
			object arg_95_2 = (PvpCode)current.Key;
			KeyValuePair<int, GameStats.PackageStats> current2 = enumerator.Current;
			arg_95_0.AppendFormat(arg_95_1, arg_95_2, current2.Value);
		}
		stringBuilder.Append("\n\n=================> actions created(countCreated/countDestroyed):\n");
		List<KeyValuePair<Type, GameStats.ActionStat>> list2 = GameStats._actionStats.ToList<KeyValuePair<Type, GameStats.ActionStat>>();
		list2.Sort((KeyValuePair<Type, GameStats.ActionStat> x, KeyValuePair<Type, GameStats.ActionStat> y) => y.Value.countCreated - x.Value.countCreated);
		List<KeyValuePair<Type, GameStats.ActionStat>>.Enumerator enumerator2 = list2.GetEnumerator();
		while (enumerator2.MoveNext())
		{
			StringBuilder arg_138_0 = stringBuilder;
			string arg_138_1 = "{0}=>{1}/{2}\n";
			KeyValuePair<Type, GameStats.ActionStat> current3 = enumerator2.Current;
			object arg_138_2 = current3.Key;
			KeyValuePair<Type, GameStats.ActionStat> current4 = enumerator2.Current;
			object arg_138_3 = current4.Value.countCreated;
			KeyValuePair<Type, GameStats.ActionStat> current5 = enumerator2.Current;
			arg_138_0.AppendFormat(arg_138_1, arg_138_2, arg_138_3, current5.Value.countDestroyed);
		}
		stringBuilder.Append("\n\n=================> effects spawned(poolSpawn/rawSpawn):\n");
		List<KeyValuePair<string, GameStats.EffectStat>> list3 = GameStats._effectStats.ToList<KeyValuePair<string, GameStats.EffectStat>>();
		list3.Sort((KeyValuePair<string, GameStats.EffectStat> x, KeyValuePair<string, GameStats.EffectStat> y) => y.Value.AllSpawn - x.Value.AllSpawn);
		List<KeyValuePair<string, GameStats.EffectStat>>.Enumerator enumerator3 = list3.GetEnumerator();
		while (enumerator3.MoveNext())
		{
			StringBuilder arg_1DE_0 = stringBuilder;
			string arg_1DE_1 = "{0}=>{1}/{2}\n";
			KeyValuePair<string, GameStats.EffectStat> current6 = enumerator3.Current;
			object arg_1DE_2 = current6.Key;
			KeyValuePair<string, GameStats.EffectStat> current7 = enumerator3.Current;
			object arg_1DE_3 = current7.Value.poolSpawn;
			KeyValuePair<string, GameStats.EffectStat> current8 = enumerator3.Current;
			arg_1DE_0.AppendFormat(arg_1DE_1, arg_1DE_2, arg_1DE_3, current8.Value.rawSpawn);
		}
		stringBuilder.Append("\n\n=================> effects despawned(despawn):\n");
		List<KeyValuePair<string, GameStats.DespawnStat>> list4 = GameStats._despawnStat.ToList<KeyValuePair<string, GameStats.DespawnStat>>();
		list4.Sort((KeyValuePair<string, GameStats.DespawnStat> x, KeyValuePair<string, GameStats.DespawnStat> y) => y.Value.despawn - x.Value.despawn);
		List<KeyValuePair<string, GameStats.DespawnStat>>.Enumerator enumerator4 = list4.GetEnumerator();
		while (enumerator4.MoveNext())
		{
			StringBuilder arg_26A_0 = stringBuilder;
			string arg_26A_1 = "{0}=>{1}\n";
			KeyValuePair<string, GameStats.DespawnStat> current9 = enumerator4.Current;
			object arg_26A_2 = current9.Key;
			KeyValuePair<string, GameStats.DespawnStat> current10 = enumerator4.Current;
			arg_26A_0.AppendFormat(arg_26A_1, arg_26A_2, current10.Value.despawn);
		}
		stringBuilder.Append("\n\n=================> effects managed by handle (AllSpawn/despawn):\n");
		List<KeyValuePair<string, GameStats.EffectStat>> list5 = GameStats._effectHandleStats.ToList<KeyValuePair<string, GameStats.EffectStat>>();
		list5.Sort((KeyValuePair<string, GameStats.EffectStat> x, KeyValuePair<string, GameStats.EffectStat> y) => y.Value.AllSpawn - x.Value.AllSpawn);
		List<KeyValuePair<string, GameStats.EffectStat>>.Enumerator enumerator5 = list5.GetEnumerator();
		while (enumerator5.MoveNext())
		{
			StringBuilder arg_310_0 = stringBuilder;
			string arg_310_1 = "{0}=>{1}/{2}\n";
			KeyValuePair<string, GameStats.EffectStat> current11 = enumerator5.Current;
			object arg_310_2 = current11.Key;
			KeyValuePair<string, GameStats.EffectStat> current12 = enumerator5.Current;
			object arg_310_3 = current12.Value.AllSpawn;
			KeyValuePair<string, GameStats.EffectStat> current13 = enumerator5.Current;
			arg_310_0.AppendFormat(arg_310_1, arg_310_2, arg_310_3, current13.Value.despawn);
		}
		stringBuilder.Append("\n\n=================> prefab pool:\n");
		List<GameStats.PoolStat> list6 = GameStats._prefabPoolList.ToList<GameStats.PoolStat>();
		list6.Sort((GameStats.PoolStat x, GameStats.PoolStat y) => y.maxSpawned - x.maxSpawned);
		List<GameStats.PoolStat>.Enumerator enumerator6 = list6.GetEnumerator();
		while (enumerator6.MoveNext())
		{
			stringBuilder.Append(enumerator6.Current.ToString() + "\n");
		}
		UnityEngine.Debug.LogError(stringBuilder.ToString());
	}

	private static void ReportProcessPvp()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("=================> process pvpcode:\n");
		stringBuilder.Append("code, frameNum, totalCals, sumMs, maxMs, avgMs\n");
		foreach (KeyValuePair<int, GameStats.PvpCodeProccessInfo> current in GameStats._pvpCodeProcessInfo)
		{
			stringBuilder.Append(string.Concat(new object[]
			{
				(PvpCode)current.Key,
				", ",
				current.Value.ToString(),
				"\n"
			}));
		}
		UnityEngine.Debug.LogError(stringBuilder.ToString());
	}

	public static void StartStat()
	{
		GameStats.Reset();
		GameStats._isStarted = true;
		GameStats._startTime = DateTime.Now;
	}

	public static void StopStat()
	{
		GameStats._isStarted = false;
		GameStats._endTime = DateTime.Now;
		GameStats.Report();
	}

	public static void Reset()
	{
		GameStats._actionStats.Clear();
		GameStats._stats.Clear();
		GameStats._pvpCodeProcessInfo.Clear();
	}

	[Conditional("MOBA_GUI")]
	public static void On_WidgetFill(UIWidget w)
	{
		GlobalSettings instance = GlobalSettings.Instance;
		if (!instance)
		{
			return;
		}
		if (instance.PvpSetting.testGUI)
		{
			UnityEngine.Debug.Log(string.Concat(new object[]
			{
				"Fill ",
				w.name,
				" (",
				Time.time,
				")",
				(!(w.panel == null)) ? w.panel.name : string.Empty
			}), w);
		}
		GameStats.WidgetStat widgetStat;
		if (!GlobalSettings.Instance.PvpSetting.recordGUI)
		{
			GameStats._widgetStats.Clear();
		}
		else if (GameStats._widgetStats.TryGetValue(w, out widgetStat))
		{
			widgetStat.fillNum++;
		}
		else
		{
			GameStats._widgetStats[w] = new GameStats.WidgetStat
			{
				fillNum = 1
			};
		}
	}

	[Conditional("MOBA_GUI")]
	public static void On_WidgetMove(UIWidget w)
	{
		GlobalSettings instance = GlobalSettings.Instance;
		if (!instance)
		{
			return;
		}
		if (instance.PvpSetting.testGUIMove)
		{
			UnityEngine.Debug.Log(string.Concat(new object[]
			{
				"Move ",
				w.name,
				" (",
				Time.time,
				")",
				(!(w.panel == null)) ? w.panel.name : string.Empty
			}), w);
		}
		if (GlobalSettings.Instance.PvpSetting.recordGUI)
		{
			GameStats.WidgetStat widgetStat;
			if (GameStats._widgetStats.TryGetValue(w, out widgetStat))
			{
				widgetStat.moveNum++;
			}
			else
			{
				GameStats._widgetStats[w] = new GameStats.WidgetStat
				{
					moveNum = 1
				};
			}
		}
	}

	private void ReportWidget()
	{
		List<KeyValuePair<UIWidget, GameStats.WidgetStat>> list = GameStats._widgetStats.ToList<KeyValuePair<UIWidget, GameStats.WidgetStat>>();
		list.Sort((KeyValuePair<UIWidget, GameStats.WidgetStat> x, KeyValuePair<UIWidget, GameStats.WidgetStat> y) => y.Value.fillNum - x.Value.fillNum);
		UnityEngine.Debug.Log("ReportWidget()");
		foreach (KeyValuePair<UIWidget, GameStats.WidgetStat> current in list)
		{
			UnityEngine.Debug.Log(string.Format("{0} fill {1} move {2}", current.Key.name, current.Value.fillNum, current.Value.moveNum), current.Key);
		}
	}

	private void OnGUI()
	{
		bool flag = GUILayout.Toggle(GameStats._isStarted, (!GameStats._isStarted) ? "start" : "stop", new GUILayoutOption[0]);
		if (flag != GameStats._isStarted)
		{
			if (flag)
			{
				GameStats.StartStat();
			}
			else
			{
				GameStats.StopStat();
			}
			GameStats._isStarted = flag;
		}
		if (GUILayout.Button("report", new GUILayoutOption[0]))
		{
			GameStats.Report();
		}
		if (GUILayout.Button("report_pvp_process", new GUILayoutOption[0]))
		{
			GameStats.ReportProcessPvp();
		}
		if (GUILayout.Button("pool stat", new GUILayoutOption[0]))
		{
			PoolManager.Dump();
		}
		if (GUILayout.Button("widget stat", new GUILayoutOption[0]))
		{
			this.ReportWidget();
		}
	}
}
