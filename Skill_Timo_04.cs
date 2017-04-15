using Com.Game.Module;
using MobaFrame.SkillAction;
using MobaHeros.Pvp;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class Skill_Timo_04 : Skill
{
	protected float maxChargeTime = 10f;

	private float preMaxChargeTime = 10f;

	protected BuffData buffData;

	private Coroutine task;

	protected string NeedBuff
	{
		get;
		set;
	}

	protected int currCount
	{
		get
		{
			return this.self.buffManager.GetBuffLayer(this.NeedBuff);
		}
	}

	public Skill_Timo_04(string skill_id, Units self) : base(skill_id, self)
	{
		this.NeedBuff = "buff_" + base.skillMainId;
		this.buffData = Singleton<BuffDataManager>.Instance.GetVo(this.NeedBuff);
		if (this.buffData == null)
		{
			UnityEngine.Debug.LogError("BuffHostSkill buff is null: " + this.NeedBuff);
		}
		this.task = GlobalObject.Instance.StartCoroutine(this._update());
	}

	public override void genericSet()
	{
		if (this.IsMaxCount())
		{
			this.ShowSkillActiveHint();
		}
		this.RemoveCount();
		base.genericSet();
		this.HideSkillActiveHint();
	}

	public override bool CheckCD(bool showMsg = true)
	{
		return base.CheckCD(showMsg) && this.currCount > 0;
	}

	[DebuggerHidden]
	private IEnumerator _update()
	{
		Skill_Timo_04.<_update>c__Iterator99 <_update>c__Iterator = new Skill_Timo_04.<_update>c__Iterator99();
		<_update>c__Iterator.<>f__this = this;
		return <_update>c__Iterator;
	}

	public override void OnUpdate(float deltaTime)
	{
		base.OnUpdate(deltaTime);
	}

	public override void ParseExtraParam()
	{
		int num = 0;
		if (base.Data.config.SkillExtraParam != null && int.TryParse(base.Data.config.SkillExtraParam, out num))
		{
			this.maxChargeTime = (float)num;
			if (this.skillLevel == 1)
			{
				this.preMaxChargeTime = this.maxChargeTime;
			}
		}
	}

	protected virtual void UpdateTime(float deltaTime)
	{
		if (this.skillKey.Level <= 0)
		{
			return;
		}
		float chargeTime = this.self.GetChargeTime(base.skillMainId);
		if (this.IsMaxCount())
		{
			if (this.self.isPlayer)
			{
				Singleton<SkillView>.Instance.UpdateChargeCD(base.skillIndex, 0f, chargeTime, this.currCount);
			}
			return;
		}
		if (this.self.isPlayer)
		{
			if (!this.IsMaxCount())
			{
				if (base.IsCDTimeOver)
				{
					Singleton<SkillView>.Instance.UpdateChargeCD(base.skillIndex, chargeTime / this.preMaxChargeTime, chargeTime, this.currCount);
				}
				else
				{
					Singleton<SkillView>.Instance.UpdateChargeCD(base.skillIndex, 0f, chargeTime, this.currCount);
				}
			}
			else
			{
				Singleton<SkillView>.Instance.UpdateChargeCD(base.skillIndex, 0f, chargeTime, this.currCount);
			}
		}
	}

	public virtual void RemoveCount()
	{
		if (Singleton<PvpManager>.Instance.IsInPvp)
		{
			return;
		}
		ActionManager.RemoveBuff(this.NeedBuff, this.self, this.self, 1, true);
	}

	public virtual void AddCount()
	{
		if (Singleton<PvpManager>.Instance.IsInPvp)
		{
			return;
		}
		ActionManager.AddBuff(this.NeedBuff, this.self, this.self, true, string.Empty);
	}

	public bool IsMaxCount()
	{
		return this.currCount >= this.buffData.config.max_layers;
	}

	public virtual void SetCount(int count)
	{
		ActionManager.RemoveBuff(this.NeedBuff, this.self, this.self, -1, false);
		for (int i = 0; i < count; i++)
		{
			ActionManager.AddBuff(this.NeedBuff, this.self, this.self, false, string.Empty);
		}
	}

	public override void OnStop()
	{
		this.HideSkillActiveHint();
	}

	public override void OnExit()
	{
		this.HideSkillActiveHint();
		if (this.task != null && GlobalObject.Instance != null)
		{
			GlobalObject.Instance.StopCoroutine(this.task);
		}
	}

	public override void OnDeath(Units attacker)
	{
		this.HideSkillActiveHint();
	}

	protected virtual void ShowSkillActiveHint()
	{
		if (this.unit != null)
		{
			Singleton<SkillView>.Instance.ShowTriggerBornPowerObjHint(base.skillMainId);
		}
	}

	protected virtual void HideSkillActiveHint()
	{
		if (this.unit != null)
		{
			Singleton<SkillView>.Instance.HideTriggerBornPowerObjHint(base.skillMainId);
		}
	}

	public override void SynInfo(SynSkillInfo info)
	{
		this.self.SetChargeTime(info.skillId, (float)(info.extraInt1 / 1000));
	}
}
