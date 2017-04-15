using Com.Game.Module;
using System;
using UnityEngine;

public class EasyCodeScannerControl : MonoBehaviour
{
	private static string dataStr;

	public Renderer PlaneRender;

	public bool isUsing;

	public bool toRun;

	private void Awake()
	{
		EasyCodeScannerControl.dataStr = string.Empty;
		NetWorkHelper.Instance.GateReconnection.LeaveGame("leave");
		EasyCodeScanner.Initialize();
		EasyCodeScanner.OnScannerMessage += new Action<string>(this.onScannerMessage);
		EasyCodeScanner.OnScannerEvent += new Action<string>(this.onScannerEvent);
		EasyCodeScanner.OnDecoderMessage += new Action<string>(this.onDecoderMessage);
	}

	public void Reset()
	{
		EasyCodeScannerControl.dataStr = string.Empty;
		this.isUsing = true;
		this.toRun = true;
	}

	private void OnDestroy()
	{
		EasyCodeScanner.OnScannerMessage -= new Action<string>(this.onScannerMessage);
		EasyCodeScanner.OnScannerEvent -= new Action<string>(this.onScannerEvent);
		EasyCodeScanner.OnDecoderMessage -= new Action<string>(this.onDecoderMessage);
	}

	public void Update()
	{
	}

	private void OnGUI()
	{
		if (this.isUsing && this.toRun)
		{
			this.toRun = false;
			base.Invoke("DelayLaunchScanner", 0.5f);
		}
	}

	private void DelayLaunchScanner()
	{
		EasyCodeScanner.launchScanner(true, string.Empty, -1, true);
	}

	private void onScannerMessage(string data)
	{
		EasyCodeScannerControl.dataStr = data;
		this.isUsing = false;
		Singleton<FriendView>.Instance.OnFindButtonQR(EasyCodeScannerControl.dataStr);
	}

	private void onScannerEvent(string eventStr)
	{
		if (EasyCodeScannerControl.dataStr != string.Empty)
		{
			this.isUsing = false;
			Singleton<FriendView>.Instance.OnFindButtonQR(EasyCodeScannerControl.dataStr);
		}
	}

	private void onDecoderMessage(string data)
	{
		EasyCodeScannerControl.dataStr = data;
		this.isUsing = false;
		Singleton<FriendView>.Instance.OnFindButtonQR(EasyCodeScannerControl.dataStr);
	}
}
