using Assets.Scripts.GUILogic.View.PropertyView;
using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using Com.Game.Utils;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.Runes
{
	public class RunesCenterHeroRunes : MonoBehaviour
	{
		private RunesItem runesItem;

		private RunesItem lastRune;

		private PropAttribute propAttribute;

		private Dictionary<string, List<int>> dicValueTypeRunes = new Dictionary<string, List<int>>();

		private Dictionary<string, List<int>> dicGrowthTypeRunes = new Dictionary<string, List<int>>();

		private List<RunesItem> runesList = new List<RunesItem>();

		private Transform transHeroRunes;

		private Transform btnAllDemount;

		private UIGrid gridRunes;

		private UIGrid gridAttributes;

		private Color alphaBtn = new Color(255f, 255f, 255f, 0.4862745f);

		private Color nonAlphaBtn = new Color(255f, 255f, 255f, 1f);

		private Transform transFilterArea;

		private Transform transProcessBar;

		private object[] mgs;

		private int Num;

		private int Rank;

		private int summonerLevel;

		private string heroNPC = string.Empty;

		private void Awake()
		{
			this.mgs = new object[]
			{
				ClientV2C.runesviewChangeToggle,
				ClientV2C.propviewSkipAnotherRune,
				ClientC2V.RunesInlay,
				ClientC2V.RunesDemount,
				ClientC2V.RunesAllDemount
			};
			this.Initialize();
		}

		private void OnEnable()
		{
			this.Register();
		}

		private void OnDisable()
		{
			this.Unregister();
		}

		private void Register()
		{
			MobaMessageManagerTools.RegistMsg(this, this.mgs, true, "OnMsg_");
		}

		private void Unregister()
		{
			MobaMessageManagerTools.RegistMsg(this, this.mgs, false, "OnMsg_");
		}

		private void Start()
		{
		}

		private void Update()
		{
		}

		private void Initialize()
		{
			this.transHeroRunes = base.transform.Find("HeroRunes");
			this.btnAllDemount = this.transHeroRunes.Find("BtnDemountAll");
			this.gridRunes = this.transHeroRunes.Find("Grid").GetComponent<UIGrid>();
			this.gridAttributes = this.transHeroRunes.Find("Attribute/Grid").GetComponent<UIGrid>();
			this.runesItem = Resources.Load<RunesItem>("Prefab/UI/Sacrificial/RunesItem");
			this.propAttribute = Resources.Load<PropAttribute>("Prefab/UI/Sacrificial/PropAttribute");
			this.transFilterArea = base.transform.Find("FilterArea");
			this.transProcessBar = base.transform.Find("Progress Bar");
			UIEventListener.Get(this.btnAllDemount.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickAllDemount);
			this.Num = BaseDataMgr.instance.GetTypeDicByType<SysHeroRunsUnlockVo>().Keys.Count;
		}

		private void OnMsg_runesviewChangeToggle(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				if (Singleton<RunesOverView>.Instance != null)
				{
					this.heroNPC = Singleton<RunesOverView>.Instance.HeroNpc;
				}
				RunesFunctionType runesFunctionType = (RunesFunctionType)((int)msg.Param);
				this.transHeroRunes.gameObject.SetActive(runesFunctionType == RunesFunctionType.Inlay);
				this.btnAllDemount.gameObject.SetActive(runesFunctionType == RunesFunctionType.Inlay);
				this.InitGroove(runesFunctionType == RunesFunctionType.Inlay);
				if (null != this.lastRune)
				{
					this.lastRune = null;
				}
			}
		}

		private void OnMsg_propviewSkipAnotherRune(MobaMessage msg)
		{
			if (msg != null)
			{
				this.SkipToAnotherRune();
			}
		}

		private void OnMsg_RunesInlay(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				int modelid = (int)msg.Param;
				this.lastRune.ChangeRunesTexture(modelid);
				ModelManager.Instance.Set_HeroRunes(this.heroNPC, (int)this.lastRune.RunePosition, modelid);
				this.ChangeAttributeDisplay();
			}
		}

		private void OnMsg_RunesDemount(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				int num = (int)msg.Param;
				this.lastRune.ChangeRunesTexture(0);
				ModelManager.Instance.Set_HeroRunes(this.heroNPC, (int)this.lastRune.RunePosition, 0);
				this.ChangeAttributeDisplay();
			}
		}

		private void OnMsg_RunesAllDemount(MobaMessage msg)
		{
			if (msg != null)
			{
				if (this.runesList != null && this.runesList.Count != 0)
				{
					for (int i = 0; i < this.runesList.Count; i++)
					{
						this.runesList[i].ChangeRunesTexture(0);
					}
				}
				ModelManager.Instance.Set_HeroRunes(this.heroNPC);
				this.ChangeAttributeDisplay();
			}
		}

		private void ClickAllDemount(GameObject obj)
		{
			if (null != obj)
			{
				MobaMessageManagerTools.SendClientMsg(ClientV2C.propviewDemountAll, null, false);
			}
		}

		private void InitGroove(bool isTrue = true)
		{
			if (isTrue)
			{
				this.runesList.Clear();
				UserData userData = ModelManager.Instance.Get_userData_X();
				if (userData != null)
				{
					this.summonerLevel = CharacterDataMgr.instance.GetUserLevel(userData.Exp);
				}
				HeroInfoData heroinfodata = ModelManager.Instance.Get_heroInfo_item_byModelID_X(this.heroNPC);
				if (heroinfodata == null)
				{
					return;
				}
				this.Rank = this.CheckRank(this.summonerLevel);
				Dictionary<string, SysHeroRunsUnlockVo> runes_info = BaseDataMgr.instance.GetTypeDicByType<SysHeroRunsUnlockVo>();
				GridHelper.FillGrid<RunesItem>(this.gridRunes, this.runesItem, this.Num, delegate(int idx, RunesItem comp)
				{
					comp.name = "Level" + runes_info[(idx + 1).ToString()].unlock_level;
					comp.Init(this.Rank, idx, runes_info.Keys.ToList<string>(), heroinfodata);
					comp.ClickCallBack = new Callback<GameObject>(this.RunesExtension);
					if (comp.LimitLock == Limit_lock.unlocked)
					{
						this.runesList.Add(comp);
					}
				});
				this.gridRunes.Reposition();
				this.ChangeAttributeDisplay();
			}
		}

		private void RunesExtension(GameObject obj)
		{
			if (null != obj)
			{
				bool isNeedRefresh = true;
				RunesItem tempRunesItem = obj.GetComponent<RunesItem>();
				if (null != this.lastRune && this.lastRune.RunePosition == tempRunesItem.RunePosition)
				{
					isNeedRefresh = false;
				}
				int limitLock = (int)tempRunesItem.LimitLock;
				if (limitLock == 0)
				{
					Singleton<TipView>.Instance.ShowViewSetText("召唤师等级" + tempRunesItem.RankRune + "级可使用", 1f);
				}
				else if (limitLock == 1)
				{
					GridHelper.FillGrid<RunesItem>(this.gridRunes, this.runesItem, this.Num, delegate(int idx, RunesItem comp)
					{
						comp.ChangeState(tempRunesItem);
					});
					RunesPara runesPara = default(RunesPara);
					this.lastRune = tempRunesItem;
					runesPara.isNeedRefresh = isNeedRefresh;
					runesPara.runesItem = this.lastRune;
					MobaMessageManagerTools.SendClientMsg(ClientV2C.runesviewClickRune, runesPara, false);
				}
			}
		}

		private int CheckRank(int level)
		{
			int result = 0;
			Dictionary<string, SysHeroRunsUnlockVo> typeDicByType = BaseDataMgr.instance.GetTypeDicByType<SysHeroRunsUnlockVo>();
			Dictionary<string, SysHeroRunsUnlockVo>.KeyCollection keys = typeDicByType.Keys;
			List<string> list = keys.ToList<string>();
			for (int num = keys.Count - 1; num != -1; num--)
			{
				if (level >= typeDicByType[list[num]].unlock_level)
				{
					result = int.Parse(list[num]);
					break;
				}
			}
			return result;
		}

		private void ChangeAttributeDisplay()
		{
			this.GetRunesTypes();
			int count = this.dicValueTypeRunes.Count + this.dicGrowthTypeRunes.Count;
			GridHelper.FillGrid<PropAttribute>(this.gridAttributes, this.propAttribute, count, delegate(int idx, PropAttribute comp)
			{
				comp.gameObject.SetActive(true);
				comp.Init(this.dicValueTypeRunes, this.dicGrowthTypeRunes, idx);
			});
			this.gridAttributes.Reposition();
		}

		private void GetRunesTypes()
		{
			this.dicValueTypeRunes.Clear();
			this.dicGrowthTypeRunes.Clear();
			List<int> list = new List<int>();
			HeroInfoData heroInfoData = ModelManager.Instance.Get_heroInfo_item_byModelID_X(this.heroNPC);
			if (heroInfoData != null)
			{
				if (heroInfoData.Ep_1 != 0)
				{
					list.Add(heroInfoData.Ep_1);
				}
				if (heroInfoData.Ep_2 != 0)
				{
					list.Add(heroInfoData.Ep_2);
				}
				if (heroInfoData.Ep_3 != 0)
				{
					list.Add(heroInfoData.Ep_3);
				}
				if (heroInfoData.Ep_4 != 0)
				{
					list.Add(heroInfoData.Ep_4);
				}
				if (heroInfoData.Ep_5 != 0)
				{
					list.Add(heroInfoData.Ep_5);
				}
				if (heroInfoData.Ep_6 != 0)
				{
					list.Add(heroInfoData.Ep_6);
				}
			}
			int num = 0;
			for (int i = 0; i < list.Count; i++)
			{
				num += list[i];
			}
			if (num == 0)
			{
				this.btnAllDemount.GetComponent<UISprite>().alpha = 0.4862745f;
				this.btnAllDemount.GetComponent<UIButton>().defaultColor = this.alphaBtn;
				this.btnAllDemount.GetComponent<UIButton>().hover = this.alphaBtn;
				this.btnAllDemount.GetComponent<UIButton>().pressed = this.alphaBtn;
			}
			else
			{
				this.btnAllDemount.GetComponent<UISprite>().alpha = 1f;
				this.btnAllDemount.GetComponent<UIButton>().defaultColor = this.nonAlphaBtn;
				this.btnAllDemount.GetComponent<UIButton>().hover = this.nonAlphaBtn;
				this.btnAllDemount.GetComponent<UIButton>().pressed = this.nonAlphaBtn;
			}
			this.ParseData(list);
		}

		private void ParseData(List<int> data)
		{
			for (int num = 0; num != data.Count; num++)
			{
				SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(data[num].ToString());
				string[] array = null;
				string[] array2 = null;
				if (dataById != null)
				{
					array = dataById.attribute.Split(new char[]
					{
						','
					});
				}
				if (array != null && array.Count<string>() != 0 && array.Count<string>() == 1)
				{
					array2 = array[0].Split(new char[]
					{
						'|'
					});
				}
				if (dataById.rune_type == 1)
				{
					if (this.dicValueTypeRunes.ContainsKey(array2[0]))
					{
						this.dicValueTypeRunes[array2[0]].Add(data[num]);
					}
					else
					{
						this.dicValueTypeRunes[array2[0]] = new List<int>
						{
							data[num]
						};
					}
				}
				if (dataById.rune_type == 2)
				{
					if (this.dicGrowthTypeRunes.ContainsKey(array2[0]))
					{
						this.dicGrowthTypeRunes[array2[0]].Add(data[num]);
					}
					else
					{
						this.dicGrowthTypeRunes[array2[0]] = new List<int>
						{
							data[num]
						};
					}
				}
			}
		}

		private void SkipToAnotherRune()
		{
			List<RunesItem> list = new List<RunesItem>();
			if (this.gridRunes.transform.childCount > 0)
			{
				for (int i = 0; i < this.gridRunes.transform.childCount; i++)
				{
					list.Add(this.gridRunes.transform.GetChild(i).GetComponent<RunesItem>());
				}
			}
			list = list.FindAll((RunesItem obj) => obj.LimitLock == Limit_lock.unlocked && obj.ModelID == 0);
			int num = -1;
			for (int num2 = 0; num2 != list.Count; num2++)
			{
				if (list[num2].RunePosition > this.lastRune.RunePosition)
				{
					num = num2;
					break;
				}
				num = -1;
			}
			if (list.Count > 0 && num == -1)
			{
				num = 0;
			}
			if (num == -1)
			{
				return;
			}
			this.lastRune = list[num];
			bool isNeedRefresh = true;
			GridHelper.FillGrid<RunesItem>(this.gridRunes, this.runesItem, this.Num, delegate(int idx, RunesItem comp)
			{
				comp.ChangeState(this.lastRune);
			});
			RunesPara runesPara = default(RunesPara);
			runesPara.isNeedRefresh = isNeedRefresh;
			runesPara.runesItem = this.lastRune;
			MobaMessageManagerTools.SendClientMsg(ClientV2C.runesviewClickRune, runesPara, false);
		}
	}
}
