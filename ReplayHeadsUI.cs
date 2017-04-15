using Com.Game.Data;
using Com.Game.Manager;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ReplayHeadsUI : MonoBehaviour
{
	private const int RoleCount = 5;

	public GameObject baseHead;

	public Transform LMRoot;

	public Transform BLRoot;

	private List<HeadItem> m_lmRoleList = new List<HeadItem>();

	private List<HeadItem> m_blRoleList = new List<HeadItem>();

	private List<HeadItem> m_activeItems = new List<HeadItem>();

	public UISprite LMBg;

	public UISprite BLBg;

	private void Awake()
	{
		this.BuildHeads(this.LMRoot, ref this.m_lmRoleList);
		this.BuildHeads(this.BLRoot, ref this.m_blRoleList);
	}

	private void Update()
	{
	}

	private void BuildHeads(Transform root, ref List<HeadItem> list)
	{
		list.Clear();
		for (int i = 0; i < 5; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(this.baseHead) as GameObject;
			gameObject.transform.parent = root;
			HeadItem headItem = gameObject.GetComponent<HeadItem>();
			if (headItem == null)
			{
				headItem = gameObject.AddComponent<HeadItem>();
			}
			list.Add(headItem);
		}
	}

	public void UpdateHeads(EntityVo[] ovLM, EntityVo[] voBL)
	{
		this.m_activeItems.Clear();
		this.SetGroupItems(ovLM, ref this.m_lmRoleList);
		this.SetGroupItems(voBL, ref this.m_blRoleList);
		this.LMBg.width = 166 * ovLM.Length;
		this.BLBg.width = 166 * voBL.Length;
	}

	public void SetGroupItems(EntityVo[] ovList, ref List<HeadItem> list)
	{
		int count = list.Count;
		for (int i = 0; i < count; i++)
		{
			HeadItem headItem = list[i];
			if (i >= ovList.Length || ovList.Length <= 0)
			{
				headItem.gameObject.SetActive(false);
			}
			else
			{
				headItem.gameObject.SetActive(true);
				EntityVo entityVo = ovList[i];
				SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(entityVo.npc_id);
				headItem.UpdateItemById(entityVo.uid, heroMainData.avatar_icon, heroMainData.npc_id);
				this.m_activeItems.Add(headItem);
			}
		}
	}

	public HeadItem GetItem(string sid, TeamType tType)
	{
		HeadItem result;
		if (tType == TeamType.LM)
		{
			result = this.m_lmRoleList.Find((HeadItem obj) => obj.npc_id == sid);
		}
		else
		{
			result = this.m_blRoleList.Find((HeadItem obj) => obj.npc_id == sid);
		}
		return result;
	}

	public void UpdateHeroValue(string player_id, TeamType tType, float hp_value, float mp_value)
	{
		HeadItem item = this.GetItem(player_id, tType);
		if (item != null)
		{
			item.UpdateValue(hp_value, mp_value);
		}
	}
}
