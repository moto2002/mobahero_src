using Com.Game.Module;
using MobaFrame.SkillAction;
using System;
using UnityEngine;

public class HighEffVo
{
	public string higheff_id;

	public HighEffectData data;

	public int target_type;

	public Units casterUnit;

	public Vector3? skillPosition;

	public string skillId;

	public float time;

	public HighEffVo(string higheff_id, string skillId, Units casterUnit, Vector3? skillPosition = null, bool isPassiveAdd = false, int target_type = 1)
	{
		this.skillPosition = skillPosition;
		this.casterUnit = casterUnit;
		this.target_type = target_type;
		this.higheff_id = higheff_id;
		this.skillId = skillId;
		this.data = Singleton<HighEffectDataManager>.Instance.GetVo(higheff_id);
	}

	public static HighEffVo Create(string higheff_id, string skillId, Units casterUnit, Vector3? skillPosition = null, bool isPassiveAdd = false, int target_type = 1)
	{
		if (!StringUtils.CheckValid(higheff_id))
		{
			Debug.LogError(" HighEffVo Error : No HighEff !! " + higheff_id);
			return null;
		}
		if (Singleton<HighEffectDataManager>.Instance.GetVo(higheff_id) == null)
		{
			Debug.LogError(" HighEffVo Error : No HighEff !! " + higheff_id);
			return null;
		}
		return new HighEffVo(higheff_id, skillId, casterUnit, skillPosition, isPassiveAdd, target_type);
	}
}
