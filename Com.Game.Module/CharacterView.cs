using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Utils;
using GUIFramework;
using MobaProtocol.Data;
using System;
using UnityEngine;

namespace Com.Game.Module
{
	public class CharacterView : BaseView<CharacterView>
	{
		private bool alwaysShow = true;

		private readonly HudBarRecycler _barRecycler = new HudBarRecycler();

		private readonly ObjectRecycler<HUDText> _hudTextRecycler;

		private readonly ObjectRecycler<UIJumpWord> _jumpwordRecycler;

		private Transform HUDBarNode;

		private Transform HUDTextNode;

		private Transform _heroHudBars;

		public CharacterView()
		{
			this.WinResCfg = new WinResurceCfg(true, true, "CharacterView");
			this._hudTextRecycler = new ObjectRecycler<HUDText>(new Func<HUDText>(this.NewHudText), null);
			this._jumpwordRecycler = new ObjectRecycler<UIJumpWord>(delegate
			{
				UIJumpWord uIJumpWord = ResourceManager.LoadPath<UIJumpWord>("Prefab/UI/HUDText/JumpWord", null, 0);
				if (uIJumpWord == null)
				{
					ClientLogger.Error("UIJumpWordPrefab is Null跳字预设为空（没有找到）");
					return null;
				}
				return UnityEngine.Object.Instantiate(uIJumpWord) as UIJumpWord;
			}, new Action<UIJumpWord>(UnityEngine.Object.Destroy));
		}

		public void ShowHpBar(bool always)
		{
			this.alwaysShow = always;
			if (this.HUDBarNode)
			{
				this.HUDBarNode.gameObject.SetActive(always);
			}
		}

		public override void Init()
		{
			base.Init();
			this.HUDBarNode = this.transform.FindChild("HUDBar");
			this.HUDTextNode = this.transform.FindChild("HUDText");
			this.ShowHpBar(this.alwaysShow);
			RecyclerManager.AddOnExit(delegate
			{
				this._barRecycler.ReleaseAll();
			});
			RecyclerManager.AddOnExit(delegate
			{
				this._hudTextRecycler.DestroyPool();
			});
			RecyclerManager.AddOnExit(delegate
			{
				this._jumpwordRecycler.DestroyPool();
			});
		}

		public override void HandleAfterOpenView()
		{
			base.HandleAfterOpenView();
			this._jumpwordRecycler.Preload(10);
		}

		public override void RefreshUI()
		{
		}

		public override void Destroy()
		{
			this.HUDBarNode = null;
			this.HUDTextNode = null;
			base.Destroy();
		}

		public void SetBarParent(GameObject obj)
		{
			obj.transform.parent = this.HUDBarNode;
			obj.transform.localScale = Vector3.one;
			obj.transform.localPosition = Vector3.zero;
		}

		public void SetTextParent(GameObject obj)
		{
			obj.transform.parent = this.HUDTextNode;
			obj.transform.localScale = Vector3.one;
			obj.transform.localPosition = Vector3.zero;
		}

		private void UpdateHUDBar(Units player)
		{
			if (player.mHpBar)
			{
				player.mHpBar.UpdateHeroLevel(player.level);
			}
		}

		private Transform GetHudBarParent(Units player)
		{
			if (player.isHero)
			{
				if (!this._heroHudBars)
				{
					this._heroHudBars = NGUITools.AddChild(this.HUDBarNode.gameObject).transform;
					this._heroHudBars.SetDebugName("herohuds");
				}
				return this._heroHudBars;
			}
			int num = 4;
			Transform transform = null;
			int childCount = this.HUDBarNode.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Transform child = this.HUDBarNode.GetChild(i);
				if (!(child == this._heroHudBars))
				{
					if (child != null && child.childCount < num)
					{
						transform = child;
						break;
					}
				}
			}
			if (transform != null)
			{
				return transform;
			}
			GameObject gameObject = new GameObject
			{
				layer = Layer.NGUILayer
			};
			gameObject.SetDebugName("HudBarPanelRoot");
			UIPanel uIPanel = gameObject.AddComponent<UIPanel>();
			uIPanel.depth = 100;
			Transform transform2 = gameObject.transform;
			transform2.parent = this.HUDBarNode;
			transform2.localPosition = Vector3.zero;
			transform2.localRotation = Quaternion.identity;
			transform2.localScale = Vector3.one;
			return transform2;
		}

		public void DestroyHudBar(Units self)
		{
			if (self && self.mHpBar)
			{
				self.mHpBar.On_Spawn();
				this._barRecycler.Release(self.mHpBar);
			}
		}

		public UIBloodBar CreateHudBar(Units player)
		{
			ClientLogger.AssertNotNull(player, null);
			if (player == null)
			{
				return null;
			}
			if (!this.IsOpened)
			{
				CtrlManager.OpenWindow(WindowID.CharacterView, null);
			}
			this.TryShow();
			UIBloodBar uIBloodBar;
			if (player.mHpBar)
			{
				uIBloodBar = player.mHpBar;
			}
			else
			{
				Transform hudBarParent = this.GetHudBarParent(player);
				if (player.isHero)
				{
					if (player.tag == "Player")
					{
						uIBloodBar = this._barRecycler.Create("MainHeroSlider", hudBarParent, player);
					}
					else
					{
						uIBloodBar = this._barRecycler.Create("HeroSlider", hudBarParent, player);
					}
				}
				else if (TagManager.CheckTag(player, global::TargetTag.EyeUnit))
				{
					uIBloodBar = this._barRecycler.Create("EyeSlider", hudBarParent, player);
				}
				else if (player.isMonster)
				{
					if (player.isCreep)
					{
						uIBloodBar = this._barRecycler.Create("CreepSlider", hudBarParent, player);
					}
					else
					{
						if (player.UnitType == UnitType.Pet)
						{
							return null;
						}
						if (player.UnitType == UnitType.SummonMonster)
						{
							uIBloodBar = this._barRecycler.Create("SummonerMonsterSlider", hudBarParent, player);
						}
						else
						{
							uIBloodBar = this._barRecycler.Create("MonsterSlider", hudBarParent, player);
						}
					}
				}
				else
				{
					if (!player.isTower && !player.isHome)
					{
						return null;
					}
					uIBloodBar = this._barRecycler.Create("TowerSlider", hudBarParent, player);
				}
				uIBloodBar.name = player.name;
				uIBloodBar.SetTargetUnit(player);
				if (LevelManager.Instance.IsPvpBattleType && (player.isHero || player.isCreep))
				{
					if (player.isHero)
					{
						uIBloodBar.ShowName(true, player.summonerName, player.unique_id);
					}
					else
					{
						SysMonsterMainVo monsterMainData = BaseDataMgr.instance.GetMonsterMainData(player.npc_id);
						uIBloodBar.ShowName(true, LanguageManager.Instance.GetStringById(monsterMainData.name), 0);
					}
				}
				else if (player.isHero && player.tag != "Player")
				{
					SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(player.npc_id);
					uIBloodBar.ShowName(true, LanguageManager.Instance.GetStringById(heroMainData.name) + LanguageManager.Instance.GetStringById("BattleAiText_Computer"), 0);
				}
				else if (player.isCreep)
				{
					SysMonsterMainVo monsterMainData2 = BaseDataMgr.instance.GetMonsterMainData(player.npc_id);
					uIBloodBar.ShowName(true, LanguageManager.Instance.GetStringById(monsterMainData2.name), 0);
				}
				else
				{
					uIBloodBar.ShowName(false, null, 0);
				}
				uIBloodBar.transform.localScale = player.surface.HudbarLocalScale;
			}
			uIBloodBar.UpdateHudBarType(player);
			return uIBloodBar;
		}

		private void TryShow()
		{
			if (Singleton<CharacterView>.Instance == null)
			{
				CtrlManager.OpenWindow(WindowID.CharacterView, null);
			}
			if (this.gameObject == null)
			{
				CtrlManager.OpenWindow(WindowID.CharacterView, null);
			}
		}

		public HUDText CreateHudText(Units player)
		{
			if (player == null)
			{
				return null;
			}
			this.TryShow();
			HUDText hUDText = this._hudTextRecycler.Create(this.HUDTextNode);
			hUDText.SetDebugName(player.gameObject.name);
			UIFollowTarget component = hUDText.gameObject.GetComponent<UIFollowTarget>();
			component.target = player.transform;
			return hUDText;
		}

		public void DestroyHudText(Units self)
		{
			if (self)
			{
				this._hudTextRecycler.Release(self.mText);
			}
		}

		private HUDText NewHudText()
		{
			Transform transform = MapManager.Instance.SpawnUI("HUDText", this.HUDTextNode);
			GameObject gameObject = transform.gameObject;
			HUDText componentInChildren = gameObject.GetComponentInChildren<HUDText>();
			if (componentInChildren != null)
			{
				UIFollowTarget x = componentInChildren.gameObject.GetComponent<UIFollowTarget>();
				if (x == null)
				{
					x = gameObject.AddComponent<UIFollowTarget>();
				}
				return componentInChildren;
			}
			return null;
		}

		public UIJumpWord CreateJumpWord(Transform parent)
		{
			return this._jumpwordRecycler.Create(parent);
		}

		public void DestroyJumpWord(UIJumpWord word)
		{
			this._jumpwordRecycler.Release(word);
		}
	}
}
