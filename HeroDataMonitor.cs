using MobaHeros;
using System;
using System.Collections.Generic;
using UnityEngine;

public class HeroDataMonitor : MonoBehaviour
{
	private const int START_IDX = 6;

	private const int END_IDX = 25;

	public UIGrid grid;

	public GameObject sample;

	public new UILabel name;

	public GameObject anchor;

	private static HeroDataMonitor _instance;

	private int _curHeroId = -1;

	private List<int> _heroIds = new List<int>();

	public Dictionary<int, DataPair> _allPairs = new Dictionary<int, DataPair>();

	public Dictionary<int, Dictionary<int, float>> _allHeroValues = new Dictionary<int, Dictionary<int, float>>();

	private int _curIdx;

	public static HeroDataMonitor Instance
	{
		get
		{
			return HeroDataMonitor._instance;
		}
	}

	private void Awake()
	{
		HeroDataMonitor._instance = this;
	}

	private void ResetAllData()
	{
		this.ClearObjs();
		this._curIdx = 0;
		this._heroIds.Clear();
		this._allHeroValues.Clear();
		this.name.text = string.Empty;
	}

	private void ClearObjs()
	{
		if (this._allPairs.Count > 0)
		{
			foreach (KeyValuePair<int, DataPair> current in this._allPairs)
			{
				UnityEngine.Object.Destroy(current.Value.gameObject);
			}
		}
		this._allPairs.Clear();
	}

	public void InitData(Dictionary<int, Dictionary<int, float>> _data, List<int> hero_id)
	{
		this.ResetAllData();
		this._allHeroValues = _data;
		this._heroIds = hero_id;
		this._curIdx = 0;
		this._curHeroId = this._heroIds[0];
		this.InitPanel(this._allHeroValues[this._curHeroId]);
	}

	public void OnNext()
	{
		this._curIdx++;
		if (this._curIdx >= this._heroIds.Count)
		{
			this._curIdx = 0;
		}
		this._curHeroId = this._heroIds[this._curIdx];
		this.RebindAll();
	}

	private void RebindAll()
	{
		Dictionary<int, float> dictionary = this._allHeroValues[this._curHeroId];
		foreach (KeyValuePair<int, float> current in dictionary)
		{
			this._allPairs[current.Key].SetContent(((AttrType)current.Key).ToString(), current.Value);
		}
		this.SetName();
	}

	private void SetName()
	{
		Units unit = MapManager.Instance.GetUnit(this._curHeroId);
		if (unit != null)
		{
			this.name.text = unit.npc_id;
		}
	}

	private void InitPanel(Dictionary<int, float> curData)
	{
		this.SetName();
		foreach (KeyValuePair<int, float> current in curData)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(this.sample) as GameObject;
			DataPair component = gameObject.GetComponent<DataPair>();
			gameObject.transform.parent = this.sample.transform.parent;
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.localPosition = Vector3.zero;
			component.SetContent(((AttrType)current.Key).ToString(), current.Value);
			gameObject.SetActive(true);
			this._allPairs.Add(current.Key, component);
		}
		this.grid.Reposition();
	}

	public void OnAttrChange(int type, object newVal, object changeVal, int heroId)
	{
		if (this._allHeroValues.ContainsKey(heroId))
		{
			Dictionary<int, float> dictionary = this._allHeroValues[heroId];
			if (dictionary.ContainsKey(type))
			{
				dictionary[type] = (float)newVal;
			}
			else
			{
				dictionary.Add(type, (float)newVal);
			}
			if (this._allPairs.ContainsKey(type) && this._curHeroId == heroId)
			{
				this._allPairs[type].SetContent(((AttrType)type).ToString(), (float)newVal);
			}
		}
	}
}
