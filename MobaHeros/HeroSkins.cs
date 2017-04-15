using Com.Game.Data;
using Com.Game.Manager;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MobaHeros
{
	public class HeroSkins
	{
		private static readonly Dictionary<TeamType, Dictionary<string, int>> _skinsDict;

		private static readonly Dictionary<TeamType, Dictionary<string, int>> _realSkinsDict;

		static HeroSkins()
		{
			HeroSkins._skinsDict = new Dictionary<TeamType, Dictionary<string, int>>();
			HeroSkins._realSkinsDict = new Dictionary<TeamType, Dictionary<string, int>>();
			for (int i = 0; i < 4; i++)
			{
				HeroSkins._skinsDict[(TeamType)i] = new Dictionary<string, int>();
				HeroSkins._realSkinsDict[(TeamType)i] = new Dictionary<string, int>();
			}
		}

		public static void SetHeroSkin(TeamType team, string heroName, int skinIdx)
		{
			if (HeroSkins._skinsDict == null || HeroSkins._realSkinsDict == null)
			{
				return;
			}
			if (HeroSkins._skinsDict[team] == null || HeroSkins._realSkinsDict[team] == null)
			{
				return;
			}
			if (string.IsNullOrEmpty(heroName))
			{
				return;
			}
			if (HeroSkins._skinsDict[team].ContainsKey(heroName))
			{
				HeroSkins._skinsDict[team][heroName] = skinIdx;
			}
			else
			{
				HeroSkins._skinsDict[team].Add(heroName, skinIdx);
			}
			int value = 0;
			if (skinIdx != 0)
			{
				value = int.Parse(skinIdx.ToString().Substring(4, 2));
			}
			if (!HeroSkins._realSkinsDict.ContainsKey(team))
			{
				HeroSkins._realSkinsDict.Add(team, new Dictionary<string, int>());
			}
			if (HeroSkins._realSkinsDict[team].ContainsKey(heroName))
			{
				HeroSkins._realSkinsDict[team][heroName] = value;
			}
			else
			{
				HeroSkins._realSkinsDict[team].Add(heroName, value);
			}
		}

		public static int GetHeroSkin(TeamType team, string heroName)
		{
			Dictionary<string, int> dictionary = HeroSkins._skinsDict[team];
			return (!dictionary.ContainsKey(heroName)) ? 0 : dictionary[heroName];
		}

		public static int GetRealHeroSkin(TeamType team, string heroName)
		{
			if (heroName == null)
			{
				return 0;
			}
			Dictionary<string, int> dictionary = HeroSkins._realSkinsDict[team];
			if (dictionary == null)
			{
				return 0;
			}
			return (!dictionary.ContainsKey(heroName)) ? 0 : dictionary[heroName];
		}

		public static void GetHeroSkinPath(int skin, ref string path)
		{
			if (skin > 0)
			{
				if (skin <= 9)
				{
					path = path + "0" + skin;
				}
				else
				{
					path = path + string.Empty + skin;
				}
			}
		}

		public static void GetHeroSkinResPath(int skin, SysGameResVo gameRes, ref string path, ref string gameResId, bool isMonster = false)
		{
			if (gameRes != null)
			{
				path = gameRes.path;
				gameResId = gameRes.id;
				if (gameRes.type == 7 && gameRes.group == "common")
				{
					return;
				}
			}
			if (skin > 0)
			{
				if (skin <= 9)
				{
					if (isMonster)
					{
						path = path + "0" + skin;
						gameResId = gameResId + "0" + skin;
					}
					else
					{
						path = path + "_skin0" + skin;
						gameResId = gameResId + "_skin0" + skin;
					}
				}
				else if (isMonster)
				{
					path += skin;
					gameResId += skin;
				}
				else
				{
					path = path + "_skin" + skin;
					gameResId = gameResId + "_skin" + skin;
				}
			}
		}

		public static GameObject GetHeroPrefabWithSkin(string pathWithoutSkin, int skinId)
		{
			string resPath = string.Empty;
			if (skinId == 0)
			{
				resPath = pathWithoutSkin;
			}
			else
			{
				SysHeroSkinVo sysHeroSkinVo = BaseDataMgr.instance.GetDicByType<SysHeroSkinVo>()[skinId.ToString()] as SysHeroSkinVo;
				SysGameResVo sysGameResVo = BaseDataMgr.instance.GetDicByType<SysGameResVo>()[sysHeroSkinVo.model_id] as SysGameResVo;
				resPath = sysGameResVo.path;
			}
			GameObject unitAtResPath = CachedRes.getUnitAtResPath(resPath);
			if (unitAtResPath != null)
			{
				return unitAtResPath;
			}
			return CachedRes.getUnitAtResPath(pathWithoutSkin);
		}

		public static string GetHeroSkinPath(string pathWithoutSkin, int skinId)
		{
			string result;
			if (skinId == 0)
			{
				result = pathWithoutSkin;
			}
			else
			{
				SysHeroSkinVo sysHeroSkinVo = BaseDataMgr.instance.GetDicByType<SysHeroSkinVo>()[skinId.ToString()] as SysHeroSkinVo;
				SysGameResVo sysGameResVo = BaseDataMgr.instance.GetDicByType<SysGameResVo>()[sysHeroSkinVo.model_id] as SysGameResVo;
				result = sysGameResVo.id;
			}
			return result;
		}

		public static void Clear()
		{
			foreach (Dictionary<string, int> current in HeroSkins._skinsDict.Values)
			{
				current.Clear();
			}
			foreach (Dictionary<string, int> current2 in HeroSkins._realSkinsDict.Values)
			{
				current2.Clear();
			}
		}
	}
}
