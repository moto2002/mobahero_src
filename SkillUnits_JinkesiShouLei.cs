using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class SkillUnits_JinkesiShouLei : SkillUnit
{
	public new Animator anim;

	public GameObject halo;

	protected override void OnCreate()
	{
		base.OnCreate();
	}

	protected override void Awake()
	{
		this.anim = base.gameObject.GetComponentInChildren<Animator>();
		base.Awake();
	}

	protected override void OnStart()
	{
		base.OnStart();
		UnitVisibilityManager.SetItemVisible(this, true);
		GlobalObject.Instance.StartCoroutine(this.PlayHalo());
	}

	public override void RemoveSelf(float delay = 0f)
	{
		GlobalObject.Instance.StartCoroutine(this.PlayAnim());
	}

	protected override void OnDestroy()
	{
		GlobalObject.Instance.StopCoroutine(this.PlayAnim());
		base.OnDestroy();
	}

	[DebuggerHidden]
	private IEnumerator PlayHalo()
	{
		SkillUnits_JinkesiShouLei.<PlayHalo>c__Iterator20 <PlayHalo>c__Iterator = new SkillUnits_JinkesiShouLei.<PlayHalo>c__Iterator20();
		<PlayHalo>c__Iterator.<>f__this = this;
		return <PlayHalo>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator PlayAnim()
	{
		SkillUnits_JinkesiShouLei.<PlayAnim>c__Iterator21 <PlayAnim>c__Iterator = new SkillUnits_JinkesiShouLei.<PlayAnim>c__Iterator21();
		<PlayAnim>c__Iterator.<>f__this = this;
		return <PlayAnim>c__Iterator;
	}
}
