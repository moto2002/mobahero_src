using System;
using UnityEngine;

public class FxmTestMain : MonoBehaviour
{
	public static FxmTestMain inst;

	public GUISkin m_GuiMainSkin;

	public FxmTestMouse m_FXMakerMouse;

	public FxmTestControls m_FXMakerControls;

	public AnimationCurve m_SimulateArcCurve;

	public GameObject m_GroupList;

	public int m_CurrentGroupIndex;

	public GameObject m_PrefabList;

	public int m_CurrentPrefabIndex;

	public bool m_bAutoChange = true;

	public bool m_bAutoSetting = true;

	protected GameObject m_OriginalEffectObject;

	protected GameObject m_InstanceEffectObject;

	private FxmTestMain()
	{
		FxmTestMain.inst = this;
	}

	public FxmTestMouse GetFXMakerMouse()
	{
		if (this.m_FXMakerMouse == null)
		{
			this.m_FXMakerMouse = base.GetComponentInChildren<FxmTestMouse>();
		}
		return this.m_FXMakerMouse;
	}

	public FxmTestControls GetFXMakerControls()
	{
		if (this.m_FXMakerControls == null)
		{
			this.m_FXMakerControls = base.GetComponent<FxmTestControls>();
		}
		return this.m_FXMakerControls;
	}

	private void Awake()
	{
		NgUtil.LogDevelop("Awake - FXMakerMain");
		this.GetFXMakerControls().enabled = true;
	}

	private void OnEnable()
	{
		NgUtil.LogDevelop("OnEnable - FXMakerMain");
	}

	private void Start()
	{
		if (0 < this.m_GroupList.transform.childCount)
		{
			this.m_PrefabList = this.m_GroupList.transform.GetChild(0).gameObject;
		}
		if (this.m_PrefabList != null && 0 < this.m_PrefabList.transform.childCount)
		{
			this.m_OriginalEffectObject = this.m_PrefabList.transform.GetChild(0).gameObject;
			this.CreateCurrentInstanceEffect(true);
		}
	}

	private void Update()
	{
	}

	public void OnGUI()
	{
		GUI.skin = this.m_GuiMainSkin;
		float num = (float)(Screen.width / 7);
		float num2 = (float)(Screen.height / 10);
		this.m_FXMakerControls.OnGUIControl();
		if (GUI.Button(new Rect(0f, 0f, num, num2), "GPrev"))
		{
			this.ChangeGroup(false);
		}
		if (GUI.Button(new Rect(num + 10f, 0f, num, num2), "GNext"))
		{
			this.ChangeGroup(true);
		}
		GUI.Box(new Rect(0f, num2 + 10f, num * 2f + 10f, 20f), this.m_GroupList.transform.GetChild(this.m_CurrentGroupIndex).name, GUI.skin.FindStyle("Hierarchy_Button"));
		if (GUI.Button(new Rect((float)Screen.width - num * 2f - 10f, 0f, num, num2), "EPrev"))
		{
			this.ChangeEffect(false);
		}
		if (GUI.Button(new Rect((float)Screen.width - num, 0f, num, num2), "ENext"))
		{
			this.ChangeEffect(true);
		}
		this.m_bAutoChange = GUI.Toggle(new Rect((float)Screen.width - num, num2 + 10f, num, 20f), this.m_bAutoChange, "AutoChange");
		bool flag = GUI.Toggle(new Rect((float)Screen.width - num * 2f - 10f, num2 + 10f, num, 20f), this.m_bAutoSetting, "AutoSetting");
		if (flag != this.m_bAutoSetting)
		{
			this.m_bAutoSetting = flag;
			if (!flag)
			{
				this.m_FXMakerControls.SetDefaultSetting();
			}
		}
		float num3 = GUI.VerticalSlider(new Rect(10f, num2 + 10f + 30f, 25f, (float)Screen.height - (num2 + 10f + 50f) - this.GetFXMakerControls().GetActionToolbarRect().height), this.GetFXMakerMouse().m_fDistance, this.GetFXMakerMouse().m_fDistanceMin, this.GetFXMakerMouse().m_fDistanceMax);
		if (num3 != this.GetFXMakerMouse().m_fDistance)
		{
			this.GetFXMakerMouse().SetDistance(num3);
		}
	}

	public void ChangeEffect(bool bNext)
	{
		if (this.m_PrefabList == null)
		{
			return;
		}
		if (bNext)
		{
			if (this.m_CurrentPrefabIndex >= this.m_PrefabList.transform.childCount - 1)
			{
				this.ChangeGroup(true);
				return;
			}
			this.m_CurrentPrefabIndex++;
		}
		else
		{
			if (this.m_CurrentPrefabIndex == 0)
			{
				this.ChangeGroup(false);
				return;
			}
			this.m_CurrentPrefabIndex--;
		}
		this.m_OriginalEffectObject = this.m_PrefabList.transform.GetChild(this.m_CurrentPrefabIndex).gameObject;
		this.CreateCurrentInstanceEffect(true);
	}

	public bool ChangeGroup(bool bNext)
	{
		if (bNext)
		{
			if (this.m_CurrentGroupIndex < this.m_GroupList.transform.childCount - 1)
			{
				this.m_CurrentGroupIndex++;
			}
			else
			{
				this.m_CurrentGroupIndex = 0;
			}
		}
		else if (this.m_CurrentGroupIndex == 0)
		{
			this.m_CurrentGroupIndex = this.m_GroupList.transform.childCount - 1;
		}
		else
		{
			this.m_CurrentGroupIndex--;
		}
		this.m_PrefabList = this.m_GroupList.transform.GetChild(this.m_CurrentGroupIndex).gameObject;
		if (this.m_PrefabList != null && 0 < this.m_PrefabList.transform.childCount)
		{
			this.m_CurrentPrefabIndex = 0;
			this.m_OriginalEffectObject = this.m_PrefabList.transform.GetChild(this.m_CurrentPrefabIndex).gameObject;
			this.CreateCurrentInstanceEffect(true);
			return true;
		}
		return true;
	}

	public bool IsCurrentEffectObject()
	{
		return this.m_OriginalEffectObject != null;
	}

	public GameObject GetOriginalEffectObject()
	{
		return this.m_OriginalEffectObject;
	}

	public void ChangeRoot_OriginalEffectObject(GameObject newRoot)
	{
		this.m_OriginalEffectObject = newRoot;
	}

	public void ChangeRoot_InstanceEffectObject(GameObject newRoot)
	{
		this.m_InstanceEffectObject = newRoot;
	}

	public GameObject GetInstanceEffectObject()
	{
		return this.m_InstanceEffectObject;
	}

	public void ClearCurrentEffectObject(GameObject effectRoot, bool bClearEventObject)
	{
		if (bClearEventObject)
		{
			GameObject instanceRoot = this.GetInstanceRoot();
			if (instanceRoot != null)
			{
				NgObject.RemoveAllChildObject(instanceRoot, true);
			}
		}
		NgObject.RemoveAllChildObject(effectRoot, true);
		this.m_OriginalEffectObject = null;
		this.CreateCurrentInstanceEffect(null);
	}

	public void CreateCurrentInstanceEffect(bool bRunAction)
	{
		FxmTestSetting component = this.m_PrefabList.GetComponent<FxmTestSetting>();
		if (this.m_bAutoSetting && component != null)
		{
			this.m_FXMakerControls.AutoSetting(component.m_nPlayIndex, component.m_nTransIndex, component.m_nTransAxis, component.m_fDistPerTime, component.m_nRotateIndex, component.m_nMultiShotCount, component.m_fTransRate, component.m_fStartPosition);
		}
		NgUtil.LogDevelop("CreateCurrentInstanceEffect() - bRunAction - " + bRunAction);
		bool flag = this.CreateCurrentInstanceEffect(this.m_OriginalEffectObject);
		if (flag && bRunAction)
		{
			this.m_FXMakerControls.RunActionControl();
		}
	}

	public GameObject GetInstanceRoot()
	{
		return NcEffectBehaviour.GetRootInstanceEffect();
	}

	private bool CreateCurrentInstanceEffect(GameObject gameObj)
	{
		NgUtil.LogDevelop("CreateCurrentInstanceEffect() - gameObj - " + gameObj);
		GameObject instanceRoot = this.GetInstanceRoot();
		NgObject.RemoveAllChildObject(instanceRoot, true);
		if (gameObj != null)
		{
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(gameObj);
			NsEffectManager.PreloadResource(gameObject);
			gameObject.transform.parent = instanceRoot.transform;
			this.m_InstanceEffectObject = gameObject;
			NgObject.SetActiveRecursively(gameObject, true);
			this.m_FXMakerControls.SetStartTime();
			return true;
		}
		this.m_InstanceEffectObject = null;
		return false;
	}
}
