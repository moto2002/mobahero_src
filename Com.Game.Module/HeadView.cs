using Com.Game.Data;
using Com.Game.Manager;
using GUIFramework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Game.Module
{
	public class HeadView : BaseView<HeadView>
	{
		private Transform HeadPanel;

		private UISprite Bg;

		private Transform ShieldBlank;

		private Dictionary<string, HeadItem> headItems = new Dictionary<string, HeadItem>();

		private List<string> m_Playerlist;

		private Units m_Player;

		private VTrigger ChangePlayerTrigger;

		private VTrigger RestarTrigger;

		public HeadView()
		{
			this.WinResCfg = new WinResurceCfg(true, true, "HeadView");
		}

		public override void Init()
		{
			base.Init();
			this.HeadPanel = this.transform.FindChild("Anchor/Panel");
			this.ShieldBlank = this.transform.FindChild("Anchor/ShieldBlank");
			this.Bg = this.transform.Find("Anchor/Bg").GetComponent<UISprite>();
			this.ShieldBlank.gameObject.SetActive(false);
		}

		public override void RegisterUpdateHandler()
		{
			this.RegisterTrigger();
			this.ChangePlayer();
		}

		public override void CancelUpdateHandler()
		{
			this.UnRegisterTrigger();
		}

		public override void RefreshUI()
		{
			this.UpdateHeadView();
		}

		public override void Destroy()
		{
			this.UnRegisterTrigger();
			this.m_Playerlist = null;
			this.headItems.Clear();
			base.Destroy();
		}

		private void RegisterTrigger()
		{
			this.ChangePlayerTrigger = TriggerManager.CreateGameEventTrigger(GameEvent.ChangePlayer, null, new TriggerAction(this.ChangePlayer));
			this.RestarTrigger = TriggerManager.CreateGameEventTrigger(GameEvent.GameRestart, null, new TriggerAction(this.ReplayHeadView));
		}

		private void UnRegisterTrigger()
		{
			TriggerManager.DestroyTrigger(this.ChangePlayerTrigger);
			TriggerManager.DestroyTrigger(this.RestarTrigger);
			this.ChangePlayerTrigger = null;
			this.RestarTrigger = null;
		}

		public void ChangePlayer()
		{
			this.m_Player = PlayerControlMgr.Instance.GetPlayer();
			this.m_Playerlist = GameManager.Instance.Spawner.GetHeroNames(TeamType.LM);
			this.RefreshUI();
		}

		private void UpdateHeadView()
		{
			if (this.m_Player == null)
			{
				return;
			}
			if (this.HeadPanel == null)
			{
				return;
			}
			int childCount = this.HeadPanel.childCount;
			int num = 0;
			for (int i = 0; i < this.m_Playerlist.Count; i++)
			{
				if (this.m_Playerlist[i] != string.Empty)
				{
					SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(this.m_Playerlist[i]);
					GameObject gameObject;
					if (num >= childCount)
					{
						gameObject = NGUITools.AddChild(this.HeadPanel.gameObject, base.LoadPrefabCache("HeadItem"));
						gameObject.AddComponent<HeadItem>();
					}
					else
					{
						gameObject = this.HeadPanel.GetChild(num).gameObject;
						gameObject.SetActive(true);
					}
					HeadItem component = gameObject.GetComponent<HeadItem>();
					component.UpdateItem(heroMainData.avatar_icon, this.m_Playerlist[i].Equals(this.m_Player.npc_id), heroMainData.npc_id);
					if (!this.headItems.ContainsKey(this.m_Playerlist[i]))
					{
						this.headItems.Add(this.m_Playerlist[i], component);
					}
					num++;
				}
			}
			for (int j = num; j < childCount; j++)
			{
				GameObject gameObject2 = this.HeadPanel.GetChild(j).gameObject;
				gameObject2.SetActive(false);
			}
			this.Bg.SetDimensions(112, 120 * num + 20);
			this.HeadPanel.GetComponent<UIGrid>().Reposition();
		}

		public void UpdateHeroValue(string player_id, float hp_value, float mp_value)
		{
			if (player_id != null && this.headItems.ContainsKey(player_id))
			{
				HeadItem headItem = this.headItems[player_id];
				headItem.UpdateValue(hp_value, mp_value);
			}
		}

		public void ReplayHeadView()
		{
			for (int i = 0; i < this.HeadPanel.transform.childCount; i++)
			{
				GameObject gameObject = this.HeadPanel.GetChild(i).gameObject;
				gameObject.GetComponent<HeadItem>().NormalHeroHead();
			}
		}

		public void ShieldScreen()
		{
			if (this.ShieldBlank)
			{
				this.ShieldBlank.gameObject.SetActive(true);
			}
		}
	}
}
