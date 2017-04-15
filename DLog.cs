using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;

public class DLog : MonoBehaviour
{
	private enum EndLine
	{
		endl = 10
	}

	public static DLog GameLog;

	private static FileStream mLogFile;

	private static FileStream mErrFile;

	private static FileStream mCurFile;

	private StringBuilder mCacheString = new StringBuilder();

	public byte LogLevel = 3;

	public bool DebugPrintTime = true;

	public bool UseSystemDebug;

	public bool UseScreenLogOut;

	public static bool mEnable;

	public static bool checkPrintOut = true;

	public bool logOnGame = true;

	public bool logOnNet = true;

	public bool logOnMove = true;

	public bool logOnTurn = true;

	public bool logOnState = true;

	public bool logOnLoading = true;

	public bool logOnAnim = true;

	public static DLog dtNet
	{
		get
		{
			DLog.checkPrintOut = (DLog.GameLog != null && DLog.GameLog.logOnNet);
			return DLog.GameLog;
		}
	}

	public static DLog dtGame
	{
		get
		{
			DLog.checkPrintOut = (DLog.GameLog != null && DLog.GameLog.logOnGame);
			return DLog.GameLog;
		}
	}

	public static DLog dtMove
	{
		get
		{
			DLog.checkPrintOut = (DLog.GameLog != null && DLog.GameLog.logOnMove);
			return DLog.GameLog;
		}
	}

	public static DLog dtTurn
	{
		get
		{
			DLog.checkPrintOut = (DLog.GameLog != null && DLog.GameLog.logOnTurn);
			return DLog.GameLog;
		}
	}

	public static DLog dtState
	{
		get
		{
			DLog.checkPrintOut = (DLog.GameLog != null && DLog.GameLog.logOnState);
			return DLog.GameLog;
		}
	}

	public static DLog dtLoading
	{
		get
		{
			DLog.checkPrintOut = (DLog.GameLog != null && DLog.GameLog.logOnLoading);
			return DLog.GameLog;
		}
	}

	public static DLog dtAnim
	{
		get
		{
			DLog.checkPrintOut = (DLog.GameLog != null && DLog.GameLog.logOnAnim);
			return DLog.GameLog;
		}
	}

	private void Awake()
	{
		DLog.GameLog = this;
		DLog.mEnable = true;
		DLog.mLogFile = File.Open(Application.persistentDataPath + "/game.log", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
		DLog.mErrFile = File.Open(Application.persistentDataPath + "/error.log", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
		DLog.mErrFile.Seek(0L, SeekOrigin.End);
		Application.RegisterLogCallback(new Application.LogCallback(this.HandleLogCallback));
	}

	private void HandleLogCallback(string logString, string stackTrace, LogType type)
	{
		if (type == LogType.Error || type == LogType.Exception)
		{
			DLog.mErrFile.WriteByte(10);
			DLog.mErrFile.Write(Encoding.Default.GetBytes(logString), 0, Encoding.Default.GetByteCount(logString));
			DLog.mErrFile.Write(Encoding.Default.GetBytes(stackTrace), 0, Encoding.Default.GetByteCount(stackTrace));
			DLog.mErrFile.Flush();
		}
	}

	private void OnApplicationQuit()
	{
		if (DLog.mLogFile != null)
		{
			DLog.mLogFile.Close();
			DLog.mLogFile = null;
		}
		if (DLog.mErrFile != null)
		{
			DLog.mErrFile.Close();
			DLog.mErrFile = null;
		}
		DLog.mEnable = false;
		DLog.GameLog = null;
	}

	private void _log(DLog.EndLine output)
	{
		if (this.UseScreenLogOut)
		{
			Cheat.Msg(this.mCacheString.ToString());
		}
		if (this.UseSystemDebug)
		{
			UnityEngine.Debug.Log(this.mCacheString);
		}
		else
		{
			this.mCacheString.Append('\n');
			string s = this.mCacheString.ToString();
			DLog.mCurFile.Write(Encoding.Default.GetBytes(s), 0, Encoding.Default.GetByteCount(s));
			DLog.mCurFile.Flush();
		}
		this.mCacheString.Length = 0;
	}

	private void _log(object output)
	{
		if (output != null)
		{
			this.mCacheString.Append(output);
		}
		else
		{
			this.mCacheString.Append("null");
		}
	}

	[Conditional("MOBA_TESTING")]
	public void Info(params object[] output)
	{
		if (!DLog.checkPrintOut || this.LogLevel < 4 || !DLog.mEnable)
		{
			return;
		}
		DLog.mCurFile = DLog.mLogFile;
		if (this.DebugPrintTime)
		{
			this._log(Time.realtimeSinceStartup);
		}
		this._log("[Info] ");
		for (int i = 0; i < output.Length; i++)
		{
			this._log(output[i]);
		}
		this._log(DLog.EndLine.endl);
	}

	[Conditional("MOBA_TESTING")]
	public void Log(params object[] output)
	{
		if (!DLog.checkPrintOut || this.LogLevel < 3 || !DLog.mEnable)
		{
			return;
		}
		DLog.mCurFile = DLog.mLogFile;
		if (this.DebugPrintTime)
		{
			this._log(Time.realtimeSinceStartup);
		}
		this._log("[Log] ");
		for (int i = 0; i < output.Length; i++)
		{
			this._log(output[i]);
		}
		this._log(DLog.EndLine.endl);
	}

	public void Warning(params object[] output)
	{
		if (!DLog.checkPrintOut || this.LogLevel < 2 || !DLog.mEnable)
		{
			return;
		}
		DLog.mCurFile = DLog.mLogFile;
		if (this.DebugPrintTime)
		{
			this._log(Time.time);
		}
		this._log("[Warning] ");
		for (int i = 0; i < output.Length; i++)
		{
			this._log(output[i]);
		}
		this._log(DLog.EndLine.endl);
	}

	public void Error(params object[] output)
	{
		if (!DLog.checkPrintOut || this.LogLevel < 1 || !DLog.mEnable)
		{
			return;
		}
		DLog.mCurFile = DLog.mErrFile;
		if (this.DebugPrintTime)
		{
			this._log(Time.time);
		}
		this._log("[Error] ");
		for (int i = 0; i < output.Length; i++)
		{
			this._log(output[i]);
		}
		this._log(DLog.EndLine.endl);
	}

	[Conditional("MOBA_TESTING")]
	public void unitInfo(Units checkUnit, params object[] output)
	{
		if (checkUnit.DebugDLog)
		{
		}
	}

	[Conditional("MOBA_TESTING")]
	public void unitLog(Units checkUnit, params object[] output)
	{
		if (checkUnit.DebugDLog)
		{
		}
	}

	public void unitWarning(Units checkUnit, params object[] output)
	{
		if (checkUnit.DebugDLog)
		{
			this.Warning(output);
		}
	}

	public void unitError(Units checkUnit, params object[] output)
	{
		if (checkUnit.DebugDLog)
		{
			this.Error(output);
		}
	}
}
