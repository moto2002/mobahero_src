using System;
using System.Diagnostics;
using UnityEngine;

public class CpuStatistic : MonoBehaviour
{
	[SerializeField]
	public CpuStatisticInfo[] _arr = new CpuStatisticInfo[9];

	public static CpuStatisticInfo[] arr = new CpuStatisticInfo[9];

	private static CpuStatistic Instance = null;

	private int pressCnt = 0;

	private void Awake()
	{
		CpuStatistic.Instance = this;
		for (int i = 0; i < 9; i++)
		{
			CpuStatistic.arr[i] = new CpuStatisticInfo();
			CpuStatistic.arr[i].statisticName = (CpuStatisticType)i + ":";
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void LateUpdate()
	{
		for (int i = 0; i < 9; i++)
		{
			CpuStatisticInfo cpuStatisticInfo = CpuStatistic.arr[i];
			if (cpuStatisticInfo._totalCpuTime > 0L)
			{
				cpuStatisticInfo.frameCount++;
				cpuStatisticInfo.totalCpuTime = (int)cpuStatisticInfo._totalCpuTime / 10;
				cpuStatisticInfo.cpuTimePerFrame = (int)cpuStatisticInfo._totalCpuTime / cpuStatisticInfo.frameCount / 10;
			}
			this._arr[i] = CpuStatistic.arr[i];
		}
	}

	private void OnGUI()
	{
		if (GUILayout.Button("Reset Cpu Statistic", new GUILayoutOption[0]))
		{
			for (int i = 0; i < 9; i++)
			{
				CpuStatistic.arr[i].frameCount = 0;
				CpuStatistic.arr[i].totalCpuTime = 0;
				CpuStatistic.arr[i]._totalCpuTime = 0L;
			}
		}
	}

	[Conditional("UseCpuStatistic")]
	public static void SampleBegin(CpuStatisticType type)
	{
		CpuStatistic.AddGo();
		CpuStatistic.arr[(int)type]._sampleBeginTime = DateTime.Now.Ticks;
	}

	[Conditional("UseCpuStatistic")]
	public static void SampleEnd(CpuStatisticType type)
	{
		CpuStatisticInfo cpuStatisticInfo = CpuStatistic.arr[(int)type];
		cpuStatisticInfo._totalCpuTime += DateTime.Now.Ticks - cpuStatisticInfo._sampleBeginTime;
	}

	private static void AddGo()
	{
		if (CpuStatistic.Instance == null)
		{
			GameObject gameObject = new GameObject("CpuStatistic");
			CpuStatistic.Instance = gameObject.AddComponent<CpuStatistic>();
		}
	}
}
