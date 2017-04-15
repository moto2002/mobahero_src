using Assets.Scripts.Model;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;

public class HeroesInBattle
{
	public static List<EntityVo> ConvertFromHeroInfo(List<HeroInfoData> infos)
	{
		List<EntityVo> list = new List<EntityVo>();
		if (infos != null)
		{
			for (int i = 0; i < infos.Count; i++)
			{
				HeroInfoData heroInfoData = infos[i];
				EntityVo item = new EntityVo(EntityType.Hero, heroInfoData.ModelId, CharacterDataMgr.instance.GetLevel(heroInfoData.Exp), heroInfoData.Level, heroInfoData.Grade, 0f, 0f, 0, 0);
				list.Add(item);
			}
		}
		return list;
	}

	public static HeroData GetHeroData(string heroID)
	{
		HeroInfoData heroInfoData = ModelManager.Instance.Get_heroInfo_item_byModelID_X(heroID);
		return new HeroData
		{
			Quality = heroInfoData.Grade,
			Stars = heroInfoData.Level,
			LV = CharacterDataMgr.instance.GetLevel(heroInfoData.Exp),
			HeroID = heroID
		};
	}

	public static EntityVo GetMyHero(string heroId)
	{
		HeroInfoData heroInfoData = ModelManager.Instance.Get_heroInfo_item_byModelID_X(heroId);
		EntityVo result;
		if (heroInfoData != null)
		{
			result = new EntityVo(EntityType.Hero, heroId, CharacterDataMgr.instance.GetLevel(heroInfoData.Exp), heroInfoData.Level, heroInfoData.Grade, 0f, 0f, 0, 0);
		}
		else
		{
			result = new EntityVo(EntityType.Hero, heroId, 1, 1, 1, 0f, 0f, 0, 0);
		}
		return result;
	}

	public static List<EntityVo> GetMyHeroByIDs(List<string> heros)
	{
		List<EntityVo> list = new List<EntityVo>();
		for (int i = 0; i < heros.Count; i++)
		{
			list.Add(HeroesInBattle.GetMyHero(heros[i]));
		}
		return list;
	}

	public static EntityVo GetMonsterByID(string npcId, int pos)
	{
		return new EntityVo(EntityType.Monster, npcId, pos, 0, string.Empty, "Default", 0);
	}
}
