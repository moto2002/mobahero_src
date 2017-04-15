using Com.Game.Module;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class Skill_Jinkesi_01 : Skill
{
	public static readonly string CannonBuff = "1033010401";

	private Mecanim mecanim;

	private float curZhijianWeight;

	private float curNormalWeight;

	private Coroutine task;

	public Skill_Jinkesi_01()
	{
	}

	public Skill_Jinkesi_01(string skill_id, Units self) : base(skill_id, self)
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

	public override void SynInfo(SynSkillInfo info)
	{
		if (info.extraInt1 == 0 && this.unit.isPlayer)
		{
			Singleton<SkillView>.Instance.UpdateSkillView(base.realSkillMainId, true);
		}
		if (info.extraInt1 == 1 && this.unit.isPlayer)
		{
			if (info.extraInt2 == 0f)
			{
				Singleton<SkillView>.Instance.RefreshIcon(base.skillIndex, "Skill_Jinkesi_01_01");
			}
			else if (info.extraInt2 == 1f)
			{
				Singleton<SkillView>.Instance.RefreshIcon(base.skillIndex, "Skill_Jinkesi_01");
			}
		}
	}

	public override void OnSynced(byte inUseState)
	{
		base.OnSynced(inUseState);
		if (this.unit.buffManager.IsHaveBuff(Skill_Jinkesi_01.CannonBuff))
		{
			if (this.unit.isPlayer)
			{
				Singleton<SkillView>.Instance.RefreshIcon(base.skillIndex, "Skill_Jinkesi_01");
			}
		}
		else if (this.unit.isPlayer)
		{
			Singleton<SkillView>.Instance.RefreshIcon(base.skillIndex, "Skill_Jinkesi_01_01");
		}
	}

	[DebuggerHidden]
	private IEnumerator UpdateAnim()
	{
		Skill_Jinkesi_01.<UpdateAnim>c__Iterator9D <UpdateAnim>c__Iterator9D = new Skill_Jinkesi_01.<UpdateAnim>c__Iterator9D();
		<UpdateAnim>c__Iterator9D.<>f__this = this;
		return <UpdateAnim>c__Iterator9D;
	}
}
