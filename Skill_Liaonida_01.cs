using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class Skill_Liaonida_01 : Skill
{
	private Mecanim mecanim;

	private float curZhijianWeight;

	private float curNormalWeight;

	private Coroutine task;

	public Skill_Liaonida_01()
	{
	}

	public Skill_Liaonida_01(string skill_id, Units self) : base(skill_id, self)
	{
		if (this.mecanim == null)
		{
			this.mecanim = this.unit.animController.GetMecanim();
		}
		this.task = GlobalObject.Instance.StartCoroutine(this.UpdateAnim());
	}

	public override void OnExit()
	{
		if (this.task != null && GlobalObject.Instance != null)
		{
			GlobalObject.Instance.StopCoroutine(this.task);
		}
		base.OnExit();
	}

	[DebuggerHidden]
	private IEnumerator UpdateAnim()
	{
		Skill_Liaonida_01.<UpdateAnim>c__Iterator9E <UpdateAnim>c__Iterator9E = new Skill_Liaonida_01.<UpdateAnim>c__Iterator9E();
		<UpdateAnim>c__Iterator9E.<>f__this = this;
		return <UpdateAnim>c__Iterator9E;
	}

	public override void OnUpdate(float deltaTime)
	{
	}
}
