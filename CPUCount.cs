using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class CPUCount
{
	private static List<FuncInfo> CounterList;

	public static List<string> NameList = new List<string>();

	public static List<FuncInfo> FuncList;

	public static int FuncNum = 0;

	private int id;

	private string name;

	public CPUCount(string funcName)
	{
		this.name = funcName;
		this.id = CPUCount.NameList.Count;
		CPUCount.NameList.Add(funcName);
	}

	[Conditional("UseCPUCount")]
	public static void Start(object target = null)
	{
		if (CPUCount.CounterList == null)
		{
			CPUCount.CounterList = new List<FuncInfo>();
		}
		FuncInfo funcInfo = CPUPerf.getFuncInfo(target);
		CPUCount.CounterList.Add(funcInfo);
		if (++funcInfo.recCnt == 1)
		{
			funcInfo.lastRecord = Stopwatch.GetTimestamp();
		}
	}

	[Conditional("UseCPUCount")]
	public static void End()
	{
		CPUCount.CounterList.Last<FuncInfo>().recordTime(Stopwatch.GetTimestamp());
		CPUCount.CounterList.RemoveAt(CPUCount.CounterList.Count - 1);
	}

	public static T End<T>(T result)
	{
		CPUCount.CounterList.Last<FuncInfo>().recordTime(Stopwatch.GetTimestamp());
		CPUCount.CounterList.RemoveAt(CPUCount.CounterList.Count - 1);
		return result;
	}

	[Conditional("UseCPUCount")]
	public static void Count(object target = null)
	{
		FuncInfo funcInfo = CPUPerf.getFuncInfo(target);
		funcInfo.callCount++;
	}

	public FuncInfo getInfo()
	{
		if (this.id < CPUCount.FuncNum)
		{
			return CPUCount.FuncList[this.id];
		}
		if (CPUCount.FuncList == null)
		{
			CPUCount.FuncList = new List<FuncInfo>();
		}
		while (CPUCount.FuncNum < CPUCount.NameList.Count)
		{
			CPUCount.FuncList.Add(new FuncInfo(CPUCount.NameList[CPUCount.FuncNum]));
			CPUCount.FuncNum = CPUCount.FuncList.Count;
		}
		return CPUCount.FuncList[this.id];
	}

	[Conditional("UseCPUCount")]
	public void start()
	{
		FuncInfo info = this.getInfo();
		if (++info.recCnt == 1)
		{
			info.lastRecord = Stopwatch.GetTimestamp();
		}
	}

	[Conditional("UseCPUCount")]
	public void end()
	{
		FuncInfo info = this.getInfo();
		info.recordTime(Stopwatch.GetTimestamp());
	}

	[Conditional("UseCPUCount")]
	public void count()
	{
		FuncInfo info = this.getInfo();
		info.callCount++;
	}

	public static implicit operator CPUCount(string funcName)
	{
		return new CPUCount(funcName);
	}
}
