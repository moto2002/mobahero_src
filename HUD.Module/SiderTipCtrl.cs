using Com.Game.Module;
using Com.Game.Utils;
using MobaHeros.Pvp;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HUD.Module
{
	public class SiderTipCtrl : MonoBehaviour
	{
		[Serializable]
		public class PlayerIcon
		{
			public UISprite icon;

			[NonSerialized]
			public UISprite border;

			public void AssignBorder()
			{
				this.border = this.icon.parent.GetComponent<UISprite>();
			}
		}

		[SerializeField]
		private bool isRightSiderTip;

		[SerializeField]
		private UISprite background;

		[SerializeField]
		private UISprite actionIcon;

		[SerializeField]
		private UISprite signIcon;

		[SerializeField]
		private SiderTipCtrl.PlayerIcon attacker;

		[SerializeField]
		private SiderTipCtrl.PlayerIcon victim;

		[SerializeField]
		private List<SiderTipCtrl.PlayerIcon> assist;

		[HideInInspector]
		public UIWidget selfWidget;

		[HideInInspector]
		public TweenPosition tweenPos;

		[HideInInspector]
		public TweenAlpha tweenAlpha;

		[HideInInspector]
		public BattleMsgState state;

		[HideInInspector]
		public Vector3 defaultPos = Vector3.zero;

		private Vector3 currentPos = Vector3.zero;

		private static Color32 Green = new Color32(24, 154, 25, 255);

		private static Color32 Red = new Color32(220, 0, 20, 255);

		private static Color32 Yellow = new Color32(220, 220, 0, 255);

		private static Color32 Magenta = new Color32(219, 0, 155, 255);

		private static Color32 Cyan = new Color32(38, 230, 231, 255);

		private static Color32 Yellow_T3 = new Color32(244, 224, 49, 255);

		private void Awake()
		{
			this.selfWidget = base.transform.GetComponent<UIWidget>();
			this.tweenPos = base.transform.GetComponent<TweenPosition>();
			this.tweenAlpha = base.transform.GetComponent<TweenAlpha>();
			this.state = BattleMsgState.Free;
			for (int i = 0; i < this.assist.Count; i++)
			{
				this.assist[i].AssignBorder();
			}
			this.attacker.AssignBorder();
			this.victim.AssignBorder();
			base.transform.GetComponent<UIPanel>().depth = Singleton<HUDModuleManager>.Instance.GetDepth(255);
		}

		private void OnDestroy()
		{
			this.selfWidget = null;
			this.tweenAlpha.enabled = false;
			this.tweenPos.enabled = false;
			this.tweenAlpha = null;
			this.tweenPos = null;
		}

		public void SetData(SiderTipMsg _msg)
		{
			if (_msg.isTeamSign)
			{
				this.background.color = new Color32(0, 41, 103, 255);
				this.attacker.border.color = new Color32(24, 154, 25, 255);
				this.attacker.icon.spriteName = _msg.killerSpriteName;
				this.victim.border.gameObject.SetActive(false);
				this.signIcon.alpha = 1f;
				switch (_msg.signType)
				{
				case TeamSignalType.Danger:
					this.actionIcon.spriteName = "HUD_left_icons_danger";
					this.signIcon.spriteName = "HUD_text_danger";
					break;
				case TeamSignalType.Miss:
					this.actionIcon.spriteName = "HUD_left_icons_missing";
					this.signIcon.spriteName = "HUD_text_missing";
					break;
				case TeamSignalType.Converge:
					this.actionIcon.spriteName = "HUD_left_icons_help";
					this.signIcon.spriteName = "HUD_text_help";
					break;
				case TeamSignalType.Fire:
					this.actionIcon.spriteName = "HUD_left_icons_gather";
					this.signIcon.spriteName = "HUD_text_gather";
					break;
				case TeamSignalType.Defense:
					this.actionIcon.spriteName = "HUD_left_icons_protect";
					this.signIcon.spriteName = "HUD_text_protect";
					break;
				case TeamSignalType.Goto:
					this.actionIcon.spriteName = "HUD_left_icons_way";
					this.signIcon.spriteName = "HUD_text_way";
					break;
				}
				this.actionIcon.MakePixelPerfect();
				this.signIcon.MakePixelPerfect();
				for (int i = 0; i < this.assist.Count; i++)
				{
					this.assist[i].border.SetActive(false);
				}
			}
			else
			{
				this.actionIcon.spriteName = "HUD_kill_icon";
				this.actionIcon.MakePixelPerfect();
				this.signIcon.alpha = 0.002f;
				this.victim.border.gameObject.SetActive(true);
				this.attacker.icon.spriteName = _msg.killerSpriteName;
				this.victim.icon.spriteName = _msg.victimSpriteName;
				Color32 c;
				if (_msg.isAllyTip)
				{
					this.background.color = new Color32(0, 127, 0, 255);
					c = new Color32(24, 154, 25, 255);
					this.attacker.border.color = c;
					this.victim.border.color = new Color32(219, 0, 27, 255);
				}
				else
				{
					this.background.color = new Color32(129, 0, 0, 255);
					c = new Color32(219, 0, 27, 255);
					this.attacker.border.color = c;
					this.victim.border.color = new Color32(24, 154, 25, 255);
				}
				if (LevelManager.m_CurLevel.Is3V3V3())
				{
					switch (_msg.attackerTeam)
					{
					case TeamType.LM:
						c = SiderTipCtrl.Magenta;
						break;
					case TeamType.BL:
						c = SiderTipCtrl.Cyan;
						break;
					case TeamType.Team_3:
						c = SiderTipCtrl.Yellow_T3;
						break;
					}
					this.attacker.border.color = c;
					switch (_msg.victimTeam)
					{
					case TeamType.LM:
						this.victim.border.color = SiderTipCtrl.Magenta;
						break;
					case TeamType.BL:
						this.victim.border.color = SiderTipCtrl.Cyan;
						break;
					case TeamType.Team_3:
						this.victim.border.color = SiderTipCtrl.Yellow_T3;
						break;
					}
				}
				if (_msg.assistSpriteName.Count <= this.assist.Count)
				{
					for (int j = 0; j < _msg.assistSpriteName.Count; j++)
					{
						this.assist[j].border.SetActive(true);
						this.assist[j].border.color = c;
						this.assist[j].icon.spriteName = _msg.assistSpriteName[j];
					}
					for (int k = _msg.assistSpriteName.Count; k < this.assist.Count; k++)
					{
						this.assist[k].border.SetActive(false);
					}
				}
				else
				{
					ClientLogger.Error("助攻人数不正常");
					for (int l = 0; l < this.assist.Count; l++)
					{
						this.assist[l].border.SetActive(false);
					}
				}
			}
		}

		public void EaseIn(float _duration = 1f)
		{
			this.state = BattleMsgState.EaseIn;
			this.tweenPos.from = this.defaultPos + new Vector3((float)((!this.isRightSiderTip) ? 150 : -150), 0f, 0f);
			this.tweenPos.to = this.defaultPos;
			this.tweenPos.duration = _duration;
			this.tweenAlpha.from = 0.002f;
			this.tweenAlpha.to = 1f;
			this.tweenPos.duration = _duration;
			this.tweenPos.ResetToBeginning();
			this.tweenAlpha.ResetToBeginning();
			this.tweenPos.PlayForward();
			this.tweenAlpha.PlayForward();
			this.currentPos = this.defaultPos;
		}

		public void EaseOut(float _duration = 1f)
		{
			this.state = BattleMsgState.EaseOut;
			this.tweenAlpha.from = 1f;
			this.tweenAlpha.to = 0.002f;
			this.tweenAlpha.duration = _duration;
			this.tweenAlpha.ResetToBeginning();
			this.tweenAlpha.PlayForward();
		}
	}
}
