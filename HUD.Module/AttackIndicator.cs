using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using GUIFramework;
using MobaHeros.Pvp;
using System;
using UnityEngine;

namespace HUD.Module
{
	public class AttackIndicator : BaseModule
	{
		private Transform heroTargetInfo;

		private string heroName;

		private UISprite heroIcon;

		private UILabel heroKillInfo;

		private Transform otherTargetInfo;

		private UISprite otherIcon;

		private UIBloodBarLightWeight _heroTarget;

		private UIBloodBarLightWeight _monsterTarget;

		private UILabel otherName;

		private TweenPosition tPos;

		private Vector3 positionShow = new Vector3(-756.2f, -6.9f, 0f);

		private Vector3 positionHide = new Vector3(-756.2f, 493.1f, 0f);

		public AttackIndicator()
		{
			this.module = EHUDModule.AttackIndicator;
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/HUDModule/AttackIndicatorModule");
		}

		public override void Init()
		{
			base.Init();
			this.heroTargetInfo = this.transform.Find("offset/HeroTarget");
			this.heroIcon = this.heroTargetInfo.Find("Sprite/Texture").GetComponent<UISprite>();
			this._heroTarget = this.heroTargetInfo.Find("HPBar").GetComponent<UIBloodBarLightWeight>();
			this.heroKillInfo = this.heroTargetInfo.Find("KillInfo/KillNumber").GetComponent<UILabel>();
			this.otherTargetInfo = this.transform.Find("offset/OtherTarget");
			this.otherIcon = this.otherTargetInfo.Find("Sprite/Texture").GetComponent<UISprite>();
			this._monsterTarget = this.otherTargetInfo.Find("HPBar/HPBar").GetComponent<UIBloodBarLightWeight>();
			this.otherName = this.otherTargetInfo.Find("Name/name").GetComponent<UILabel>();
			this.tPos = this.transform.GetComponent<TweenPosition>();
			this.SetHeroIndicatorActive(false);
			this.SetOtherIndicatorActive(false);
		}

		public override void RegisterUpdateHandler()
		{
			base.RegisterUpdateHandler();
			MobaMessageManager.RegistMessage((ClientMsg)25059, new MobaMessageFunc(this.OnMsgGetTarget));
		}

		public override void CancelUpdateHandler()
		{
			base.CancelUpdateHandler();
			MobaMessageManager.UnRegistMessage((ClientMsg)25059, new MobaMessageFunc(this.OnMsgGetTarget));
		}

		public override void onFlyOut()
		{
			this.tPos.PlayForward();
		}

		public override void onFlyIn()
		{
			this.tPos.PlayReverse();
		}

		private void SetHeroIndicatorActive(bool isActive)
		{
			this.SetVisibleByTranslate(this.heroTargetInfo.transform, isActive);
			if (!isActive)
			{
				this._heroTarget.SetTarget(null);
			}
		}

		private void SetOtherIndicatorActive(bool isActive)
		{
			this.SetVisibleByTranslate(this.otherTargetInfo.transform, isActive);
			if (!isActive)
			{
				this._monsterTarget.SetTarget(null);
			}
		}

		private void SetVisibleByTranslate(Transform trans, bool visible)
		{
			if (visible)
			{
				trans.localPosition = this.positionShow;
			}
			else
			{
				trans.localPosition = this.positionHide;
			}
		}

		public override void Destroy()
		{
			this.heroTargetInfo = null;
			this.heroIcon = null;
			this.heroKillInfo = null;
			this.otherTargetInfo = null;
			this.otherIcon = null;
			this.otherName = null;
			base.Destroy();
		}

		private void OnMsgGetTarget(MobaMessage msg)
		{
			Units target = msg.Param as Units;
			this.ShowTargetInfo(target);
		}

		public void ShowTargetInfo(Units target)
		{
			if (target == null)
			{
				if (this.otherTargetInfo == null || this.heroTargetInfo == null)
				{
					return;
				}
				this.SetHeroIndicatorActive(false);
				this.SetOtherIndicatorActive(false);
				return;
			}
			else
			{
				if (target.teamType == PlayerControlMgr.Instance.GetPlayer().teamType)
				{
					return;
				}
				if (target.isHero)
				{
					this.ShowHeroTargetInfo(target);
				}
				else
				{
					this.ShowOtherTargetInfo(target);
				}
				return;
			}
		}

		private void ShowHeroTargetInfo(Units hero)
		{
			this.SetOtherIndicatorActive(false);
			this.SetHeroIndicatorActive(true);
			if (this.heroName != hero.name)
			{
				this.heroName = hero.name;
				SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(hero.npc_id);
				if (heroMainData != null)
				{
					this.heroIcon.spriteName = heroMainData.avatar_icon;
				}
			}
			PvpStatisticMgr.HeroData heroData = Singleton<PvpManager>.Instance.StatisticMgr.GetHeroData(hero.unique_id);
			string text = heroData.MonsterKill.ToString();
			string text2 = string.Concat(new object[]
			{
				heroData.HeroKill,
				"/",
				heroData.Death,
				"/",
				heroData.Assist
			});
			int num = text.Length + text2.Length;
			for (int i = 0; i < 12 - num; i++)
			{
				text += " ";
			}
			this.heroKillInfo.text = text + text2;
			this._heroTarget.SetTarget(hero);
		}

		private void ShowOtherTargetInfo(Units other)
		{
			if (this.heroTargetInfo == null)
			{
				return;
			}
			this.SetOtherIndicatorActive(true);
			this.SetHeroIndicatorActive(false);
			if (this.heroName != other.name)
			{
				this.heroName = other.name;
				SysMonsterMainVo monsterMainData = BaseDataMgr.instance.GetMonsterMainData(other.npc_id);
				if (monsterMainData != null)
				{
					if (other.isTower)
					{
						if (other.TeamType == TeamType.LM)
						{
							this.otherIcon.spriteName = "TowerBlue_Avatar";
						}
						else
						{
							this.otherIcon.spriteName = "TowerRed_Avatar";
						}
					}
					else if (other.isHome)
					{
						if (other.TeamType == TeamType.LM)
						{
							this.otherIcon.spriteName = "GemBlue_Avatar";
						}
						else
						{
							this.otherIcon.spriteName = "GemRed_Avatar";
						}
					}
					else
					{
						this.otherIcon.spriteName = monsterMainData.avatar_icon;
					}
					this.otherName.text = LanguageManager.Instance.GetStringById(monsterMainData.name);
				}
			}
			this._monsterTarget.SetTarget(other);
		}
	}
}
