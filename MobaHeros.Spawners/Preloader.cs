using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using Com.Game.Utils;
using MobaHeros.Pvp;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace MobaHeros.Spawners
{
	public class Preloader
	{
		private readonly EntityVoCreator _entityVoCreator;

		private readonly SysBattleSceneVo _scene;

		private static readonly int AoeMaxNum = 10;

		public Preloader(SysBattleSceneVo scene, EntityVoCreator entityVoCreator)
		{
			this._entityVoCreator = entityVoCreator;
			this._scene = scene;
		}

		private static string GetResId(EntityVo vo)
		{
			if (vo == null)
			{
				return null;
			}
			string npc_id = vo.npc_id;
			if (npc_id != string.Empty)
			{
				if (vo.entity_type == EntityType.Hero)
				{
					SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(npc_id);
					if (heroMainData == null)
					{
					}
					return heroMainData.model_id;
				}
				if (vo.entity_type == EntityType.Monster)
				{
					SysMonsterMainVo monsterMainData = BaseDataMgr.instance.GetMonsterMainData(npc_id);
					return monsterMainData.model_id;
				}
				ClientLogger.Error("cannot support " + vo.entity_type);
			}
			return null;
		}

		[DebuggerHidden]
		public IEnumerator PreloadResources(int count = 1)
		{
			Preloader.<PreloadResources>c__Iterator1C0 <PreloadResources>c__Iterator1C = new Preloader.<PreloadResources>c__Iterator1C0();
			<PreloadResources>c__Iterator1C.count = count;
			<PreloadResources>c__Iterator1C.<$>count = count;
			<PreloadResources>c__Iterator1C.<>f__this = this;
			return <PreloadResources>c__Iterator1C;
		}

		private void DoPreloadResources(string resId, int count = 1)
		{
			SysGameResVo gameResData = BaseDataMgr.instance.GetGameResData(resId);
			if (gameResData != null)
			{
				GameObject gameObject = ResourceManager.Load<GameObject>(gameResData.id, true, true, null, 0, false);
				if (gameObject == null)
				{
					ClientLogger.Error("Error resVo id=" + gameResData.id + " path = " + gameResData.path);
				}
				MapManager.Instance.CreatePrefabPoolByTeam(TeamType.None, gameObject.transform, count, 100, 20, 5);
			}
		}

		private void PreloadCharacterResources(string performId, int count = 1, int skin = 0)
		{
			Dictionary<string, object> dicByType = BaseDataMgr.instance.GetDicByType<SysGameResVo>();
			Dictionary<string, object>.Enumerator enumerator = dicByType.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<string, object> current = enumerator.Current;
				SysGameResVo sysGameResVo = current.Value as SysGameResVo;
				if (sysGameResVo != null)
				{
					int type = sysGameResVo.type;
					string group = sysGameResVo.group;
					if (type == 7)
					{
						if (group.Equals(performId))
						{
							GameObject gameObject = ResourceManager.Load<GameObject>(sysGameResVo.id, true, true, null, skin, false);
							if (gameObject == null)
							{
								gameObject = ResourceManager.Load<GameObject>(sysGameResVo.id, true, true, null, skin, false);
								if (gameObject == null)
								{
									ClientLogger.Error("Error resVo id=" + sysGameResVo.id);
								}
							}
							if (gameObject != null)
							{
								MapManager.Instance.CreatePrefabPoolByTeam(TeamType.None, gameObject.transform, count, 100, 20, 5);
							}
						}
					}
					else if (type != 3 || group.Equals(performId))
					{
					}
				}
			}
		}

		private void PreloadAvatar(string[] players, string[] enemies)
		{
			if (players != null)
			{
				for (int i = 0; i < players.Length; i++)
				{
					if (players[i] != string.Empty)
					{
						SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(players[i]);
						ResourceManager.Load<Texture>(heroMainData.avatar_icon, true, true, null, 0, false);
					}
				}
			}
			if (enemies != null)
			{
				for (int j = 0; j < enemies.Length; j++)
				{
					if (enemies[j] != string.Empty)
					{
						SysHeroMainVo heroMainData2 = BaseDataMgr.instance.GetHeroMainData(enemies[j]);
						ResourceManager.Load<Texture>(heroMainData2.avatar_icon, true, true, null, 0, false);
					}
				}
			}
		}

		private void LoadEntityVo(EntityVo vo, TeamType teamType, int count = 1)
		{
			string resId = Preloader.GetResId(vo);
			if (!string.IsNullOrEmpty(resId))
			{
				GameObject gameObject = ResourceManager.Load<GameObject>(resId, true, true, null, 0, false);
				if (gameObject == null)
				{
					UnityEngine.Debug.LogError("LoadEntityVo failed where resId= " + resId);
					return;
				}
				MapManager.Instance.CreatePrefabPoolByTeam(teamType, gameObject.transform, count, 100, 20, 5);
			}
		}

		[DebuggerHidden]
		public IEnumerator PreloadHeroes(List<EntityVo> heroList, TeamType inTeamTypeVal, int count = 1)
		{
			Preloader.<PreloadHeroes>c__Iterator1C1 <PreloadHeroes>c__Iterator1C = new Preloader.<PreloadHeroes>c__Iterator1C1();
			<PreloadHeroes>c__Iterator1C.heroList = heroList;
			<PreloadHeroes>c__Iterator1C.inTeamTypeVal = inTeamTypeVal;
			<PreloadHeroes>c__Iterator1C.count = count;
			<PreloadHeroes>c__Iterator1C.<$>heroList = heroList;
			<PreloadHeroes>c__Iterator1C.<$>inTeamTypeVal = inTeamTypeVal;
			<PreloadHeroes>c__Iterator1C.<$>count = count;
			<PreloadHeroes>c__Iterator1C.<>f__this = this;
			return <PreloadHeroes>c__Iterator1C;
		}

		private GameObject PreloadXiaoluMonster(string path, string skinPath, int skin)
		{
			GameObject gameObject = ResourceManager.LoadPath<GameObject>(path + skinPath, null, skin);
			if (gameObject == null && skin != 0)
			{
				gameObject = ResourceManager.LoadPath<GameObject>(path, null, skin);
			}
			return gameObject;
		}

		private void PreloadPersonalEffect(EntityVo vo, TeamType team)
		{
			if (Singleton<PvpManager>.Instance == null)
			{
				return;
			}
			HeroInfoData heroInfoData = Singleton<PvpManager>.Instance.GetHeroInfoData(vo.uid);
			if (heroInfoData == null)
			{
				return;
			}
			this.GetPersonalEffect(heroInfoData.backEffectId);
			this.GetPersonalEffect(heroInfoData.birthEffectId);
			this.GetPersonalEffect(heroInfoData.deathEffectId);
			this.GetPersonalEffect(heroInfoData.levelEffectId);
			this.GetPersonalEffect(heroInfoData.tailEffectId);
			if (heroInfoData.petId != 0)
			{
				SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(heroInfoData.petId.ToString());
				if (dataById == null)
				{
					UnityEngine.Debug.LogError("没有这个道具id error id=" + heroInfoData.petId.ToString());
					return;
				}
				SysMonsterMainVo monsterMainData = BaseDataMgr.instance.GetMonsterMainData(dataById.hero_decorate_param);
				if (monsterMainData == null)
				{
					UnityEngine.Debug.LogError("Error resId=" + monsterMainData.model_id);
					return;
				}
				GameObject gameObject = ResourceManager.Load<GameObject>(monsterMainData.model_id, true, true, null, 0, false);
				if (gameObject == null)
				{
					UnityEngine.Debug.LogError("Error resId=" + monsterMainData.model_id);
					return;
				}
				GameManager.Instance.SkillData.InsertMonsterSkillData(ref monsterMainData);
				MapManager.Instance.CreatePrefabPoolByTeam(team, gameObject.transform, 1, 100, 20, 5);
			}
		}

		private void GetPersonalEffect(int effectid)
		{
			if (effectid == 0)
			{
				return;
			}
			List<SysGameResVo> list = new List<SysGameResVo>();
			SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(effectid.ToString());
			if (dataById == null)
			{
				UnityEngine.Debug.LogError(effectid.ToString() + " is null");
			}
			this.CollectActionGameRes(1, dataById.hero_decorate_param, ref list);
			this.PreloadEffectCustom(list, 1, 0);
		}

		[DebuggerHidden]
		public IEnumerator PreloadGuard(int count = 1)
		{
			Preloader.<PreloadGuard>c__Iterator1C2 <PreloadGuard>c__Iterator1C = new Preloader.<PreloadGuard>c__Iterator1C2();
			<PreloadGuard>c__Iterator1C.count = count;
			<PreloadGuard>c__Iterator1C.<$>count = count;
			<PreloadGuard>c__Iterator1C.<>f__this = this;
			return <PreloadGuard>c__Iterator1C;
		}

		[DebuggerHidden]
		public IEnumerator PreloadTowers(TeamType teamType)
		{
			Preloader.<PreloadTowers>c__Iterator1C3 <PreloadTowers>c__Iterator1C = new Preloader.<PreloadTowers>c__Iterator1C3();
			<PreloadTowers>c__Iterator1C.teamType = teamType;
			<PreloadTowers>c__Iterator1C.<$>teamType = teamType;
			<PreloadTowers>c__Iterator1C.<>f__this = this;
			return <PreloadTowers>c__Iterator1C;
		}

		private List<SysGameResVo> CollectEffectResByHeroId(string heroId)
		{
			List<SysGameResVo> result = new List<SysGameResVo>();
			SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(heroId);
			if (heroMainData != null)
			{
				string[] stringValue = StringUtils.GetStringValue(heroMainData.attack_id, ',');
				string[] stringValue2 = StringUtils.GetStringValue(heroMainData.skill_id, ',');
				if (stringValue != null)
				{
					string[] array = stringValue;
					for (int i = 0; i < array.Length; i++)
					{
						string skillId = array[i];
						this.CollectSkillGameRes(skillId, ref result);
					}
				}
				if (stringValue2 != null)
				{
					string[] array2 = stringValue2;
					for (int j = 0; j < array2.Length; j++)
					{
						string skillId2 = array2[j];
						this.CollectSkillGameRes(skillId2, ref result);
					}
				}
				string[] stringValue3 = StringUtils.GetStringValue(heroMainData.effect_id, ',');
				string[] array3 = stringValue3;
				for (int k = 0; k < array3.Length; k++)
				{
					string str = array3[k];
					string[] stringValue4 = StringUtils.GetStringValue(str, '|');
					if (stringValue4 != null && stringValue4.Length > 1)
					{
						this.CollectActionGameRes(1, stringValue4[1], ref result);
					}
				}
			}
			return result;
		}

		private List<SysGameResVo> CollectEffectResByMonsterId(string monsterId)
		{
			List<SysGameResVo> result = new List<SysGameResVo>();
			SysMonsterMainVo monsterMainData = BaseDataMgr.instance.GetMonsterMainData(monsterId);
			if (monsterMainData != null)
			{
				string[] stringValue = StringUtils.GetStringValue(monsterMainData.attack_id, ',');
				string[] stringValue2 = StringUtils.GetStringValue(monsterMainData.skill_id, ',');
				if (stringValue != null)
				{
					string[] array = stringValue;
					for (int i = 0; i < array.Length; i++)
					{
						string skillId = array[i];
						this.CollectSkillGameRes(skillId, ref result);
					}
				}
				if (stringValue2 != null)
				{
					string[] array2 = stringValue2;
					for (int j = 0; j < array2.Length; j++)
					{
						string skillId2 = array2[j];
						this.CollectSkillGameRes(skillId2, ref result);
					}
				}
			}
			return result;
		}

		private List<SysGameResVo> CollectEffectResBySkillUnitId(string skillUnitId)
		{
			List<SysGameResVo> result = new List<SysGameResVo>();
			SysSkillUnitVo dataById = BaseDataMgr.instance.GetDataById<SysSkillUnitVo>(skillUnitId);
			if (dataById != null)
			{
				string[] stringValue = StringUtils.GetStringValue(dataById.attack_id, ',');
				string[] stringValue2 = StringUtils.GetStringValue(dataById.skill_id, ',');
				if (stringValue != null)
				{
					string[] array = stringValue;
					for (int i = 0; i < array.Length; i++)
					{
						string skillId = array[i];
						this.CollectSkillGameRes(skillId, ref result);
					}
				}
				if (stringValue2 != null)
				{
					string[] array2 = stringValue2;
					for (int j = 0; j < array2.Length; j++)
					{
						string skillId2 = array2[j];
						this.CollectSkillGameRes(skillId2, ref result);
					}
				}
			}
			return result;
		}

		private void CollectSkillGameRes(string skillId, ref List<SysGameResVo> result)
		{
			SysSkillMainVo skillMainData = BaseDataMgr.instance.GetSkillMainData(skillId);
			if (skillMainData != null)
			{
				string[] stringValue = StringUtils.GetStringValue(skillMainData.start_action_ids, ',');
				string[] stringValue2 = StringUtils.GetStringValue(skillMainData.ready_action_ids, ',');
				string[] stringValue3 = StringUtils.GetStringValue(skillMainData.hit_action_ids, ',');
				string[] stringValue4 = StringUtils.GetStringValue(skillMainData.end_action_ids, ',');
				if (stringValue != null)
				{
					string[] array = stringValue;
					for (int i = 0; i < array.Length; i++)
					{
						string id = array[i];
						this.CollectActionGameRes(1, id, ref result);
					}
				}
				if (stringValue2 != null)
				{
					string[] array2 = stringValue2;
					for (int j = 0; j < array2.Length; j++)
					{
						string id2 = array2[j];
						this.CollectActionGameRes(1, id2, ref result);
					}
				}
				if (stringValue3 != null)
				{
					string[] array3 = stringValue3;
					for (int k = 0; k < array3.Length; k++)
					{
						string id3 = array3[k];
						this.CollectActionGameRes(skillMainData.max_num, id3, ref result);
					}
				}
				if (stringValue4 != null)
				{
					string[] array4 = stringValue4;
					for (int l = 0; l < array4.Length; l++)
					{
						string id4 = array4[l];
						this.CollectActionGameRes(1, id4, ref result);
					}
				}
				string[] stringValue5 = StringUtils.GetStringValue(skillMainData.start_buff_ids, ',');
				string[] stringValue6 = StringUtils.GetStringValue(skillMainData.hit_buff_ids, ',');
				if (stringValue5 != null)
				{
					string[] array5 = stringValue5;
					for (int m = 0; m < array5.Length; m++)
					{
						string buffId = array5[m];
						this.CollectBuffGameRes(1, buffId, ref result);
					}
				}
				if (stringValue6 != null)
				{
					string[] array6 = stringValue6;
					for (int n = 0; n < array6.Length; n++)
					{
						string buffId2 = array6[n];
						this.CollectBuffGameRes(skillMainData.max_num, buffId2, ref result);
					}
				}
				string[] stringValue7 = StringUtils.GetStringValue(skillMainData.init_higheff_ids, ',');
				string[] stringValue8 = StringUtils.GetStringValue(skillMainData.start_higheff_ids, ',');
				string[] stringValue9 = StringUtils.GetStringValue(skillMainData.hit_higheff_ids, ',');
				if (stringValue7 != null)
				{
					string[] array7 = stringValue7;
					for (int num = 0; num < array7.Length; num++)
					{
						string higheffId = array7[num];
						this.CollectHigheffGameRes(1, higheffId, ref result);
					}
				}
				if (stringValue8 != null)
				{
					string[] array8 = stringValue8;
					for (int num2 = 0; num2 < array8.Length; num2++)
					{
						string higheffId2 = array8[num2];
						this.CollectHigheffGameRes(1, higheffId2, ref result);
					}
				}
				if (stringValue9 != null)
				{
					string[] array9 = stringValue9;
					for (int num3 = 0; num3 < array9.Length; num3++)
					{
						string higheffId3 = array9[num3];
						this.CollectHigheffGameRes(skillMainData.max_num, higheffId3, ref result);
					}
				}
			}
			this.CollectSkillLevelupEffectInGameRes(skillId + "_lv01", ref result);
			this.CollectSkillLevelupEffectInGameRes(skillId + "_lv02", ref result);
			this.CollectSkillLevelupEffectInGameRes(skillId + "_lv03", ref result);
			this.CollectSkillLevelupEffectInGameRes(skillId + "_lv04", ref result);
		}

		private void CollectSkillLevelupEffectInGameRes(string skillId, ref List<SysGameResVo> result)
		{
			SysSkillLevelupVo dataById = BaseDataMgr.instance.GetDataById<SysSkillLevelupVo>(skillId);
			if (dataById != null)
			{
				string[] stringValue = StringUtils.GetStringValue(dataById.start_action_ids, ',');
				string[] stringValue2 = StringUtils.GetStringValue(dataById.ready_action_ids, ',');
				string[] stringValue3 = StringUtils.GetStringValue(dataById.hit_action_ids, ',');
				string[] stringValue4 = StringUtils.GetStringValue(dataById.end_action_ids, ',');
				if (stringValue != null)
				{
					string[] array = stringValue;
					for (int i = 0; i < array.Length; i++)
					{
						string id = array[i];
						this.CollectActionGameRes(1, id, ref result);
					}
				}
				if (stringValue2 != null)
				{
					string[] array2 = stringValue2;
					for (int j = 0; j < array2.Length; j++)
					{
						string id2 = array2[j];
						this.CollectActionGameRes(1, id2, ref result);
					}
				}
				if (stringValue3 != null)
				{
					string[] array3 = stringValue3;
					for (int k = 0; k < array3.Length; k++)
					{
						string id3 = array3[k];
						this.CollectActionGameRes(dataById.max_num, id3, ref result);
					}
				}
				if (stringValue4 != null)
				{
					string[] array4 = stringValue4;
					for (int l = 0; l < array4.Length; l++)
					{
						string id4 = array4[l];
						this.CollectActionGameRes(1, id4, ref result);
					}
				}
				string[] stringValue5 = StringUtils.GetStringValue(dataById.start_buff_ids, ',');
				string[] stringValue6 = StringUtils.GetStringValue(dataById.hit_buff_ids, ',');
				if (stringValue5 != null)
				{
					string[] array5 = stringValue5;
					for (int m = 0; m < array5.Length; m++)
					{
						string buffId = array5[m];
						this.CollectBuffGameRes(1, buffId, ref result);
					}
				}
				if (stringValue6 != null)
				{
					string[] array6 = stringValue6;
					for (int n = 0; n < array6.Length; n++)
					{
						string buffId2 = array6[n];
						this.CollectBuffGameRes(dataById.max_num, buffId2, ref result);
					}
				}
				string[] stringValue7 = StringUtils.GetStringValue(dataById.init_higheff_ids, ',');
				string[] stringValue8 = StringUtils.GetStringValue(dataById.start_higheff_ids, ',');
				string[] stringValue9 = StringUtils.GetStringValue(dataById.hit_higheff_ids, ',');
				if (stringValue7 != null)
				{
					string[] array7 = stringValue7;
					for (int num = 0; num < array7.Length; num++)
					{
						string higheffId = array7[num];
						this.CollectHigheffGameRes(1, higheffId, ref result);
					}
				}
				if (stringValue8 != null)
				{
					string[] array8 = stringValue8;
					for (int num2 = 0; num2 < array8.Length; num2++)
					{
						string higheffId2 = array8[num2];
						this.CollectHigheffGameRes(1, higheffId2, ref result);
					}
				}
				if (stringValue9 != null)
				{
					string[] array9 = stringValue9;
					for (int num3 = 0; num3 < array9.Length; num3++)
					{
						string higheffId3 = array9[num3];
						this.CollectHigheffGameRes(dataById.max_num, higheffId3, ref result);
					}
				}
			}
		}

		private void CollectHigheffGameRes(int maxNum, string higheffId, ref List<SysGameResVo> result)
		{
			SysSkillHigheffVo dataById = BaseDataMgr.instance.GetDataById<SysSkillHigheffVo>(higheffId);
			if (dataById != null)
			{
				string[] stringValue = StringUtils.GetStringValue(dataById.perform_id, ',');
				if (stringValue != null)
				{
					string[] array = stringValue;
					for (int i = 0; i < array.Length; i++)
					{
						string id = array[i];
						this.CollectActionGameRes(maxNum, id, ref result);
					}
				}
				string[] stringValue2 = StringUtils.GetStringValue(dataById.attach_buff, ',');
				if (stringValue2 != null)
				{
					string[] array2 = stringValue2;
					for (int j = 0; j < array2.Length; j++)
					{
						string buffId = array2[j];
						this.CollectBuffGameRes(maxNum, buffId, ref result);
					}
				}
				if (dataById.higheff_type.StartsWith("32"))
				{
					string[] stringValue3 = StringUtils.GetStringValue(dataById.higheff_type, '|');
					if (stringValue3.Length > 2)
					{
						this.CollectActionGameRes(maxNum, stringValue3[2], ref result);
					}
				}
				string[] stringValue4 = StringUtils.GetStringValue(dataById.attach_higheff, ',');
				string[] stringValue5 = StringUtils.GetStringValue(dataById.attach_self_higheff, ',');
				if (stringValue4 != null)
				{
					string[] array3 = stringValue4;
					for (int k = 0; k < array3.Length; k++)
					{
						string higheffId2 = array3[k];
						this.CollectHigheffGameRes(maxNum, higheffId2, ref result);
					}
				}
				if (stringValue5 != null)
				{
					string[] array4 = stringValue5;
					for (int l = 0; l < array4.Length; l++)
					{
						string higheffId3 = array4[l];
						this.CollectHigheffGameRes(maxNum, higheffId3, ref result);
					}
				}
			}
		}

		private void CollectBuffGameRes(int maxNum, string buffId, ref List<SysGameResVo> result)
		{
			SysSkillBuffVo dataById = BaseDataMgr.instance.GetDataById<SysSkillBuffVo>(buffId);
			if (dataById != null)
			{
				string[] stringValue = StringUtils.GetStringValue(dataById.perform_id, ',');
				if (stringValue != null)
				{
					string[] array = stringValue;
					for (int i = 0; i < array.Length; i++)
					{
						string id = array[i];
						this.CollectActionGameRes(maxNum, id, ref result);
					}
				}
				string[] stringValue2 = StringUtils.GetStringValue(dataById.attach_buff, ',');
				string[] stringValue3 = StringUtils.GetStringValue(dataById.end_attach_buff, ',');
				if (stringValue2 != null)
				{
					string[] array2 = stringValue2;
					for (int j = 0; j < array2.Length; j++)
					{
						string buffId2 = array2[j];
						this.CollectBuffGameRes(maxNum, buffId2, ref result);
					}
				}
				if (stringValue3 != null)
				{
					string[] array3 = stringValue3;
					for (int k = 0; k < array3.Length; k++)
					{
						string buffId3 = array3[k];
						this.CollectBuffGameRes(maxNum, buffId3, ref result);
					}
				}
				string[] stringValue4 = StringUtils.GetStringValue(dataById.attach_higheff, ',');
				string[] stringValue5 = StringUtils.GetStringValue(dataById.end_attach_higheff, ',');
				if (stringValue4 != null)
				{
					string[] array4 = stringValue4;
					for (int l = 0; l < array4.Length; l++)
					{
						string higheffId = array4[l];
						this.CollectHigheffGameRes(maxNum, higheffId, ref result);
					}
				}
				if (stringValue5 != null)
				{
					string[] array5 = stringValue5;
					for (int m = 0; m < array5.Length; m++)
					{
						string higheffId2 = array5[m];
						this.CollectHigheffGameRes(maxNum, higheffId2, ref result);
					}
				}
			}
		}

		private void CollectActionGameRes(int maxNum, string id, ref List<SysGameResVo> result)
		{
			SysSkillPerformVo dataById = BaseDataMgr.instance.GetDataById<SysSkillPerformVo>(id);
			if (dataById != null)
			{
				string effect_id = dataById.effect_id;
				if (StringUtils.CheckValid(effect_id))
				{
					SysGameResVo gameResData = BaseDataMgr.instance.GetGameResData(effect_id);
					if (gameResData == null)
					{
						return;
					}
					if (!result.Contains(gameResData))
					{
						if (maxNum > 1 && gameResData.count < Preloader.AoeMaxNum)
						{
							gameResData.count = Preloader.AoeMaxNum;
						}
						if (gameResData.count == 0)
						{
							gameResData.count = 1;
						}
						result.Add(gameResData);
					}
				}
			}
		}

		private void PreloadEffect(List<SysGameResVo> list, int count, int skin = 0)
		{
			if (list != null)
			{
				foreach (SysGameResVo current in list)
				{
					GameObject gameObject = ResourceManager.Load<GameObject>(current.id, true, true, null, skin, false);
					if (gameObject == null)
					{
						gameObject = ResourceManager.Load<GameObject>(current.id, true, true, null, 0, false);
						if (gameObject == null)
						{
							ClientLogger.Error("Error resVo id=" + current.id);
						}
					}
					if (gameObject != null)
					{
						MapManager.Instance.CreatePrefabPoolByTeam(TeamType.None, gameObject.transform, count * current.count, Math.Max(count * current.count * 2, 200), 20, 5);
					}
				}
			}
		}

		private void PreloadEffectCustom(List<SysGameResVo> list, int count, int skin = 0)
		{
			if (list != null)
			{
				foreach (SysGameResVo current in list)
				{
					GameObject gameObject = ResourceManager.Load<GameObject>(current.id, true, true, null, skin, false);
					if (gameObject == null)
					{
						gameObject = ResourceManager.Load<GameObject>(current.id, true, true, null, 0, false);
						if (gameObject == null)
						{
							ClientLogger.Error("Error resVo id=" + current.id);
						}
					}
					if (gameObject != null)
					{
						MapManager.Instance.CreatePrefabPoolByTeam(TeamType.None, gameObject.transform, count, 100, 20, 5);
					}
				}
			}
		}

		[DebuggerHidden]
		public IEnumerator PreloadMonster(TeamType teamType, int count = 1)
		{
			Preloader.<PreloadMonster>c__Iterator1C4 <PreloadMonster>c__Iterator1C = new Preloader.<PreloadMonster>c__Iterator1C4();
			<PreloadMonster>c__Iterator1C.teamType = teamType;
			<PreloadMonster>c__Iterator1C.count = count;
			<PreloadMonster>c__Iterator1C.<$>teamType = teamType;
			<PreloadMonster>c__Iterator1C.<$>count = count;
			<PreloadMonster>c__Iterator1C.<>f__this = this;
			return <PreloadMonster>c__Iterator1C;
		}

		public void PreloadEye()
		{
			GameObject gameObject = ResourceManager.Load<GameObject>("VisionWard", true, true, null, 0, false);
			MapManager.Instance.CreatePrefabPoolByTeam(TeamType.None, gameObject.transform, 10, 200, 20, 5);
		}

		private void PreloadMonster(string monsterStr, TeamType teamType, int count)
		{
			if (monsterStr != "[]")
			{
				string[] waveString = StringUtils.GetWaveString(monsterStr);
				for (int i = 0; i < waveString.Length; i++)
				{
					string text = waveString[i];
					if (!(text == string.Empty))
					{
						string[] stringValue = StringUtils.GetStringValue(text, ',');
						for (int j = 0; j < stringValue.Length; j++)
						{
							if (stringValue[j] != string.Empty)
							{
								string[] stringValue2 = StringUtils.GetStringValue(stringValue[j], '|');
								string text2 = stringValue2[0];
								string correctModelId = this._entityVoCreator.GetCorrectModelId(text2, EntityType.Monster, teamType);
								SysMonsterMainVo monsterMainData = BaseDataMgr.instance.GetMonsterMainData(correctModelId);
								string model_id = monsterMainData.model_id;
								GameObject gameObject = ResourceManager.Load<GameObject>(monsterMainData.model_id, true, true, null, 0, false);
								if (gameObject == null)
								{
									UnityEngine.Debug.LogError("Error resId=" + monsterMainData.model_id);
								}
								GameManager.Instance.SkillData.InsertMonsterSkillData(ref monsterMainData);
								MapManager.Instance.CreatePrefabPoolByTeam(teamType, gameObject.transform, count, 100, 20, 5);
								List<SysGameResVo> list = this.CollectEffectResByMonsterId(text2);
								this.PreloadEffect(list, count, 0);
								this.DoPreloadResources("HUDText", count);
								this.DoPreloadResources("MonsterSlider", count);
								this.DoPreloadResources("SoldierBirth", 1);
							}
						}
					}
				}
			}
		}

		[DebuggerHidden]
		public IEnumerator PreloadMonsterCreep(int count)
		{
			Preloader.<PreloadMonsterCreep>c__Iterator1C5 <PreloadMonsterCreep>c__Iterator1C = new Preloader.<PreloadMonsterCreep>c__Iterator1C5();
			<PreloadMonsterCreep>c__Iterator1C.count = count;
			<PreloadMonsterCreep>c__Iterator1C.<$>count = count;
			<PreloadMonsterCreep>c__Iterator1C.<>f__this = this;
			return <PreloadMonsterCreep>c__Iterator1C;
		}

		[DebuggerHidden]
		public IEnumerator PreloadMapItem()
		{
			Preloader.<PreloadMapItem>c__Iterator1C6 <PreloadMapItem>c__Iterator1C = new Preloader.<PreloadMapItem>c__Iterator1C6();
			<PreloadMapItem>c__Iterator1C.<>f__this = this;
			return <PreloadMapItem>c__Iterator1C;
		}

		public void PreloadSummonerSkill()
		{
			List<SysGameResVo> list = new List<SysGameResVo>();
			int num = 0;
			foreach (ReadyPlayerSampleInfo current in Singleton<PvpManager>.Instance.RoomInfo.PvpPlayers)
			{
				if (current != null)
				{
					SysSummonersSkillVo dataById = BaseDataMgr.instance.GetDataById<SysSummonersSkillVo>(current.selfDefSkillId);
					if (dataById != null)
					{
						this.CollectSkillGameRes(dataById.skill_id, ref list);
					}
					num++;
				}
			}
			this.PreloadEffect(list, num, 0);
		}

		[DebuggerHidden]
		public IEnumerator PreloadOthers()
		{
			return new Preloader.<PreloadOthers>c__Iterator1C7();
		}
	}
}
