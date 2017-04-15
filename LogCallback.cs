using Assets.Scripts.Model;
using Assets.Scripts.Server;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class LogCallback : IGlobalComServer
{
	private string logPath;

	private bool hasLog;

	private string version = string.Empty;

	private CoroutineManager coroutineMng;

	void IGlobalComServer.OnDestroy()
	{
		Application.RegisterLogCallback(null);
		this.coroutineMng.StopAllCoroutine();
	}

	public void OnAwake()
	{
		this.logPath = Application.persistentDataPath + "/log.txt";
		this.coroutineMng = new CoroutineManager();
	}

	public void OnStart()
	{
		Application.RegisterLogCallback(new Application.LogCallback(this.HandleLogCallback));
		this.hasLog = false;
		this.version = GlobalSettings.AppVersion;
	}

	public void OnUpdate()
	{
	}

	public void Enable(bool b)
	{
	}

	public void OnRestart()
	{
	}

	public void OnApplicationQuit()
	{
	}

	public void OnApplicationFocus(bool isFocus)
	{
	}

	public void OnApplicationPause(bool isPause)
	{
	}

	private WWW CreateWWW(string log, string stackTrace, LogType type)
	{
		string text = string.Empty;
		string text2 = string.Empty;
		string text3 = string.Empty;
		string text4 = string.Empty;
		string text5 = string.Empty;
		string text6 = string.Empty;
		AccountData accountData = ModelManager.Instance.Get_accountData_X();
		UserData userData = ModelManager.Instance.Get_userData_X();
		if (accountData != null)
		{
			text = accountData.AccountId;
			text2 = accountData.UserName;
		}
		if (userData != null)
		{
			text3 = userData.NickName;
		}
		text6 = DateTime.Now.ToString();
		text5 = log + "\n" + stackTrace;
		text4 = SystemInfo.deviceUniqueIdentifier;
		WWWForm wWWForm = new WWWForm();
		if (text2 != null)
		{
			wWWForm.AddField("UserId", text2);
		}
		if (text != null)
		{
			wWWForm.AddField("AccountId", text);
		}
		if (text4 != null)
		{
			wWWForm.AddField("DeviceCode", text4);
		}
		if (text5 != null)
		{
			text5 = this.ReplaceDanger(text5);
		}
		if (text5 != null)
		{
			wWWForm.AddField("Message", text5);
		}
		if (text6 != null)
		{
			wWWForm.AddField("DateTime", text6);
		}
		if (this.version != null)
		{
			wWWForm.AddField("Versions", this.version);
		}
		if (SystemInfo.deviceModel != null)
		{
			wWWForm.AddField("Device", SystemInfo.deviceModel);
		}
		return new WWW("http://mobacrashlog.xiaomeng.cc:8081/UploadException.ashx", wWWForm);
	}

	private string ReplaceDanger(string str)
	{
		str = str.Replace(">", "&gt;");
		str = str.Replace("<", "&lt;");
		str = str.Replace('"'.ToString(), "&quot;");
		str = str.Replace('\''.ToString(), "&#39;");
		str = str.Replace('\r'.ToString(), " ");
		str = str.Replace('\n'.ToString(), "br");
		return str;
	}

	private void WriteLog(string log, string stackTrace)
	{
		using (StreamWriter streamWriter = File.AppendText(this.logPath))
		{
			streamWriter.WriteLine(this.GetCurTime());
			streamWriter.WriteLine(this.version);
			streamWriter.WriteLine(SystemInfo.deviceModel);
			streamWriter.WriteLine(log + "\n" + stackTrace);
			streamWriter.WriteLine();
			this.hasLog = true;
		}
	}

	private void HandleLogCallback_Android(string log, string stackTrace, LogType type)
	{
		if (type == LogType.Error || type == LogType.Exception)
		{
			this.coroutineMng.StartCoroutine(this.HandleLogCallback_AndroidWait(log, stackTrace, type), true);
		}
	}

	[DebuggerHidden]
	private IEnumerator HandleLogCallback_AndroidWait(string log, string stackTrace, LogType type)
	{
		LogCallback.<HandleLogCallback_AndroidWait>c__Iterator9F <HandleLogCallback_AndroidWait>c__Iterator9F = new LogCallback.<HandleLogCallback_AndroidWait>c__Iterator9F();
		<HandleLogCallback_AndroidWait>c__Iterator9F.log = log;
		<HandleLogCallback_AndroidWait>c__Iterator9F.stackTrace = stackTrace;
		<HandleLogCallback_AndroidWait>c__Iterator9F.type = type;
		<HandleLogCallback_AndroidWait>c__Iterator9F.<$>log = log;
		<HandleLogCallback_AndroidWait>c__Iterator9F.<$>stackTrace = stackTrace;
		<HandleLogCallback_AndroidWait>c__Iterator9F.<$>type = type;
		<HandleLogCallback_AndroidWait>c__Iterator9F.<>f__this = this;
		return <HandleLogCallback_AndroidWait>c__Iterator9F;
	}

	private void HandleLogCallback_IOS(string log, string stackTrace, LogType type)
	{
		if (type == LogType.Error || type == LogType.Exception)
		{
			this.coroutineMng.StartCoroutine(this.IosReportError_Coroutine(log, stackTrace, type), true);
		}
	}

	[DebuggerHidden]
	private IEnumerator IosReportError_Coroutine(string log, string stackTrace, LogType type)
	{
		LogCallback.<IosReportError_Coroutine>c__IteratorA0 <IosReportError_Coroutine>c__IteratorA = new LogCallback.<IosReportError_Coroutine>c__IteratorA0();
		<IosReportError_Coroutine>c__IteratorA.log = log;
		<IosReportError_Coroutine>c__IteratorA.stackTrace = stackTrace;
		<IosReportError_Coroutine>c__IteratorA.type = type;
		<IosReportError_Coroutine>c__IteratorA.<$>log = log;
		<IosReportError_Coroutine>c__IteratorA.<$>stackTrace = stackTrace;
		<IosReportError_Coroutine>c__IteratorA.<$>type = type;
		<IosReportError_Coroutine>c__IteratorA.<>f__this = this;
		return <IosReportError_Coroutine>c__IteratorA;
	}

	private void HandleLogCallback(string log, string stackTrace, LogType type)
	{
		this.HandleLogCallback_Android(log, stackTrace, type);
	}

	private string GetCurTime()
	{
		return DateTime.Now.ToString("yyyy-MM-dd HH:mm");
	}

	public void SendLogToServer()
	{
		if (!File.Exists(this.logPath))
		{
			return;
		}
		FileStream fileStream = new FileStream(this.logPath, FileMode.Open);
		BinaryReader binaryReader = new BinaryReader(fileStream);
		byte[] inArray = binaryReader.ReadBytes(Convert.ToInt32(fileStream.Length));
		string value = Convert.ToBase64String(inArray);
		binaryReader.Close();
		fileStream.Close();
		AccountData accountData = ModelManager.Instance.Get_accountData_X();
		string value2;
		if (accountData != null)
		{
			value2 = accountData.AccountId + "(" + accountData.UserName + ")";
		}
		else
		{
			value2 = SystemInfo.deviceUniqueIdentifier;
		}
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("UserId", value2);
		wWWForm.AddField("Data", value);
		string str = ModelManager.Instance.Get_IPProperty();
		WWW wWW = new WWW("http://" + str + ":1988/UploadException.ashx", wWWForm);
		while (!wWW.isDone)
		{
		}
		if (wWW.text.Equals("Hello WorldOK!"))
		{
			File.Delete(this.logPath);
		}
		wWW.Dispose();
	}
}
