using Com.Game.Module;
using MobaFrame.SkillAction;
using System;
using UnityEngine;

public class PerformVo
{
	public string perform_id;

	public PerformData data;

	public string effect_id;

	public PerformVo endPerform;

	public Units casterUnit;

	protected PerformVo(string perform_id, Units casterUnit)
	{
		this.perform_id = perform_id;
		this.data = Singleton<PerformDataManager>.Instance.GetVo(perform_id);
		this.casterUnit = casterUnit;
		this.effect_id = this.data.config.effect_id;
		if (StringUtils.CheckValid(this.data.endPerformId))
		{
			this.endPerform = PerformVo.Create(this.data.endPerformId, casterUnit);
		}
	}

	public static PerformVo Create(string perform_id, Units casterUnit)
	{
		if (!StringUtils.CheckValid(perform_id))
		{
			Debug.LogError(" PerformVo Error : No Perform !! " + perform_id);
			return null;
		}
		if (Singleton<PerformDataManager>.Instance.GetVo(perform_id) == null)
		{
			Debug.LogError(" PerformVo Error : No Perform !! " + perform_id);
			return null;
		}
		return new PerformVo(perform_id, casterUnit);
	}

	public bool IsDestroyByTime()
	{
		return true;
	}

	public static bool IsHavePerfrom(HighEffectData data)
	{
		return !(data.config.perform_id == string.Empty) && !(data.config.perform_id == "[]") && !(data.config.perform_id == "Null");
	}
}
