using Com.Game.Data;
using Com.Game.Manager;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class SkillAiConfigData : UtilData
{
	private Dictionary<string, SysHeroSkillAiVo> _datas;

	public SkillAiConfigData(string id) : base(id)
	{
	}

	protected override void InitConfig()
	{
		this._datas = new Dictionary<string, SysHeroSkillAiVo>();
	}

	public int GetIndexToLevelup(string heroName, int lv)
	{
		SysHeroSkillAiVo sysHeroSkillAiVo;
		if (this._datas.ContainsKey(heroName))
		{
			sysHeroSkillAiVo = this._datas[heroName];
		}
		else
		{
			sysHeroSkillAiVo = BaseDataMgr.instance.GetDataById<SysHeroSkillAiVo>(heroName);
			if (sysHeroSkillAiVo == null)
			{
				Debug.LogError("配置表没有 " + heroName + " 的配置>请检查HeroSkillAi表");
				return -1;
			}
			this._datas.Add(heroName, sysHeroSkillAiVo);
		}
		Type typeFromHandle = typeof(SysHeroSkillAiVo);
		int result;
		try
		{
			FieldInfo field = typeFromHandle.GetField("lv" + lv, BindingFlags.Instance | BindingFlags.Public);
			int num = (int)field.GetValue(sysHeroSkillAiVo);
			result = num;
		}
		catch (Exception ex)
		{
			Debug.LogError(string.Format("Exception: {0}", ex.Message));
			result = -1;
		}
		return result;
	}
}
