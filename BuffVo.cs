using Com.Game.Module;
using MobaFrame.SkillAction;
using MobaHeros;
using System;
using UnityEngine;

public class BuffVo
{
	public static int assign_id;

	public string buff_id;

	public BuffData data;

	public Units casterUnit;

	public Units targerUnit;

	public int target_type;

	public int layer;

	public float lastTime;

	public float curTime;

	public float totalTime;

	public bool isShieldBuff;

	public bool isBloodBuff;

	public bool isPowerBuff;

	public int unique_id;

	public BuffVo(string buff_id, Units casterUnit, Units targerUnit, int target_type = 1)
	{
		this.buff_id = buff_id;
		this.data = Singleton<BuffDataManager>.Instance.GetVo(buff_id);
		this.casterUnit = casterUnit;
		this.targerUnit = targerUnit;
		this.target_type = target_type;
		this.curTime = 0f;
		this.totalTime = 0f;
		this.lastTime = 0f;
		this.ParseIsBuffType();
	}

	public static BuffVo Create(string buff_id, Units casterUnit, Units targerUnit, int target_type = 1)
	{
		if (!StringUtils.CheckValid(buff_id))
		{
			Debug.LogError(" BuffVo Error : No Buff !! " + buff_id);
			return null;
		}
		if (Singleton<BuffDataManager>.Instance.GetVo(buff_id) == null)
		{
			Debug.LogError(" BuffVo Error : No Buff !! " + buff_id);
			return null;
		}
		return new BuffVo(buff_id, casterUnit, targerUnit, target_type);
	}

	public void Reset(int type = 0)
	{
		if (type == 0)
		{
			this.totalTime = 0f;
			this.curTime = 0f;
			this.lastTime = Time.time;
		}
		else if (type == 1)
		{
			this.curTime = 0f;
			this.lastTime = Time.time;
		}
	}

	public void ResetTotalTime()
	{
	}

	private void ParseIsBuffType()
	{
		if (this.data.damage_ids != null && this.data.damage_ids.Length > 0)
		{
			DamageData vo = Singleton<DamageDataManager>.Instance.GetVo(this.data.damage_ids[0]);
			if (vo != null && (vo.damageCalType == 4 || vo.damageCalType == 5))
			{
				if (vo.property_key == AttrType.Shield)
				{
					this.isShieldBuff = true;
				}
				else if (vo.property_key == AttrType.Hp || vo.property_key == AttrType.HpMax)
				{
					this.isBloodBuff = true;
				}
				else if (vo.property_key == AttrType.Mp || vo.property_key == AttrType.MpMax)
				{
					this.isPowerBuff = true;
				}
				else if (vo.property_key == AttrType.MoveSpeed && vo.damageParam2 < 0f && this.target_type == 1 && this.targerUnit.isPlayer)
				{
					this.targerUnit.jumpFont(JumpFontType.MoveSpeedDown, string.Empty, null, true);
				}
			}
		}
	}

	public bool IsOver()
	{
		return this.data.config.buff_time != 0f && this.totalTime > this.data.config.buff_time;
	}

	public void ForceOver()
	{
		this.totalTime = this.data.config.buff_time + 1f;
	}

	public void OnUpdate(float deltaTime)
	{
		this.curTime += deltaTime;
		this.totalTime += deltaTime;
	}

	public bool IsEffective()
	{
		return this.data.IsEffective();
	}

	public bool IsPermanent()
	{
		return this.data.config.buff_time <= 0f;
	}

	public bool IsShieldOver(Units u)
	{
		return u == null || (this.isShieldBuff && u.shield <= 0f && this.totalTime > 1f);
	}

	public bool IsHpOverflow(Units u)
	{
		return u == null || !u.isLive || (this.isBloodBuff && u.IsFullHp && this.data.isStopEffectWhenFullHpAndMp);
	}

	public bool IsMpOverflow(Units u)
	{
		return u == null || !u.isLive || (this.isPowerBuff && u.IsFullMp && this.data.isStopEffectWhenFullHpAndMp);
	}

	public bool IsTimeUp()
	{
		return this.curTime >= this.data.config.effective_time;
	}
}
