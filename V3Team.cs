using Com.Game.Module;
using MobaHeros.Pvp;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

public class V3Team : MonoBehaviour
{
	[SerializeField]
	private UILabel lb_teamName;

	[SerializeField]
	private UIGrid grid_mems;

	private V3TeamMem template_mem_bl;

	private V3TeamMem template_mem_lm;

	private V3TeamMem template_mem_team3;

	private Dictionary<int, V3TeamMem> dicComs;

	private int finishCounter;

	private List<ReadyPlayerSampleInfo> listMemInfo;

	public TeamType TeamType_P
	{
		get;
		set;
	}

	public Action<V3Team> OnTeamLoadFinish
	{
		get;
		set;
	}

	private void Awake()
	{
		this.template_mem_bl = Resources.Load<V3TeamMem>("Prefab/UI/PVPLoading/V3TeamMem_lm");
		this.template_mem_lm = Resources.Load<V3TeamMem>("Prefab/UI/PVPLoading/V3TeamMem_bl");
		this.template_mem_team3 = Resources.Load<V3TeamMem>("Prefab/UI/PVPLoading/V3TeamMem_team3");
		this.dicComs = new Dictionary<int, V3TeamMem>();
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void UpdateCom()
	{
		this.Init();
		this.RefreshUI_title();
		this.RefresUI_mem();
	}

	private void Init()
	{
		this.listMemInfo = Singleton<PvpManager>.Instance.GetPlayersByTeam(this.TeamType_P);
	}

	private void RefreshUI_title()
	{
	}

	private void RefresUI_mem()
	{
		this.finishCounter = 0;
		for (int i = 0; i < this.listMemInfo.Count; i++)
		{
			ReadyPlayerSampleInfo readyPlayerSampleInfo = this.listMemInfo[i];
			V3TeamMem v3TeamMem = null;
			switch (this.TeamType_P)
			{
			case TeamType.LM:
				v3TeamMem = this.template_mem_lm;
				break;
			case TeamType.BL:
				v3TeamMem = this.template_mem_bl;
				break;
			case TeamType.Team_3:
				v3TeamMem = this.template_mem_team3;
				break;
			}
			GameObject gameObject = NGUITools.AddChild(this.grid_mems.gameObject, v3TeamMem.gameObject);
			V3TeamMem component = gameObject.GetComponent<V3TeamMem>();
			component.MemInfo = readyPlayerSampleInfo;
			component.OnLoadFinish = new Action<V3TeamMem>(this.OnLoadFinish);
			this.dicComs[readyPlayerSampleInfo.newUid] = component;
			component.UpdateCom();
			component.gameObject.SetActive(true);
		}
		this.grid_mems.Reposition();
		if (this.dicComs.Count == 0 && this.OnTeamLoadFinish != null)
		{
			this.OnTeamLoadFinish(this);
		}
	}

	private void OnLoadFinish(V3TeamMem com)
	{
		if (++this.finishCounter == this.dicComs.Count && this.OnTeamLoadFinish != null)
		{
			this.OnTeamLoadFinish(this);
		}
	}
}
