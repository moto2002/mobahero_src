using Assets.Scripts.Model;
using GUIFramework;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using UnityEngine;

namespace HUD.Module
{
	public class DebugModule : BaseModule
	{
		private Transform DebugObj;

		private Transform SpeedBtn;

		private UILabel SpeedLabel;

		private Transform ResetSpeedBtn;

		private Transform LevelBtn;

		public Transform EnableHeroAIBnt;

		public Transform EnableMonsterAIBnt;

		public Transform EnableTowerAIBnt;

		public Transform EnableAllAIBnt;

		public Transform NoMonsterBnt;

		public Transform NoSkillCdBnt;

		public Transform NoSkillCostBnt;

		public Transform WhoIsYourDady;

		public Transform AddBlood;

		public Transform NoDamage;

		public Transform NoAutoAttack;

		public Transform Pets;

		private float[] pvpSpeeds = new float[]
		{
			1f,
			0.5f,
			0.2f,
			0.1f,
			0.05f,
			0.02f,
			0.01f,
			0.001f
		};

		private int pvpSpeed;

		public DebugModule()
		{
			this.module = EHUDModule.Debug;
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/HUDModule/DebugModule");
		}

		public override void Init()
		{
			base.Init();
			this.DebugObj = this.transform.Find("Container");
			this.SpeedBtn = this.DebugObj.Find("SpeedBtn");
			this.SpeedLabel = this.SpeedBtn.Find("speed").GetComponent<UILabel>();
			this.ResetSpeedBtn = this.DebugObj.Find("ResetSpeedBtn");
			this.LevelBtn = this.DebugObj.Find("LevelBtn");
			this.EnableHeroAIBnt = this.DebugObj.Find("Toggle/EnableHeroAI");
			this.EnableMonsterAIBnt = this.DebugObj.Find("Toggle/EnableMonsterAI");
			this.EnableTowerAIBnt = this.DebugObj.Find("Toggle/EnableTowerAI");
			this.EnableAllAIBnt = this.DebugObj.Find("Toggle/EnableAllAI");
			this.NoMonsterBnt = this.DebugObj.Find("Toggle/NoMontser");
			this.NoSkillCdBnt = this.DebugObj.Find("Toggle/NoSkillCd");
			this.NoSkillCostBnt = this.DebugObj.Find("Toggle/NoSkillCost");
			this.WhoIsYourDady = this.DebugObj.Find("Toggle/WhoIsYourDady");
			this.AddBlood = this.DebugObj.Find("Toggle/AddBlood");
			this.NoDamage = this.DebugObj.Find("Toggle/NoDamage");
			this.NoAutoAttack = this.DebugObj.Find("Toggle/NoAutoAttack");
			this.Pets = this.DebugObj.Find("Toggle/Pets");
			UIEventListener.Get(this.SpeedBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnSpeedBtn);
			UIEventListener.Get(this.ResetSpeedBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnResetSpeedBtn);
			UIEventListener.Get(this.LevelBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnLevelBtn);
			UIEventListener.Get(this.EnableHeroAIBnt.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnGameCommand);
			UIEventListener.Get(this.EnableTowerAIBnt.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnGameCommand);
			UIEventListener.Get(this.EnableMonsterAIBnt.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnGameCommand);
			UIEventListener.Get(this.EnableAllAIBnt.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnGameCommand);
			UIEventListener.Get(this.NoMonsterBnt.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnGameCommand);
			UIEventListener.Get(this.NoSkillCdBnt.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnGameCommand);
			UIEventListener.Get(this.NoSkillCostBnt.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnGameCommand);
			UIEventListener.Get(this.WhoIsYourDady.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnGameCommand);
			UIEventListener.Get(this.AddBlood.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnGameCommand);
			UIEventListener.Get(this.NoDamage.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnGameCommand);
			UIEventListener.Get(this.NoAutoAttack.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnGameCommand);
			UIEventListener.Get(this.Pets.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnGameCommand);
		}

		private void OnGameCommand(GameObject go)
		{
			if (!(go.name == "NoMontser"))
			{
				if (go.name == "NoSkillCd")
				{
					GlobalSettings.IsNoSkillCD = !GlobalSettings.IsNoSkillCD;
				}
				else if (go.name == "NoSkillCost")
				{
					GlobalSettings.NoCost = !GlobalSettings.NoCost;
				}
				else if (!(go.name == "EnableHeroAI"))
				{
					if (!(go.name == "EnableMonsterAI"))
					{
						if (!(go.name == "EnableTowerAI"))
						{
							if (!(go.name == "EnableAllAI"))
							{
								if (go.name == "Pets")
								{
								}
							}
						}
					}
				}
			}
			SendMsgManager.Instance.SendPvpMsgBase<CheatInfo>(PvpCode.C2P_GMCheat, new CheatInfo
			{
				cheatMsg = "GameCommand " + go.name
			});
		}

		private void OnSpeedBtn(GameObject obj = null)
		{
			this.pvpSpeed++;
			if (this.pvpSpeed >= this.pvpSpeeds.Length)
			{
				this.pvpSpeed = 0;
			}
			this.SetPvpSpeed(this.pvpSpeeds[this.pvpSpeed]);
			SendMsgManager.Instance.SendPvpMsgBase<CheatInfo>(PvpCode.C2P_GMCheat, new CheatInfo
			{
				cheatMsg = "speed " + this.SpeedLabel.text
			});
		}

		private void OnResetSpeedBtn(GameObject obj = null)
		{
			this.pvpSpeed = 0;
			this.SetPvpSpeed(this.pvpSpeeds[this.pvpSpeed]);
			SendMsgManager.Instance.SendPvpMsgBase<CheatInfo>(PvpCode.C2P_GMCheat, new CheatInfo
			{
				cheatMsg = "speed " + this.SpeedLabel.text
			});
		}

		public void SetPvpSpeed(float speed)
		{
			this.SpeedLabel.text = speed.ToString();
			Time.timeScale = speed;
		}

		private void OnLevelBtn(GameObject obj = null)
		{
			GameObject gameObject = GameObject.FindGameObjectWithTag("Player");
			if (gameObject != null)
			{
				Units component = gameObject.GetComponent<Units>();
				UtilCounter counter = UtilManager.Instance.GetCounter(UtilType.Exp);
				ExpValue expValue = counter.GetValue(PlayerControlMgr.Instance.GetPlayer().unique_id) as ExpValue;
				int lvUpExp = expValue.GetLvUpExp(expValue.CurLv + 1);
				expValue.AddExp((float)lvUpExp);
				SendMsgManager.Instance.SendPvpMsgBase<CheatInfo>(PvpCode.C2P_GMCheat, new CheatInfo
				{
					cheatMsg = "AddExp " + lvUpExp
				});
			}
		}
	}
}
