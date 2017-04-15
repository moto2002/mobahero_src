using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using Com.Game.Utils;
using MobaHeros.Replay;
using System;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.GameVideo
{
	public class GameVideo_LocalRecordItem : MonoBehaviour
	{
		public UISprite BackGround;

		public UISprite SelectFrame;

		public GameObject DeleteButton;

		public GameObject MvpIcon;

		public UITexture HeroTexture;

		public UILabel GameModeLabel;

		public UILabel HeroNameLabel;

		public UILabel GameTimeLabel;

		public UILabel KillNumLabel;

		public UILabel DeathNumLabel;

		public UILabel AssistNumLabel;

		public int ReplayId;

		public bool IsMvp
		{
			get
			{
				return this.MvpIcon.activeInHierarchy;
			}
			set
			{
				this.MvpIcon.SetActive(value);
			}
		}

		public string Kill
		{
			get
			{
				return this.KillNumLabel.text;
			}
			set
			{
				this.KillNumLabel.text = value;
			}
		}

		public string Death
		{
			get
			{
				return this.DeathNumLabel.text;
			}
			set
			{
				this.DeathNumLabel.text = value;
			}
		}

		public string Assist
		{
			get
			{
				return this.AssistNumLabel.text;
			}
			set
			{
				this.AssistNumLabel.text = value;
			}
		}

		public string GameMode
		{
			get
			{
				return this.GameModeLabel.text;
			}
			set
			{
				this.GameModeLabel.text = value;
			}
		}

		public SysHeroMainVo Hero
		{
			set
			{
				if (value == null)
				{
					this.HeroNameLabel.text = "N/A";
					this.HeroTexture.mainTexture = ResourceManager.Load<Texture>("Longavatar_Huonv", true, true, null, 0, false);
					return;
				}
				this.HeroNameLabel.text = LanguageManager.Instance.GetStringById(value.name);
				this.HeroTexture.mainTexture = ResourceManager.Load<Texture>(value.longAvatar_icon, true, true, null, 0, false);
			}
		}

		public string GameTime
		{
			set
			{
				this.GameTimeLabel.text = value;
			}
		}

		private void OnEnable()
		{
			this.DeleteButton.gameObject.SetActive(false);
			this.SelectFrame.gameObject.SetActive(false);
			UIEventListener.Get(this.BackGround.gameObject).onPress = new UIEventListener.BoolDelegate(this.OnPress_ShowFrame);
			UIEventListener.Get(this.BackGround.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClick_Play);
			UIEventListener.Get(this.DeleteButton.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClick_Delete);
			MobaMessageManager.RegistMessage((ClientMsg)26030, new MobaMessageFunc(this.OnMsg_ShowDeleteBtn));
		}

		private void OnDisable()
		{
			MobaMessageManager.UnRegistMessage((ClientMsg)26030, new MobaMessageFunc(this.OnMsg_ShowDeleteBtn));
		}

		private void OnDestroy()
		{
			this.HeroTexture.mainTexture = null;
		}

		private void OnMsg_ShowDeleteBtn(MobaMessage msg)
		{
			bool active = (bool)msg.Param;
			this.DeleteButton.SetActive(active);
		}

		private void OnPress_ShowFrame(GameObject obj, bool isPress)
		{
			this.SelectFrame.gameObject.SetActive(isPress);
		}

		private void OnClick_Play(GameObject obj)
		{
			CtrlManager.ShowMsgBox("战场回放", "确定要打开本场战斗回放吗？（加载战斗回放不消耗流量）", delegate(bool _isConfirm)
			{
				if (!_isConfirm)
				{
					return;
				}
				MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)26032, this.ReplayId, 0f);
				MobaMessageManager.ExecuteMsg(message);
			}, PopViewType.PopTwoButton, "确定", "取消", null);
		}

		private void OnClick_Delete(GameObject obj)
		{
			CtrlManager.ShowMsgBox("删除确认", "确定删除本场战斗回放吗？", delegate(bool _isConfirm)
			{
				if (!_isConfirm)
				{
					return;
				}
				MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)26031, this, 0f);
				MobaMessageManager.ExecuteMsg(message);
			}, PopViewType.PopTwoButton, "确定", "取消", null);
		}

		public void SetData(ReplayMetaInfo.ReplayMetaEntry replayInfo)
		{
			if (replayInfo == null)
			{
				return;
			}
			this.ReplayId = replayInfo.ReplayId;
			string extra = replayInfo.Extra;
			if (extra == null)
			{
				return;
			}
			string[] array = extra.Split(new char[]
			{
				'|'
			});
			if (array.Length != 7)
			{
				ClientLogger.Error("RecordItem: Data Unknown: " + extra);
				return;
			}
			this.Kill = array[0];
			this.Death = array[1];
			this.Assist = array[2];
			this.GameMode = array[3];
			this.GameTime = array[5];
			this.IsMvp = array[6].Equals("1");
			SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(array[4]);
			this.Hero = heroMainData;
		}
	}
}
