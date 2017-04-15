using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EasyCodeScanner : MonoBehaviour
{
	private static EasyCodeScanner instance;

	public static event Action<string> OnScannerMessage
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			EasyCodeScanner.OnScannerMessage = (Action<string>)Delegate.Combine(EasyCodeScanner.OnScannerMessage, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			EasyCodeScanner.OnScannerMessage = (Action<string>)Delegate.Remove(EasyCodeScanner.OnScannerMessage, value);
		}
	}

	public static event Action<string> OnScannerEvent
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			EasyCodeScanner.OnScannerEvent = (Action<string>)Delegate.Combine(EasyCodeScanner.OnScannerEvent, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			EasyCodeScanner.OnScannerEvent = (Action<string>)Delegate.Remove(EasyCodeScanner.OnScannerEvent, value);
		}
	}

	public static event Action<string> OnDecoderMessage
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			EasyCodeScanner.OnDecoderMessage = (Action<string>)Delegate.Combine(EasyCodeScanner.OnDecoderMessage, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			EasyCodeScanner.OnDecoderMessage = (Action<string>)Delegate.Remove(EasyCodeScanner.OnDecoderMessage, value);
		}
	}

	public static void Initialize()
	{
		if (EasyCodeScanner.instance == null)
		{
			GameObject gameObject = new GameObject("CodeScannerBridge");
			gameObject.AddComponent<EasyCodeScanner>();
			EasyCodeScanner.instance = gameObject.GetComponent<EasyCodeScanner>();
		}
	}

	private void Start()
	{
	}

	private void onScannerMessage(string data)
	{
		if (EasyCodeScanner.OnScannerMessage != null)
		{
			EasyCodeScanner.OnScannerMessage(data);
		}
	}

	private void onScannerEvent(string eventStr)
	{
		if (EasyCodeScanner.OnScannerEvent != null)
		{
			EasyCodeScanner.OnScannerEvent(eventStr);
		}
	}

	private void onDecoderMessage(string code)
	{
		if (EasyCodeScanner.OnDecoderMessage != null)
		{
			EasyCodeScanner.OnDecoderMessage(code);
		}
	}

	public static void launchScanner(bool showUI, string defaultTxt, int symbol, bool forceLandscape)
	{
		if (EasyCodeScanner.instance == null)
		{
			Debug.LogError("EasyCodeScanner - launchScanner error : scanner must be initialized before.");
			return;
		}
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.c4mprod.ezcodescanner.RootActivity");
		androidJavaClass2.CallStatic("launchScannerImpl", new object[]
		{
			@static,
			showUI,
			defaultTxt,
			symbol,
			forceLandscape
		});
	}

	public static byte[] getScannerImage()
	{
		if (EasyCodeScanner.instance == null)
		{
			Debug.LogError("EasyCodeScanner - launchScanner error : scanner must be initialized before.");
			return null;
		}
		return null;
	}

	public static Texture2D getScannerImage(int texWidth, int texHeigh)
	{
		if (EasyCodeScanner.instance == null)
		{
			Debug.LogError("EasyCodeScanner - launchScanner error : scanner must be initialized before.");
			return null;
		}
		return null;
	}

	public static void decodeImage(int symbols, byte[] image)
	{
		if (EasyCodeScanner.instance == null)
		{
			Debug.LogError("EasyCodeScanner - launchScanner error : scanner must be initialized before.");
			return;
		}
	}

	public static void decodeImage(int symbols, Texture2D texture)
	{
		if (EasyCodeScanner.instance == null)
		{
			Debug.LogError("EasyCodeScanner - launchScanner error : scanner must be initialized before.");
			return;
		}
	}
}
