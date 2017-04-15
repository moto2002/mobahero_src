using Com.Game.Utils;
using LogUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class DebugView : MonoBehaviour
{
	[Serializable]
	private class Settings
	{
		public readonly List<string> Names = new List<string>();

		public bool ShowStackTrace;

		public int PageSize = 40;
	}

	public GameObject setting;

	public GameObject view;

	public UIGrid settingGrid;

	public UILabel logContent;

	public UILabel logStats;

	public UILabel PageSizeInput;

	public UIToggle ShowStackTrace;

	private int _curPage;

	private static readonly MemoryLogger _logger = new MemoryLogger(true, 1000);

	private static DebugView _instance;

	private List<MemoryLogger.Entry> _entryList;

	private static DebugView.Settings _settings = new DebugView.Settings();

	public static int PageSize
	{
		set
		{
			DebugView._settings.PageSize = value;
		}
	}

	public static bool ShowStacktrace
	{
		set
		{
			DebugView._settings.ShowStackTrace = value;
		}
	}

	public static DebugView Instance
	{
		get
		{
			return DebugView._instance;
		}
	}

	public static void InitLogger()
	{
		ClientLogger.Logger = new MultiLogger(new ILogger[]
		{
			new ConsoleLogger(),
			DebugView._logger
		});
	}

	public static void Open()
	{
		if (DebugView._instance == null)
		{
			DebugView original = ResourceManager.LoadPath<DebugView>("Prefab/UI/Common/DebugView", null, 0);
			DebugView debugView = UnityEngine.Object.Instantiate(original) as DebugView;
			GameObject gameObject = GameObject.Find("ViewRoot/Camera");
			debugView.transform.parent = gameObject.transform;
			debugView.transform.localPosition = Vector3.zero;
			debugView.transform.localScale = Vector3.one;
		}
		DebugView._instance.gameObject.SetActive(true);
	}

	private void Awake()
	{
		DebugView._instance = this;
	}

	private void OnEnable()
	{
		this.LoadSettings();
		this.Refresh();
	}

	public void OnClose()
	{
		if (this.setting.activeInHierarchy)
		{
			this.view.SetActive(true);
			this.setting.SetActive(false);
			this.SaveSettings();
		}
		else
		{
			base.gameObject.SetActive(false);
		}
	}

	private void SaveSettings()
	{
		DebugView._settings.ShowStackTrace = this.ShowStackTrace.value;
		DebugView._settings.PageSize = int.Parse(this.PageSizeInput.text);
		DebugView._settings.Names.Clear();
		foreach (string current in LogTagMgr.Instance.TagNames)
		{
			if (LogTagMgr.Instance[current].Enable)
			{
				DebugView._settings.Names.Add(current);
			}
		}
		string value = MiniJSON.jsonEncode(DebugView._settings);
		PlayerPrefs.SetString("cosmore_log", value);
	}

	private void LoadSettings()
	{
		string @string = PlayerPrefs.GetString("cosmore_log", string.Empty);
		if (@string == string.Empty)
		{
			return;
		}
		DebugView.Settings settings = (DebugView.Settings)MiniJSON.jsonDecode(@string);
		if (settings != null)
		{
			DebugView._settings = settings;
		}
		foreach (string current in DebugView._settings.Names)
		{
			LogTagMgr.Instance.EnsureTagExists(current);
			LogTagMgr.Instance[current].Enable = true;
		}
		this.ShowStackTrace.value = DebugView._settings.ShowStackTrace;
		this.PageSizeInput.text = DebugView._settings.PageSize.ToString();
	}

	public void OnSetting()
	{
		if (!this.setting.activeInHierarchy)
		{
			this.view.SetActive(false);
			this.setting.SetActive(true);
			this.BindSettings();
		}
	}

	private void BindSettings()
	{
		List<string> names = LogTagMgr.Instance.TagNames.ToList<string>();
		GridHelper.FillGridWithFirstChild<Component>(this.settingGrid, names.Count, delegate(int idx, Component cmp)
		{
			string name = names[idx];
			cmp.transform.Find("Label").GetComponent<UILabel>().text = name;
			LogTagMgr.TagInfo tagInfo = LogTagMgr.Instance[name];
			UISprite sprite = cmp.transform.Find("check").GetComponent<UISprite>();
			if (tagInfo.Enable)
			{
				sprite.spriteName = "Icon_add";
			}
			else
			{
				sprite.spriteName = "Btn_close_n";
			}
			UIEventListener.Get(sprite.gameObject).onClick = delegate(GameObject go)
			{
				LogTagMgr.TagInfo tagInfo2 = LogTagMgr.Instance[name];
				tagInfo2.Enable = !tagInfo2.Enable;
				if (tagInfo2.Enable)
				{
					sprite.spriteName = "Icon_add";
				}
				else
				{
					sprite.spriteName = "Btn_close_n";
				}
			};
		});
	}

	public void Refresh()
	{
		if (!DebugView._instance || !DebugView._instance.gameObject.activeInHierarchy)
		{
			return;
		}
		int pageSize = DebugView._settings.PageSize;
		this._entryList = DebugView._logger.Entries.ToList<MemoryLogger.Entry>();
		this.logStats.text = this._curPage + "/" + (this._entryList.Count + pageSize - 1) / pageSize;
		StringBuilder stringBuilder = new StringBuilder(10240);
		for (int i = this._curPage * pageSize; i < Mathf.Min(this._entryList.Count, this._curPage * pageSize + pageSize); i++)
		{
			MemoryLogger.Entry entry = this._entryList[i];
			if (DebugView._settings.ShowStackTrace)
			{
				stringBuilder.AppendFormat("[{0}] {1} {2} {3}\n", new object[]
				{
					entry.Prefix,
					entry.Time,
					entry.Message,
					entry.StackTrace
				});
			}
			else
			{
				stringBuilder.AppendFormat("[{0}] {1} {2}", entry.Prefix, entry.Time, entry.Message);
			}
			stringBuilder.AppendLine();
		}
		this.logContent.text = stringBuilder.ToString();
	}

	public void PageUp()
	{
		if (this._curPage > 0)
		{
			this._curPage--;
			this.Refresh();
		}
	}

	public void PageDown()
	{
		if (this._entryList == null)
		{
			return;
		}
		int pageSize = DebugView._settings.PageSize;
		int num = (this._entryList.Count + pageSize - 1) / pageSize;
		if (this._curPage < num - 1)
		{
			this._curPage++;
			this.Refresh();
		}
	}

	public void OnToggle()
	{
		this.ShowStackTrace.value = !this.ShowStackTrace.value;
	}
}
