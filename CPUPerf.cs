using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;

public class CPUPerf : MonoBehaviour
{
	private static Dictionary<object, FuncInfo> PerfNameMap;

	public static bool ForcePrint;

	public bool mEnable = true;

	public int mUpdateInterval = 600;

	public int mDumpFPSLimit = 60;

	private int mFrameCnt;

	private long mCPUTime;

	private double mSpendTime;

	private double mTotalTime;

	private double mRadix;

	private static FileStream mPerfFile;

	private static StringBuilder mCacheString = new StringBuilder();

	public CPUPerf()
	{
		this.mCPUTime = Stopwatch.GetTimestamp();
		this.mRadix = 1000.0 / (double)Stopwatch.Frequency;
	}

	public static FuncInfo getFuncInfo(object targ)
	{
		if (CPUPerf.PerfNameMap == null)
		{
			CPUPerf.PerfNameMap = new Dictionary<object, FuncInfo>();
		}
		FuncInfo funcInfo;
		if (!CPUPerf.PerfNameMap.TryGetValue(targ, out funcInfo))
		{
			funcInfo = new FuncInfo(targ.ToString());
			CPUPerf.PerfNameMap.Add(targ, funcInfo);
		}
		return funcInfo;
	}

	private void addCell(object obj)
	{
		CPUPerf.mCacheString.Append(obj);
		CPUPerf.mCacheString.Append('\t');
	}

	private static void writeLine()
	{
		if (CPUPerf.mPerfFile == null)
		{
			string path = Application.persistentDataPath + "/pvpPerf.slk";
			CPUPerf.mPerfFile = File.Open(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
			string s = "Function\tCall count\tTotal time(ms)\tPercent(%)\tAvg time(ms)\tMax time(ms)\n";
			CPUPerf.mPerfFile.Write(Encoding.Default.GetBytes(s), 0, Encoding.Default.GetByteCount(s));
		}
		CPUPerf.mCacheString.Append('\n');
		string s2 = CPUPerf.mCacheString.ToString();
		CPUPerf.mPerfFile.Write(Encoding.Default.GetBytes(s2), 0, Encoding.Default.GetByteCount(s2));
		CPUPerf.mCacheString.Length = 0;
	}

	private void dumpInfo()
	{
		if (CPUPerf.PerfNameMap == null)
		{
			return;
		}
		foreach (FuncInfo current in CPUPerf.PerfNameMap.Values)
		{
			if (current.callCount > 0)
			{
				double num = (double)current.total * this.mRadix;
				double num2 = (double)current.maxTime * this.mRadix;
				double num3 = num / (double)current.callCount;
				this.addCell(current.name);
				this.addCell(current.callCount);
				this.addCell(num);
				this.addCell(num / this.mSpendTime * 100.0);
				this.addCell(num3);
				this.addCell(num2);
				CPUPerf.writeLine();
				current.callCount = 0;
				current.total = 0L;
				current.maxTime = 0L;
			}
		}
	}

	public static void Error(string printOut)
	{
		try
		{
			CPUPerf.mCacheString.Append(printOut);
			CPUPerf.writeLine();
			CPUPerf.mPerfFile.Flush();
		}
		catch (Exception var_0_20)
		{
		}
	}

	[Conditional("UseCPUCount")]
	private void Update()
	{
		if (++this.mFrameCnt >= this.mUpdateInterval)
		{
			long num = this.mCPUTime;
			this.mCPUTime = Stopwatch.GetTimestamp();
			this.mSpendTime = (double)(this.mCPUTime - num) * this.mRadix;
			double num2 = (double)(this.mFrameCnt * 1000) / this.mSpendTime;
			this.mTotalTime += this.mSpendTime;
			this.mFrameCnt = 0;
			if (num2 < (double)this.mDumpFPSLimit)
			{
				CPUPerf.ForcePrint = false;
				try
				{
					CPUPerf.writeLine();
					CPUPerf.mCacheString.Append("FPS:");
					CPUPerf.mCacheString.Append((float)num2);
					CPUPerf.mCacheString.Append(" | ");
					CPUPerf.mCacheString.Append(this.mUpdateInterval);
					CPUPerf.mCacheString.Append(" frame spend time about(ms): ");
					CPUPerf.mCacheString.Append((float)this.mSpendTime);
					CPUPerf.mCacheString.Append(" | cur time(s): ");
					CPUPerf.mCacheString.Append((float)(this.mTotalTime / 1000.0));
					CPUPerf.writeLine();
					this.dumpInfo();
					CPUPerf.mPerfFile.Flush();
				}
				catch (Exception var_2_133)
				{
				}
			}
			if (CPUPerf.PerfNameMap != null)
			{
				foreach (FuncInfo current in CPUPerf.PerfNameMap.Values)
				{
					current.callCount = 0;
					current.total = 0L;
					current.maxTime = 0L;
				}
			}
		}
	}
}
