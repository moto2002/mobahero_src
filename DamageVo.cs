using Com.Game.Module;
using MobaFrame.SkillAction;
using System;
using UnityEngine;

public class DamageVo
{
	public int damageId;

	public DamageData data;

	public float final_damage;

	protected DamageVo(int damage_id, DamageData data)
	{
		this.damageId = damage_id;
		this.data = data;
	}

	public static DamageVo Create(int damage_id)
	{
		DamageData vo = Singleton<DamageDataManager>.Instance.GetVo(damage_id);
		if (vo == null)
		{
			Debug.LogError(" DamageVo Error : No Damage !! " + damage_id);
			return null;
		}
		return new DamageVo(damage_id, vo);
	}
}
