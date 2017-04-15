using Assets.Scripts.Model;
using Com.Game.Manager;
using Com.Game.Module;
using MobaHeros.Spawners;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class TestSkill : MonoBehaviour
{
	public static TestSkill Instance;

	public Units selfUnit;

	public int self_skill_index;

	public List<Units> targetUnits;

	public int target_skill_index;

	public int buff_index;

	public int higheffect_index;

	[SerializeField]
	private bool StartReplay;

	[SerializeField]
	private bool StartRecord;

	[SerializeField]
	private bool OpenTestView;

	private MVC_MessageManager _mvc_MessageManager;

	private void Awake()
	{
		TestSkill.Instance = this;
		this._mvc_MessageManager = new MVC_MessageManager();
	}

	private void Start()
	{
		TestSpawner.OpenTestView = false;
		BaseDataMgr.instance.InitBaseConfigData();
		LevelManager.m_CurLevel.Set(1, "10031", "10031", PvpJoinType.Single, 0);
		GameManager.Instance.StartGame();
		CtrlManager.OpenWindow(WindowID.FPSView, null);
		CtrlManager.OpenWindow(WindowID.CharacterView, null);
		CtrlManager.OpenWindow(WindowID.MessageView, null);
		new Task(this.DelayStart(), true);
		if (this.OpenTestView)
		{
			CtrlManager.OpenWindow(WindowID.TestModeView2, null);
		}
	}

	[DebuggerHidden]
	private IEnumerator DelayStart()
	{
		return new TestSkill.<DelayStart>c__Iterator3();
	}

	private void OnDestroy()
	{
		GameManager.Instance.EndGame();
		CtrlManager.CloseWindow(WindowID.FPSView);
		CtrlManager.CloseWindow(WindowID.CharacterView);
		CtrlManager.CloseWindow(WindowID.MessageView);
		CtrlManager.CloseWindow(WindowID.SkillView);
		if (this.OpenTestView)
		{
			CtrlManager.CloseWindow(WindowID.TestModeView2);
		}
	}
}
