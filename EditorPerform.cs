using Com.Game.Module;
using MobaFrame.SkillAction;
using MobaHeros;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class EditorPerform
{
	public PerformData data;

	protected Transform trans;

	protected EditorUnit unit;

	protected List<EditorUnit> targets;

	protected Vector3? pos;

	protected AudioSourceControl m_audioSourceControl;

	protected EditorSkill skill;

	protected int hitCount;

	protected Vector3 curPos;

	private Coroutine playtask;

	public ResourceHandle handle
	{
		get;
		set;
	}

	public virtual void Start(string performID, EditorUnit unit, List<EditorUnit> targets, Vector3? pos, EditorSkill skill)
	{
		this.data = Singleton<PerformDataManager>.Instance.GetVo(performID);
		this.targets = targets;
		this.pos = pos;
		this.unit = unit;
		this.skill = skill;
		this.data = Singleton<PerformDataManager>.Instance.GetVo(performID);
		unit.StartCoroutine(this.PlayAnim());
		unit.StartCoroutine(this.PlayEffect());
		if (this.data.config.effect_time > 0f)
		{
			unit.StartCoroutine(this.Destroy());
		}
	}

	protected virtual void OnHit()
	{
		if (this.hitCount >= 1)
		{
			return;
		}
		if (this.skill != null && this.targets != null)
		{
			this.skill.OnHit(this.targets);
		}
		this.hitCount++;
	}

	[DebuggerHidden]
	private IEnumerator PlayAnim()
	{
		EditorPerform.<PlayAnim>c__Iterator1DD <PlayAnim>c__Iterator1DD = new EditorPerform.<PlayAnim>c__Iterator1DD();
		<PlayAnim>c__Iterator1DD.<>f__this = this;
		return <PlayAnim>c__Iterator1DD;
	}

	[DebuggerHidden]
	private IEnumerator EndPlayAnim()
	{
		EditorPerform.<EndPlayAnim>c__Iterator1DE <EndPlayAnim>c__Iterator1DE = new EditorPerform.<EndPlayAnim>c__Iterator1DE();
		<EndPlayAnim>c__Iterator1DE.<>f__this = this;
		return <EndPlayAnim>c__Iterator1DE;
	}

	protected virtual bool CheackHit()
	{
		return this.data.useCollider;
	}

	protected virtual void OnPlayEffect()
	{
	}

	[DebuggerHidden]
	protected virtual IEnumerator PlayEffect()
	{
		EditorPerform.<PlayEffect>c__Iterator1DF <PlayEffect>c__Iterator1DF = new EditorPerform.<PlayEffect>c__Iterator1DF();
		<PlayEffect>c__Iterator1DF.<>f__this = this;
		return <PlayEffect>c__Iterator1DF;
	}

	[DebuggerHidden]
	private IEnumerator Destroy()
	{
		EditorPerform.<Destroy>c__Iterator1E0 <Destroy>c__Iterator1E = new EditorPerform.<Destroy>c__Iterator1E0();
		<Destroy>c__Iterator1E.<>f__this = this;
		return <Destroy>c__Iterator1E;
	}

	public void doDestroy()
	{
		if (this.handle != null && this.handle.Raw != null)
		{
			UnityEngine.Object.Destroy(this.handle.Raw.gameObject);
		}
		if (this.m_audioSourceControl != null)
		{
			this.m_audioSourceControl.stopPlaying();
			this.m_audioSourceControl = null;
		}
	}

	[DebuggerHidden]
	protected virtual IEnumerator UpdateDeltaTime()
	{
		EditorPerform.<UpdateDeltaTime>c__Iterator1E1 <UpdateDeltaTime>c__Iterator1E = new EditorPerform.<UpdateDeltaTime>c__Iterator1E1();
		<UpdateDeltaTime>c__Iterator1E.<>f__this = this;
		return <UpdateDeltaTime>c__Iterator1E;
	}

	protected virtual void Update()
	{
		if (this.CheackHit())
		{
			this.OnHit();
		}
	}

	private void UpdatePosition()
	{
		if (this.trans == null)
		{
			return;
		}
		switch (this.data.config.effect_pos_type)
		{
		case 1:
		case 5:
		{
			if (this.unit.Unit.mTransform == null)
			{
				return;
			}
			Transform transform = null;
			Vector3 zero = Vector3.zero;
			int effect_anchor = this.data.config.effect_anchor;
			this.unit.Unit.GetBone(effect_anchor, out transform, out zero);
			Vector3 point = new Vector3(this.data.offset_x, this.data.offset_y, this.data.offset_z);
			this.trans.position = transform.position + zero + this.unit.Unit.mTransform.rotation * point;
			this.trans.rotation = Quaternion.Euler(((!this.data.isUseCasterRot) ? Vector3.zero : this.unit.Unit.mTransform.localEulerAngles) + new Vector3(this.data.offset_rx, this.data.offset_ry, this.data.offset_rz));
			this.trans.localScale = Vector3.one;
			break;
		}
		case 2:
		{
			Transform parent = null;
			Vector3 zero2 = Vector3.zero;
			int effect_anchor2 = this.data.config.effect_anchor;
			this.unit.Unit.GetBone(effect_anchor2, out parent, out zero2);
			this.trans.parent = parent;
			this.trans.localPosition = new Vector3(this.data.offset_x + zero2.x, this.data.offset_y + zero2.y, this.data.offset_z + zero2.z);
			this.trans.localRotation = Quaternion.Euler(new Vector3(this.data.offset_rx, this.data.offset_ry, this.data.offset_rz));
			this.trans.localScale = Vector3.one;
			break;
		}
		case 3:
		{
			Vector3 position = this.pos.Value + new Vector3(this.data.offset_x, this.data.offset_y, this.data.offset_z);
			if (this.trans != null)
			{
				this.trans.position = position;
				this.trans.rotation = Quaternion.Euler(this.pos.Value + new Vector3(this.data.offset_rx, this.data.offset_ry, this.data.offset_rz));
				this.trans.localScale = Vector3.one;
			}
			this.curPos = position;
			break;
		}
		case 6:
		{
			if (this.unit.Unit.mTransform == null)
			{
				return;
			}
			Transform transform2 = null;
			Vector3 zero3 = Vector3.zero;
			int effect_anchor3 = this.data.config.effect_anchor;
			this.unit.Unit.GetBone(effect_anchor3, out transform2, out zero3);
			Vector3 a = new Vector3(this.data.offset_x, this.data.offset_y, this.data.offset_z);
			float d = Vector3.Distance(a, Vector3.zero);
			Vector3 a2 = this.pos.Value - this.unit.Unit.mTransform.position;
			a2.Normalize();
			this.trans.position = transform2.position + zero3 + a2 * d;
			this.trans.LookAt(this.pos.Value);
			this.trans.localScale = Vector3.one;
			break;
		}
		}
	}

	private void PlaySound()
	{
		if (this.playtask != null)
		{
			this.unit.StopCoroutine(this.playtask);
		}
		this.playtask = this.unit.StartCoroutine(this.PlaySound_Cor());
	}

	[DebuggerHidden]
	public IEnumerator PlaySound_Cor()
	{
		EditorPerform.<PlaySound_Cor>c__Iterator1E2 <PlaySound_Cor>c__Iterator1E = new EditorPerform.<PlaySound_Cor>c__Iterator1E2();
		<PlaySound_Cor>c__Iterator1E.<>f__this = this;
		return <PlaySound_Cor>c__Iterator1E;
	}
}
