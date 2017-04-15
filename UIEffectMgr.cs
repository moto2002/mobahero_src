using Assets.Scripts.Server;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UIEffectMgr : IGlobalComServer
{
	private class EffectItem
	{
		public eUIEffectType effectType;

		public GameObject effectObj;

		public int nID;
	}

	private Dictionary<eUIEffectType, GameObject> m_uiEffectPrefabDict = new Dictionary<eUIEffectType, GameObject>();

	private List<UIEffectMgr.EffectItem> m_effectItemList = new List<UIEffectMgr.EffectItem>();

	private static UIEffectMgr m_instance;

	public static UIEffectMgr Instance
	{
		get
		{
			if (UIEffectMgr.m_instance == null)
			{
				throw new Exception("UIEffectMgr script is missing");
			}
			return UIEffectMgr.m_instance;
		}
	}

	void IGlobalComServer.OnDestroy()
	{
		UIEffectMgr.m_instance = null;
	}

	public void OnAwake()
	{
		UIEffectMgr.m_instance = this;
	}

	public void OnStart()
	{
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

	public void ClearEffect()
	{
		List<eUIEffectType> list = new List<eUIEffectType>(this.m_uiEffectPrefabDict.Keys);
		for (int i = list.Count - 1; i >= 0; i--)
		{
			UnityEngine.Object.Destroy(this.m_uiEffectPrefabDict[list[i]]);
			this.m_uiEffectPrefabDict[list[i]] = null;
		}
		this.m_uiEffectPrefabDict.Clear();
		for (int j = this.m_effectItemList.Count - 1; j >= 0; j--)
		{
			UnityEngine.Object.Destroy(this.m_effectItemList[j].effectObj);
			this.m_effectItemList[j].effectObj = null;
		}
		this.m_effectItemList.Clear();
	}

	private GameObject LoadEffectPrefab(eUIEffectType effectType)
	{
		if (!this.m_uiEffectPrefabDict.ContainsKey(effectType))
		{
			GameObject value = ResourceManager.Load<GameObject>(effectType.ToString(), true, true, null, 0, false);
			this.m_uiEffectPrefabDict.Add(effectType, value);
		}
		return this.m_uiEffectPrefabDict[effectType];
	}

	private int CheckEffectInList(eUIEffectType effectType, int id)
	{
		for (int i = 0; i < this.m_effectItemList.Count; i++)
		{
			if (this.m_effectItemList[i].effectType == effectType && this.m_effectItemList[i].nID == id)
			{
				return i;
			}
		}
		return -1;
	}

	public GameObject PlayEffect(eUIEffectType effectType, Transform targetRoot, float scale = 0f, int id = 0)
	{
		if (this.CheckEffectInList(effectType, id) != -1)
		{
			return null;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(this.LoadEffectPrefab(effectType)) as GameObject;
		Vector3 localScale = gameObject.transform.localScale;
		gameObject.transform.parent = targetRoot;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localEulerAngles = new Vector3(340f, 0f, 0f);
		if (scale == 0f)
		{
			gameObject.transform.localScale = localScale;
		}
		else
		{
			gameObject.transform.localScale = Vector3.one * scale;
		}
		UIEffectMgr.EffectItem effectItem = new UIEffectMgr.EffectItem();
		effectItem.effectType = effectType;
		effectItem.effectObj = gameObject;
		effectItem.nID = id;
		this.m_effectItemList.Add(effectItem);
		return gameObject;
	}

	public void StopEffect(eUIEffectType effectType, int id = 0)
	{
		int num = this.CheckEffectInList(effectType, id);
		if (num != -1)
		{
			UnityEngine.Object.Destroy(this.m_effectItemList[num].effectObj);
			this.m_effectItemList[num].effectObj = null;
			this.m_effectItemList.RemoveAt(num);
		}
	}
}
