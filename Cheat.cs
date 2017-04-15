using CodeStage.AdvancedFPSCounter;
using Com.Game.Module;
using HUD.Module;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

public class Cheat : MonoBehaviour
{
	private enum GMInstruction
	{
		ShowMoveDelay = 58,
		SetMoveDelay,
		ShowSceneEditor,
		ShowAFPSCount
	}

	private string mCheatCode = string.Empty;

	private bool mIsShown;

	private Queue<string> mHistory = new Queue<string>();

	private bool isInit;

	public bool setOn;

	public bool showDebugLog = true;

	public int HistroyCount = 9;

	private bool _noTouch = true;

	private GUIStyle buttonStyle;

	private GUIStyle labelStyle;

	private GUIStyle textFieldStyle;

	private int ButtonSize = 20;

	private static float messageTime = 10f;

	private static float scrollTime = 0.3f;

	private static float fadeTime = 0.5f;

	private static float fadeOutTime = Cheat.messageTime - Cheat.fadeTime;

	private static int lastHight = 0;

	private static Rect ListWinRect = new Rect((float)(Screen.width / 2 - 100), 0f, 200f, 200f);

	private static ArrayList messages = new ArrayList();

	private static ArrayList times = new ArrayList();

	private static ArrayList colors = new ArrayList();

	private static ArrayList infos = new ArrayList();

	private static float lastTime = 0f;

	private static StringBuilder mCacheString = new StringBuilder();

	private GameObject fogobj;

	private Dictionary<string, GameObject> TestObjs = new Dictionary<string, GameObject>();

	private static bool _profileBegin;

	public static Cheat Instance
	{
		get;
		private set;
	}

	public void Awake()
	{
		if (!this.isInit)
		{
			Cheat.Instance = this;
			this.isInit = true;
			for (int i = 0; i < this.HistroyCount; i++)
			{
				string @string = PlayerPrefs.GetString("CHEAT_HISTROY_" + i, null);
				if (@string != null && @string.Length != 0)
				{
					this.mHistory.Enqueue(@string);
				}
			}
		}
	}

	private void Update()
	{
		if (this.showDebugLog)
		{
			this.UpdateMessages();
		}
	}

	private void OnGUI()
	{
		if (this.buttonStyle == null)
		{
			this.buttonStyle = new GUIStyle(GUI.skin.button);
			this.labelStyle = new GUIStyle(GUI.skin.label);
			this.textFieldStyle = new GUIStyle(GUI.skin.textField);
			this.ButtonSize = Math.Max(Screen.width / 40, 40);
			this.buttonStyle.fontSize = this.ButtonSize - 10;
			this.labelStyle.fontSize = this.ButtonSize - 10;
			this.textFieldStyle.fontSize = this.ButtonSize - 10;
		}
		GUILayout.BeginArea(new Rect(75f, 75f, (float)(Screen.width - 100), 250f));
		if (!this.mIsShown && GUILayout.Button("Cheat", this.buttonStyle, new GUILayoutOption[]
		{
			GUILayout.ExpandWidth(false)
		}))
		{
			this.mIsShown = !this.mIsShown;
		}
		if (this.mIsShown)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			this.mCheatCode = GUILayout.TextField(this.mCheatCode, this.textFieldStyle, new GUILayoutOption[]
			{
				GUILayout.Height((float)this.ButtonSize),
				GUILayout.Width(200f)
			});
			if (GUILayout.Button("Enter", this.buttonStyle, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			}))
			{
				this.Apply();
			}
			if (Application.isEditor && Event.current.isKey && Event.current.keyCode == KeyCode.Return)
			{
				this.Apply();
			}
			string[] array = this.mHistory.ToArray();
			if (array.Length > 0)
			{
				GUILayout.Label(" | ", this.labelStyle, new GUILayoutOption[]
				{
					GUILayout.Height((float)this.ButtonSize),
					GUILayout.Width(10f)
				});
			}
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[array.Length - i - 1];
				if (text != null && text.Length != 0)
				{
					if (GUILayout.Button(text, this.buttonStyle, new GUILayoutOption[]
					{
						GUILayout.ExpandWidth(false)
					}))
					{
						this.mCheatCode = text;
						this.Apply();
					}
				}
			}
			GUILayout.EndHorizontal();
		}
		GUILayout.EndArea();
		if (this.showDebugLog)
		{
			this.ShowDebugLog();
		}
	}

	public void Execute(string code, bool fromServer = false)
	{
		this.mCheatCode = code;
		Type[] types = new Type[]
		{
			typeof(string[])
		};
		string[] array = code.Split(new char[]
		{
			' '
		});
		int num;
		if (int.TryParse(array[0], out num) && Enum.IsDefined(typeof(Cheat.GMInstruction), num))
		{
			array[0] = ((Cheat.GMInstruction)num).ToString();
		}
		BindingFlags bindingAttr = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
		MethodInfo method = typeof(Cheat).GetMethod(array[0], bindingAttr, null, types, null);
		if (method != null)
		{
			object[] parameters = new object[]
			{
				array
			};
			method.Invoke(this, parameters);
		}
		else if (!fromServer && NetWorkHelper.Instance.client != null)
		{
			NetWorkHelper.Instance.client.SendPvpServerMsg(PvpCode.C2P_GMCheat, new object[]
			{
				SerializeHelper.Serialize<CheatInfo>(new CheatInfo
				{
					cheatMsg = code
				})
			});
		}
		this.mCheatCode = string.Empty;
	}

	private void Apply()
	{
		if (this.mCheatCode == null || this.mCheatCode.Length == 0)
		{
			return;
		}
		if (!this.mHistory.Contains(this.mCheatCode))
		{
			if (this.mHistory.Count == this.HistroyCount)
			{
				this.mHistory.Dequeue();
			}
			this.mHistory.Enqueue(this.mCheatCode);
		}
		this.Execute(this.mCheatCode, false);
		this.mIsShown = false;
	}

	protected void OnApplicationQuit()
	{
		int num = 0;
		while (this.mHistory.Count != 0)
		{
			PlayerPrefs.SetString("CHEAT_HISTROY_" + num, this.mHistory.Dequeue());
			num++;
		}
	}

	public void testout(string[] param)
	{
		Debug.LogError(param[0] + " : " + param[1]);
	}

	public static void Msg(params object[] output)
	{
		for (int i = 0; i < output.Length; i++)
		{
			if (output[i] != null)
			{
				Cheat.mCacheString.Append(output[i]);
			}
			else
			{
				Cheat.mCacheString.Append("null");
			}
		}
		Cheat.Msg(Color.white, Cheat.mCacheString.ToString());
		Cheat.mCacheString.Length = 0;
	}

	public static void Msg(string output)
	{
		Cheat.Msg(Color.white, output);
	}

	public static void Msg(Color c, string output)
	{
		Cheat.messages.Add(output);
		Cheat.times.Add(Cheat.messageTime);
		Cheat.colors.Add(c);
		Cheat.lastTime = Cheat.scrollTime;
	}

	public static void AddWatch(object obj, string fieldName, string showName = null)
	{
		InfoValue infoValue = new InfoValue(obj, fieldName);
		if (showName != null)
		{
			infoValue.showName = showName;
		}
		if (Cheat.infos.Contains(infoValue))
		{
			return;
		}
		Cheat.infos.Add(infoValue);
	}

	public static void RemoveWatch(object obj, string fieldName)
	{
		for (int i = 0; i < Cheat.infos.Count; i++)
		{
			InfoValue infoValue = Cheat.infos[i] as InfoValue;
			if (infoValue.obj == obj && infoValue.name == fieldName)
			{
				Cheat.infos.RemoveAt(i);
				return;
			}
		}
	}

	private void UpdateMessages()
	{
		float deltaTime = Time.deltaTime;
		for (int i = 0; i < Cheat.times.Count; i++)
		{
			Cheat.times[i] = (float)Cheat.times[i] - deltaTime;
		}
		while (Cheat.times.Count > 0 && (float)Cheat.times[0] < 0f)
		{
			Cheat.times.RemoveAt(0);
			Cheat.messages.RemoveAt(0);
		}
		Cheat.lastTime -= deltaTime;
		if (Cheat.lastTime < 0f)
		{
			Cheat.lastTime = 0f;
		}
	}

	private void ShowDebugLog()
	{
		GUI.depth = -10000;
		int count = Cheat.messages.Count;
		int[] array = new int[count];
		int num = 0;
		for (int i = 0; i < count; i++)
		{
			int num2 = 0;
			int num3 = 1;
			while (true)
			{
				string text = Cheat.messages[i] as string;
				num2 = text.IndexOf('\n', num2);
				if (num2 == -1)
				{
					break;
				}
				num3++;
				num2++;
			}
			int num4 = num3 * 20;
			array[i] = num4;
			num += num4;
		}
		Rect position = new Rect(2f, (float)(Screen.height - 2 - num) + Cheat.lastTime / Cheat.scrollTime * (float)(num - Cheat.lastHight) - 20f, 600f, 20f);
		Cheat.lastHight = num;
		for (int j = 0; j < count; j++)
		{
			string text2 = (string)Cheat.messages[j];
			float num5 = Cheat.messageTime - (float)Cheat.times[j];
			Color color = (Color)Cheat.colors[j];
			if (num5 < Cheat.fadeTime)
			{
				GUI.color = new Color(color.r, color.g, color.b, num5 / Cheat.fadeTime);
			}
			else if (num5 >= Cheat.fadeOutTime)
			{
				GUI.color = new Color(color.r, color.g, color.b, 1f - (num5 - Cheat.fadeOutTime) / Cheat.fadeTime);
			}
			else
			{
				GUI.color = new Color(color.r, color.g, color.b, 1f);
			}
			position.height = (float)array[j];
			GUI.Label(position, text2);
			position.y += (float)array[j];
		}
		if (Cheat.infos.Count != 0)
		{
			GUI.color = Color.white;
			GUI.contentColor = Color.yellow;
			Cheat.ListWinRect = GUI.Window(9999, Cheat.ListWinRect, new GUI.WindowFunction(this.ShowInfoList), "value info");
		}
	}

	private void ShowInfoList(int id)
	{
		GUI.color = Color.white;
		for (int i = 0; i < Cheat.infos.Count; i++)
		{
			InfoValue infoValue = Cheat.infos[i] as InfoValue;
			if (infoValue != null)
			{
				GUILayout.Label(infoValue.showName + ":  " + infoValue.Get().ToString(), new GUILayoutOption[0]);
			}
		}
		GUI.DragWindow();
	}

	private void _setFieldValue(FieldInfo field, object obj, string value)
	{
		if (field == null || obj == null)
		{
			return;
		}
		Type fieldType = field.FieldType;
		if (fieldType == typeof(int))
		{
			field.SetValue(obj, int.Parse(value));
		}
		else if (fieldType == typeof(float))
		{
			field.SetValue(obj, float.Parse(value));
		}
		else if (fieldType == typeof(bool))
		{
			field.SetValue(obj, value.Equals("true", StringComparison.OrdinalIgnoreCase) || value == "1");
		}
		else if (fieldType == typeof(string))
		{
			field.SetValue(obj, value);
		}
		else if (fieldType == typeof(double))
		{
			field.SetValue(obj, double.Parse(value));
		}
	}

	private void gs(string[] param)
	{
		BindingFlags bindingAttr = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
		FieldInfo field = typeof(GlobalSettings).GetField(param[1], bindingAttr);
		if (field != null)
		{
			this._setFieldValue(field, GlobalSettings.Instance, param[2]);
			return;
		}
		FieldInfo[] fields = typeof(GlobalSettings).GetFields(bindingAttr);
		FieldInfo[] array = fields;
		for (int i = 0; i < array.Length; i++)
		{
			FieldInfo fieldInfo = array[i];
			Type fieldType = fieldInfo.FieldType;
			if (fieldType.IsClass)
			{
				FieldInfo field2 = fieldType.GetField(param[1], bindingAttr);
				if (field2 != null)
				{
					object value = fieldInfo.GetValue(GlobalSettings.Instance);
					this._setFieldValue(field2, value, param[2]);
					Cheat.Msg(new object[]
					{
						"GlobalSettings.SetField: ",
						param[1],
						" = ",
						param[2]
					});
					return;
				}
			}
		}
	}

	private void logpath(string[] param)
	{
		Cheat.Msg(Application.persistentDataPath);
	}

	private void showlog(string[] param)
	{
		DLog.GameLog.UseScreenLogOut = (param.Length == 1 || param[1].Equals("true", StringComparison.OrdinalIgnoreCase) || param[1] == "1");
	}

	private void log(string[] param)
	{
		DebugView.Open();
	}

	private void logs(string[] param)
	{
		DebugView.PageSize = int.Parse(param[1]);
		DebugView.ShowStacktrace = (param[2] == "1");
		DebugView.Instance.Refresh();
	}

	private void test333(string[] param)
	{
		if (param.Length > 1)
		{
			GlobalSettings.Instance.PvpSetting.test3v3 = (param[1] == "1");
		}
	}

	private void setFogMode(string[] param)
	{
		if (param.Length > 1)
		{
			GlobalSettings.SetFogMode(int.Parse(param[1]));
		}
	}

	private void openrshadow(string[] param)
	{
		QualitySettings.SetQualityLevel(6);
	}

	private void closershadow(string[] param)
	{
		QualitySettings.SetQualityLevel(7);
	}

	private void showfsd(string[] param)
	{
		GlobalSettings.setfakeshadow(true);
	}

	private void closefsd(string[] param)
	{
		GlobalSettings.setfakeshadow(false);
	}

	private void opencob(string[] param)
	{
	}

	private void closecb(string[] param)
	{
	}

	public static void MakeActive(string path, bool active)
	{
		int num = path.IndexOf('/');
		if (num == -1)
		{
			return;
		}
		string text = path.Substring(0, num);
		GameObject gameObject = GameObject.Find(text);
		if (!gameObject)
		{
			Debug.LogError("cannot find root " + text);
			return;
		}
		string text2 = path.Substring(num + 1);
		Transform transform = gameObject.transform.Find(text2);
		if (!transform)
		{
			Debug.LogError("cannot find root " + text2);
			return;
		}
		transform.gameObject.SetActive(active);
	}

	private void gon(string[] param)
	{
		if (param.Length > 1)
		{
			Cheat.MakeActive(param[1], true);
		}
	}

	private void goff(string[] param)
	{
		if (param.Length > 1)
		{
			Cheat.MakeActive(param[1], false);
		}
	}

	private void closeUICamera(string[] param)
	{
		GameObject gameObject = GameObject.Find("ViewRoot");
		Camera[] componentsInChildren = gameObject.transform.GetComponentsInChildren<Camera>();
		Camera[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			Camera camera = array[i];
			camera.enabled = false;
		}
	}

	private void closeView(string[] param)
	{
		CtrlManager.CloseWindow(WindowID.MiniMapView);
		CtrlManager.CloseWindow(WindowID.SkillView);
		CtrlManager.CloseWindow(WindowID.ShowEquipmentPanelView);
		CtrlManager.CloseWindow(WindowID.CharacterView);
	}

	private void openView(string[] param)
	{
		CtrlManager.OpenWindow(WindowID.MiniMapView, null);
		CtrlManager.OpenWindow(WindowID.SkillView, null);
		CtrlManager.OpenWindow(WindowID.ShowEquipmentPanelView, null);
		CtrlManager.OpenWindow(WindowID.CharacterView, null);
	}

	private void openUICamera(string[] param)
	{
		GameObject gameObject = GameObject.Find("ViewRoot");
		Camera[] componentsInChildren = gameObject.transform.GetComponentsInChildren<Camera>();
		Camera[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			Camera camera = array[i];
			camera.enabled = true;
		}
	}

	private void testAddwatch(string[] param)
	{
		Units[] array = UnityEngine.Object.FindObjectsOfType<Units>();
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].moveController != null && array[i].DebugDLog)
			{
				Cheat.AddWatch(array[i].moveController, "moveState", array[i].npc_id);
			}
		}
	}

	private void ShowFPS(string[] param)
	{
		GoldViewFps goldViewFps = UnityEngine.Object.FindObjectOfType<GoldViewFps>();
		if (goldViewFps != null)
		{
			Cheat.AddWatch(goldViewFps, "fps", null);
		}
	}

	private void setfps(string[] param)
	{
		if (param.Length > 1)
		{
			Application.targetFrameRate = int.Parse(param[1]);
		}
		if (param.Length > 2)
		{
			QualitySettings.vSyncCount = int.Parse(param[2]);
		}
	}

	private void crash(string[] param)
	{
		this.gs(null);
	}

	private void testHpbar(string[] param)
	{
	}

	private void pvpdebugopen(string[] param)
	{
		GlobalSettings.Instance.PvpSetting.isDebugSpeed = true;
		if (Singleton<HUDModuleManager>.Instance.gameObject)
		{
			Singleton<HUDModuleManager>.Instance.OpenModule(EHUDModule.Debug, 0);
		}
	}

	private void pvpdebugclose(string[] param)
	{
		GlobalSettings.Instance.PvpSetting.isDebugSpeed = false;
		if (Singleton<HUDModuleManager>.Instance.gameObject)
		{
			Singleton<HUDModuleManager>.Instance.CloseModule(EHUDModule.Debug, false);
		}
	}

	private void mutevoice(string[] param)
	{
		AudioMgr.Instance.MuteVoice();
	}

	private void unmutevoice(string[] param)
	{
		AudioMgr.Instance.UnMuteVoice();
	}

	private void P2C_DebugSpeed(string[] param)
	{
		Singleton<GoldView>.Instance.SetPvpSpeed(float.Parse(param[1]));
		if (Singleton<HUDModuleManager>.Instance.gameObject)
		{
			DebugModule module = Singleton<HUDModuleManager>.Instance.GetModule<DebugModule>(EHUDModule.Debug);
			if (module != null)
			{
				module.SetPvpSpeed(float.Parse(param[1]));
			}
		}
	}

	private void AddBlock(string[] param)
	{
		int x = int.Parse(param[1]);
		int y = int.Parse(param[2]);
		BlockDebugInfo.add(x, y);
	}

	private void RemoveBlock(string[] param)
	{
		int x = int.Parse(param[1]);
		int y = int.Parse(param[2]);
		BlockDebugInfo.remove(x, y);
	}

	private void ShowMoveDelay(string[] param)
	{
		MoveController.ShowMoveDelay = true;
	}

	private void SetMoveDelay(string[] param)
	{
		if (param.Length > 2)
		{
			float speed = (param.Length <= 3) ? 0f : float.Parse(param[3]);
			FrameSyncManager.Instance.setDelayTime(float.Parse(param[1]), float.Parse(param[2]), speed);
			Cheat.Msg(this.mCheatCode);
		}
	}

	private void turnspeed(string[] param)
	{
		MoveController.TurnSpeed = float.Parse(param[1]);
	}

	private void ShowSceneEditor(string[] param)
	{
		if (Singleton<UISceneEditor>.Instance.IsOpened)
		{
			CtrlManager.CloseWindow(WindowID.SceneEditor);
		}
		else
		{
			CtrlManager.OpenWindow(WindowID.SceneEditor, null);
		}
	}

	private void SetQualityLevel(string[] param)
	{
		QualitySettings.SetQualityLevel(int.Parse(param[1]));
	}

	private void ShowAFPSCount(string[] param)
	{
		byte b;
		if (byte.TryParse(param[1], out b) && Enum.IsDefined(typeof(AFPSCounterOperationMode), b))
		{
			AFPSCounter.Instance.OperationMode = (AFPSCounterOperationMode)b;
			AFPSCounter.Instance.deviceInfoCounter.Enabled = true;
			AFPSCounter.Instance.AnchorsOffset = new Vector2(5f, 116f);
		}
	}

	private void Adapt(string[] param)
	{
		AdaptCenter.Instance.adaptOn = (int.Parse(param[1]) > 0);
	}

	private void setDPI(string[] param)
	{
		Cheat.Msg(string.Concat(new object[]
		{
			"Screen: ",
			Screen.width,
			" ",
			Screen.height,
			" ",
			Screen.currentResolution.refreshRate
		}));
		if (param.Length > 3)
		{
			Screen.SetResolution(int.Parse(param[1]), int.Parse(param[2]), true, int.Parse(param[3]));
		}
		else
		{
			Screen.SetResolution(int.Parse(param[1]), int.Parse(param[2]), true);
		}
		Cheat.Msg(string.Concat(new object[]
		{
			"To: ",
			Screen.width,
			" ",
			Screen.height,
			" ",
			Screen.height,
			" ",
			Screen.currentResolution.refreshRate
		}));
	}

	private void sq(string[] param)
	{
		if (param.Length > 1)
		{
			QualitySettings.SetQualityLevel(int.Parse(param[1]));
		}
	}

	private void EnableObject(string[] param)
	{
		if (int.Parse(param[2]) == 0)
		{
			GameObject gameObject = GameObject.Find(param[1]);
			if (gameObject != null)
			{
				this.TestObjs.Add(param[1], gameObject);
				gameObject.SetActive(false);
			}
		}
		else
		{
			GameObject gameObject2 = null;
			if (this.TestObjs.TryGetValue(param[1], out gameObject2))
			{
				gameObject2.SetActive(true);
				this.TestObjs.Remove(param[1]);
			}
		}
	}

	private void testMove(string[] param)
	{
		MoveController.TestSmoothMove = (int.Parse(param[1]) > 0);
	}

	private void testMemory(string[] param)
	{
		Cheat.Msg(new object[]
		{
			"TotalAllocatedMemory: ",
			Profiler.GetTotalAllocatedMemory()
		});
		Cheat.Msg(new object[]
		{
			"TotalReservedMemory: ",
			Profiler.GetTotalReservedMemory()
		});
		Cheat.Msg(new object[]
		{
			"TotalUnusedReservedMemory: ",
			Profiler.GetTotalUnusedReservedMemory()
		});
	}

	private void showDPI(string[] param)
	{
		Cheat.Msg(new object[]
		{
			"curDPI: ",
			Screen.dpi
		});
	}

	private void CPUPerf(string[] param)
	{
		CPUPerf cPUPerf = UnityEngine.Object.FindObjectOfType<CPUPerf>();
		if (cPUPerf != null)
		{
			cPUPerf.mUpdateInterval = int.Parse(param[1]);
			cPUPerf.mDumpFPSLimit = int.Parse(param[2]);
		}
	}

	private void spl(string[] param)
	{
		if (param.Length > 1)
		{
			int oldLevel = int.Parse(param[1]);
			GlobalSettings.Instance.QualitySetting.OldLevel = oldLevel;
		}
	}

	private void sl(string[] param)
	{
		if (param.Length > 1)
		{
			int num = int.Parse(param[1]);
			if (num >= 0 && num <= 3)
			{
				GlobalSettings.Instance.QualitySetting.SetLevel((MobaQualityLevel)num);
			}
		}
	}

	private void anti(string[] param)
	{
		QualitySettings.antiAliasing = int.Parse(param[1]);
	}

	private void surrender(string[] param)
	{
		GameManager.Instance.SurrenderMgr.StartSurrender();
	}

	private void profile(string[] param)
	{
		Cheat._profileBegin = !Cheat._profileBegin;
		if (Cheat._profileBegin)
		{
			ProfilerHelper.Begin();
		}
		else
		{
			ProfilerHelper.End();
		}
	}

	private void movewaitframe(string[] param)
	{
		FrameSyncManager.Instance.WaitFrameTime = (int.Parse(param[1]) > 0);
	}
}
