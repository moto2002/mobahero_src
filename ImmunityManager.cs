using MobaHeros;
using System;
using System.Collections.Generic;

public class ImmunityManager : StaticUnitComponent
{
	private List<EffectDataType> datas = new List<EffectDataType>();

	public void AddImmunity(EffectDataType effectDataType)
	{
		this.datas.Add(effectDataType);
	}

	public void RemoveImmunity(EffectDataType effectDataType)
	{
		this.datas.Remove(effectDataType);
	}

	public bool IsImmunity(EffectDataType effectDataType)
	{
		bool result = false;
		for (int i = 0; i < this.datas.Count; i++)
		{
			if (this.IsImmunity(effectDataType, this.datas[i]))
			{
				result = true;
			}
		}
		return result;
	}

	private bool IsImmunity(EffectDataType targetDataType, EffectDataType dataType)
	{
		return targetDataType.ImmuneType != EffectImmuneType.igronImmune && (targetDataType.GainType == dataType.GainType || dataType.GainType == EffectGainType.all) && (targetDataType.MagicType == dataType.MagicType || dataType.MagicType == EffectMagicType.all);
	}
}
