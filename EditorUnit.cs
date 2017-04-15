using Com.Game.Data;
using Com.Game.Manager;
using System;
using UnityEngine;

public class EditorUnit : EditorMono
{
	private SysHeroMainVo data;

	private new CapsuleCollider collider;

	public AutoStart AutoStart
	{
		get;
		private set;
	}

	public Units Unit
	{
		get;
		private set;
	}

	public EditorMoveController MoveController
	{
		get;
		set;
	}

	public EditorAnimationController AnimController
	{
		get;
		set;
	}

	public EditorSkillManager SkillManager
	{
		get;
		set;
	}

	public EditorBuffManager BuffManager
	{
		get;
		set;
	}

	protected override void Awake()
	{
		this.AutoStart = base.GO.GetComponent<AutoStart>();
		this.Unit = base.GO.GetComponent<Units>();
		this.Unit.npc_id = this.AutoStart.npc_id;
		if (this.Unit is Hero)
		{
			this.data = BaseDataMgr.instance.GetHeroMainData(this.Unit.npc_id);
		}
		base.GO.layer = LayerMask.NameToLayer("Unit");
		this.collider = base.GO.AddComponent<CapsuleCollider>();
		this.collider.height = 2f;
		this.collider.center = Vector3.up * 2f;
		this.MoveController = this.AddEditorComponet<EditorMoveController>();
		this.AnimController = this.AddEditorComponet<EditorAnimationController>();
		this.SkillManager = this.AddEditorComponet<EditorSkillManager>();
		this.BuffManager = this.AddEditorComponet<EditorBuffManager>();
	}

	protected override void Start()
	{
		base.Start();
		if (this.Unit is Hero)
		{
			AkBankManager.LoadBank("S" + this.data.music_id + ".bnk", 0);
		}
	}

	private T AddEditorComponet<T>() where T : MonoBehaviour, IEditorUnitCompoent
	{
		T result = base.gameObject.AddComponent<T>();
		result.Init(this);
		return result;
	}
}
