using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class BuglyTest : MonoBehaviour
{
	private const string BuglyAppIDForiOS = "900014645";

	private const string BuglyAppIDForAndroid = "900016162";

	private string errorTxt = string.Empty;

	private int selGridIntCurrent = 5;

	private int selGridIntDefault = -1;

	private Vector2 scrollPosition = Vector2.zero;

	private string[] selGridItems = new string[]
	{
		"Exception",
		"SystemException",
		"ApplicationException",
		"ArgumentException",
		"FormatException",
		"...",
		"MemberAccessException",
		"FileAccessException",
		"MethodAccessException",
		"MissingMemberException",
		"MissingMethodException",
		"MissingFieldException",
		"IndexOutOfException",
		"ArrayTypeMismatchException",
		"RankException",
		"IOException",
		"DirectionNotFoundException",
		"FileNotFoundException",
		"EndOfStreamException",
		"FileLoadException",
		"PathTooLongException",
		"ArithmeticException",
		"NotFiniteNumberException",
		"DivideByZeroException",
		"OutOfMemoryException",
		"NullReferenceException",
		"InvalidCastException",
		"InvalidOperationException",
		string.Empty,
		string.Empty,
		"TestCrash"
	};

	private GUIStyle styleTitle;

	private GUIStyle styleContent;

	private static float StandardScreenWidth = 640f;

	private static float StandardScreenHeight = 960f;

	private float guiRatioX;

	private float guiRatioY;

	private float screenWidth;

	private float screenHeight;

	private Vector3 scaleGUIs;

	private Dictionary<string, string> MyLogCallbackExtrasHandler()
	{
		BuglyAgent.PrintLog(LogSeverity.Log, "extra handler", new object[0]);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("ScreenSolution", string.Format("{0}x{1}", Screen.width, Screen.height));
		dictionary.Add("deviceModel", SystemInfo.deviceModel);
		dictionary.Add("deviceName", SystemInfo.deviceName);
		dictionary.Add("deviceType", SystemInfo.deviceType.ToString());
		dictionary.Add("deviceUId", SystemInfo.deviceUniqueIdentifier);
		dictionary.Add("gDId", string.Format("{0}", SystemInfo.graphicsDeviceID));
		dictionary.Add("gDName", SystemInfo.graphicsDeviceName);
		dictionary.Add("gDVdr", SystemInfo.graphicsDeviceVendor);
		dictionary.Add("gDVer", SystemInfo.graphicsDeviceVersion);
		dictionary.Add("gDVdrID", string.Format("{0}", SystemInfo.graphicsDeviceVendorID));
		dictionary.Add("graphicsMemorySize", string.Format("{0}", SystemInfo.graphicsMemorySize));
		dictionary.Add("systemMemorySize", string.Format("{0}", SystemInfo.systemMemorySize));
		dictionary.Add("UnityVersion", Application.unityVersion);
		BuglyAgent.PrintLog(LogSeverity.LogInfo, "Package extra data", new object[0]);
		return dictionary;
	}

	private void Start()
	{
		BuglyAgent.PrintLog(LogSeverity.LogInfo, "Demo Start()", new object[0]);
		this.SetupGUIStyle();
		BuglyAgent.PrintLog(LogSeverity.LogWarning, "Init bugly sdk done", new object[0]);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Home))
		{
			Application.Quit();
		}
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
	}

	private void OnGUI()
	{
		this.StyledGUI();
		GUILayout.BeginArea(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height));
		GUILayout.BeginVertical(new GUILayoutOption[0]);
		GUILayout.Space(20f);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.FlexibleSpace();
		GUILayout.Label("Bugly Unity Demo", this.styleTitle, new GUILayoutOption[0]);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.Space(20f);
		GUILayout.BeginVertical(new GUILayoutOption[0]);
		GUILayout.Label("Uncaught Exceptions:    " + this.errorTxt, this.styleContent, new GUILayoutOption[0]);
		GUILayout.Space(20f);
		this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition, new GUILayoutOption[]
		{
			GUILayout.Width((float)Screen.width),
			GUILayout.Height((float)(Screen.height - 100))
		});
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Space(40f);
		this.selGridIntCurrent = GUILayout.SelectionGrid(this.selGridIntCurrent, this.selGridItems, 2, new GUILayoutOption[0]);
		GUILayout.Space(40f);
		GUILayout.EndHorizontal();
		GUILayout.EndScrollView();
		if (this.selGridIntCurrent != this.selGridIntDefault)
		{
			this.selGridIntDefault = this.selGridIntCurrent;
			this.TrigException(this.selGridIntCurrent);
		}
		GUILayout.EndVertical();
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}

	private void TrigException(int selGridInt)
	{
		BuglyAgent.PrintLog(LogSeverity.LogWarning, "Trigge Exception: {0}", new object[]
		{
			selGridInt
		});
		switch (selGridInt)
		{
		case 0:
			this.throwException(new Exception("Non-fatal error, an base C# exception"));
			break;
		case 1:
			this.throwException(new SystemException("Fatal error, a system exception"));
			break;
		case 2:
			this.throwException(new ApplicationException("Fatal error, an application exception"));
			break;
		case 3:
			this.throwException(new ArgumentException(string.Format("Fatal error, {0} ", this.selGridItems[selGridInt])));
			break;
		case 4:
			this.throwException(new FormatException(string.Format("Fatal error, {0} ", this.selGridItems[selGridInt])));
			break;
		case 5:
			break;
		case 6:
			this.throwException(new MemberAccessException(string.Format("Fatal error, {0} ", this.selGridItems[selGridInt])));
			break;
		case 7:
			this.throwException(new FieldAccessException(string.Format("Fatal error, {0} ", this.selGridItems[selGridInt])));
			break;
		case 8:
			this.throwException(new MethodAccessException(string.Format("Fatal error, {0} ", this.selGridItems[selGridInt])));
			break;
		case 9:
			this.throwException(new MissingMemberException(string.Format("Fatal error, {0} ", this.selGridItems[selGridInt])));
			break;
		case 10:
			this.throwException(new MissingMethodException(string.Format("Fatal error, {0} ", this.selGridItems[selGridInt])));
			break;
		case 11:
			this.throwException(new MissingFieldException(string.Format("Fatal error, {0} ", this.selGridItems[selGridInt])));
			break;
		case 12:
			this.throwException(new IndexOutOfRangeException(string.Format("Non-Fatal error, {0} ", this.selGridItems[selGridInt])));
			break;
		case 13:
			this.throwException(new ArrayTypeMismatchException(string.Format("Non-Fatal error, {0} ", this.selGridItems[selGridInt])));
			break;
		case 14:
			this.throwException(new RankException(string.Format("Non-Fatal error, {0} ", this.selGridItems[selGridInt])));
			break;
		case 15:
		case 16:
		case 17:
		case 18:
		case 19:
		case 20:
			try
			{
				this.throwException(new Exception(string.Format("Fatal error, {0} ", this.selGridItems[selGridInt])));
			}
			catch (Exception e)
			{
				BuglyAgent.ReportException(e, "Caught an exception.");
			}
			break;
		case 21:
			this.throwException(new ArithmeticException(string.Format("Fatal error, {0} ", this.selGridItems[selGridInt])));
			break;
		case 22:
			this.throwException(new NotFiniteNumberException(string.Format("Fatal error, {0} ", this.selGridItems[selGridInt])));
			break;
		case 23:
		{
			int num = 0;
			num = 2 / num;
			break;
		}
		case 24:
			this.throwException(new OutOfMemoryException("Fatal error, OOM"));
			break;
		case 25:
			this.findGameObject();
			break;
		case 26:
		{
			Exception ex = null;
			IndexOutOfRangeException arg = (IndexOutOfRangeException)ex;
			Console.Write(string.Empty + arg);
			break;
		}
		case 27:
			this.findGameObjectByTag();
			break;
		case 28:
			this.DoCrash();
			break;
		case 29:
		case 30:
			this.TestCrashNative();
			this.TestCrash();
			break;
		default:
			try
			{
				this.throwException(new OutOfMemoryException("Fatal error, out of memory"));
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			break;
		}
	}

	private void findGameObjectByTag()
	{
		Console.Write("it will throw UnityException");
		GameObject gameObject = GameObject.FindWithTag("test");
		string name = gameObject.name;
		Console.Write(name);
	}

	private void findGameObject()
	{
		Console.Write("it will throw NullReferenceException");
		GameObject gameObject = GameObject.Find("test");
		string name = gameObject.name;
		Console.Write(name);
	}

	private void throwException(Exception e)
	{
		if (e == null)
		{
			return;
		}
		BuglyAgent.PrintLog(LogSeverity.LogWarning, "Throw exception: {0}", new object[]
		{
			e.ToString()
		});
		this.testDeepFrame(e);
	}

	private void testDeepFrame(Exception e)
	{
		this.errorTxt = e.Message;
		throw e;
	}

	private void DoCrash()
	{
		Console.Write("it will Crash...");
		this.DoCrash();
	}

	private void SetupGUIStyle()
	{
		this.styleTitle = new GUIStyle();
		this.styleTitle.fontSize = 28;
		this.styleTitle.fontStyle = FontStyle.Bold;
		this.styleContent = new GUIStyle();
		this.styleContent.fontSize = 20;
		this.styleContent.fontStyle = FontStyle.Italic;
	}

	private void Awake()
	{
		BuglyAgent.DebugLog("Demo.Awake()", "Screen: {0} x {1}", new object[]
		{
			Screen.width,
			Screen.height
		});
		this.screenWidth = (float)Screen.width;
		this.screenHeight = (float)Screen.height;
		this.guiRatioX = this.screenWidth / BuglyTest.StandardScreenWidth * 1f;
		this.guiRatioY = this.screenHeight / BuglyTest.StandardScreenHeight * 1f;
		this.scaleGUIs = new Vector3(this.guiRatioX, this.guiRatioY, 1f);
	}

	public void StyledGUI()
	{
		GUI.color = Color.gray;
		GUI.skin.label.fontSize = 20;
		GUI.skin.button.fontSize = 20;
	}

	private unsafe void TestCrash()
	{
		char* ptr = 12345;
		MonoBehaviour.print(*ptr);
	}

	private void TestCrashNative()
	{
		CrashHelper.read_null();
		CrashHelper.write_null();
	}
}
