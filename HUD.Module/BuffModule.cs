using Com.Game.Data;
using Com.Game.Module;
using GUIFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace HUD.Module
{
	public class BuffModule : BaseModule
	{
		private Transform PanelTransform;

		private Transform BuffNode;

		private Transform PassiveNode;

		private UIGrid UIPassive;

		private UIGrid UIBuff;

		private UIAnchor Anchor;

		private TweenAlpha tAlpha;

		private GameObject m_BuffItem;

		private VTrigger AddbuffTrigger;

		private VTrigger RemovebuffTrigger;

		private VTrigger ChangePlayerTrigger;

		private Units m_Player;

		private BuffListView buffView;

		private BuffListView debuffView;

		private BuffListView passiveView;

		private List<string> passive_skills = new List<string>();

		private Dictionary<string, int> buffs = new Dictionary<string, int>();

		private CoroutineManager coMgr = new CoroutineManager();

		private List<BuffItem> buffList;

		public BuffModule()
		{
			this.module = EHUDModule.Buff;
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/HUDModule/BuffModule");
		}

		public override void Init()
		{
			base.Init();
			this.PanelTransform = this.transform.Find("AnchorLeft/BuffPanel");
			this.BuffNode = this.transform.Find("AnchorLeft/BuffPanel/Buff");
			this.PassiveNode = this.transform.Find("AnchorRight/BuffPanel/Passive");
			this.tAlpha = this.transform.GetComponent<TweenAlpha>();
			this.Anchor = this.transform.FindChild("AnchorLeft").GetComponent<UIAnchor>();
			this.m_BuffItem = ResourceManager.LoadPath<GameObject>("Prefab/UI/Skill/BuffItem", null, 0);
		}

		public override void onFlyOut()
		{
			this.tAlpha.PlayForward();
		}

		public override void onFlyIn()
		{
			this.tAlpha.PlayReverse();
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

		public override void AdaptSkillPanelPivot()
		{
			this.UIBuff = this.BuffNode.GetComponent<UIGrid>();
			if (this.UIBuff != null)
			{
				switch (Singleton<HUDModuleManager>.Instance.skillPanelPivot)
				{
				case SkillPanelPivot.Bottom:
					this.Anchor.side = UIAnchor.Side.Bottom;
					this.PanelTransform.localPosition = new Vector3(-260.21f, 233.6f, 0f);
					this.UIBuff.arrangement = UIGrid.Arrangement.Horizontal;
					this.UIBuff.pivot = UIWidget.Pivot.BottomLeft;
					break;
				case SkillPanelPivot.Left:
					this.Anchor.side = UIAnchor.Side.BottomLeft;
					this.PanelTransform.localPosition = new Vector3(201.15f, 773.3f, 0f);
					this.UIBuff.arrangement = UIGrid.Arrangement.Vertical;
					this.UIBuff.pivot = UIWidget.Pivot.TopLeft;
					break;
				case SkillPanelPivot.Right:
					this.Anchor.side = UIAnchor.Side.BottomRight;
					this.PanelTransform.localPosition = new Vector3(-194.14f, 710.39f, 0f);
					this.UIBuff.arrangement = UIGrid.Arrangement.Vertical;
					this.UIBuff.pivot = UIWidget.Pivot.TopRight;
					break;
				}
				this.Anchor.enabled = true;
				this.UIBuff.Reposition();
			}
		}

		public void RefreshUI()
		{
			this.UpdateBuffView();
			this.UpdatePassiveView();
		}

		public override void Destroy()
		{
			if (this.passive_skills != null)
			{
				this.passive_skills.Clear();
			}
			if (this.buffs != null)
			{
				this.buffs.Clear();
			}
			this.m_BuffItem = null;
			if (this.passiveView != null)
			{
				this.passiveView.Clear();
				this.passiveView = null;
			}
			if (this.buffView != null)
			{
				this.buffView.Clear();
				this.buffView = null;
			}
			if (this.coMgr != null)
			{
				this.coMgr.StopAllCoroutine();
			}
			base.Destroy();
		}

		private void RegisterTrigger()
		{
			this.ChangePlayerTrigger = TriggerManager.CreateGameEventTrigger(GameEvent.ChangePlayer, null, new TriggerAction(this.ChangePlayer));
		}

		private void UnRegisterTrigger()
		{
			TriggerManager.DestroyTrigger(this.ChangePlayerTrigger);
			this.ChangePlayerTrigger = null;
			this.UniRegisterBuffTrigger();
		}

		private void RegistgerBuffTrigger(Units player)
		{
			if (player != null)
			{
				this.AddbuffTrigger = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitAddBuff, null, new TriggerAction(this.UpdateBuffView), player.unique_id);
				this.RemovebuffTrigger = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitRemoveBuff, null, new TriggerAction(this.UpdateBuffView), player.unique_id);
			}
		}

		private void UniRegisterBuffTrigger()
		{
			TriggerManager.DestroyTrigger(this.AddbuffTrigger);
			this.AddbuffTrigger = null;
			TriggerManager.DestroyTrigger(this.RemovebuffTrigger);
			this.RemovebuffTrigger = null;
		}

		private void ChangePlayer()
		{
			this.m_Player = PlayerControlMgr.Instance.GetPlayer();
			this.RegistgerBuffTrigger(this.m_Player);
			this.RefreshUI();
		}

		private void UpdateBuffView()
		{
			if (this.m_Player != null)
			{
				this.GetBuffData(this.m_Player);
				this.CreateBuffView();
			}
		}

		private void GetBuffData(Units player)
		{
			if (player != null)
			{
				this.buffs = player.buffManager.GetShowBuffGroups();
			}
		}

		private void CreateBuffView()
		{
			if (this.buffView == null)
			{
				this.buffView = new BuffListView(this.BuffNode);
				this.buffView.SetChildViewCallback(new Callback<BuffItem, int>(this.OnBuffItemChanged));
			}
			this.buffView.CreateChildView(this.buffs.Count, this.buffs.Count, this.m_BuffItem, null);
		}

		private void OnBuffItemChanged(BuffItem item, int dataIndex)
		{
			int num = 0;
			string text = null;
			int num2 = 0;
			foreach (KeyValuePair<string, int> current in this.buffs)
			{
				if (num == dataIndex)
				{
					text = current.Key;
					num2 = current.Value;
				}
				num++;
			}
			if (text == null)
			{
				return;
			}
			BuffVo buff = this.m_Player.buffManager.GetBuff(text);
			item.name = text;
			item.SetTexture(buff.data.config.buff_icon);
			item.SetType(buff.data.DataType.GainType);
			item.SetMaskActive(!buff.IsPermanent());
			if (num2 >= 1)
			{
				item.SetLayer(num2.ToString());
			}
		}

		public void UpdatePassiveView()
		{
			if (this.m_Player != null)
			{
				this.passive_skills = this.m_Player.GetPassiveSkills();
				if (this.passiveView == null)
				{
					this.passiveView = new BuffListView(this.PassiveNode);
					this.passiveView.SetChildViewCallback(new Callback<BuffItem, int>(this.OnPassiveItemChanged));
					this.coMgr.StartCoroutine(this.UpdateBuffIconMask(), true);
				}
				if (this.passive_skills == null)
				{
					return;
				}
				if (this.m_BuffItem == null)
				{
					return;
				}
				this.passiveView.CreateChildView(this.passive_skills.Count, this.passive_skills.Count, this.m_BuffItem, null);
			}
		}

		private void OnPassiveItemChanged(BuffItem item, int dataIndex)
		{
			string text = this.passive_skills[dataIndex];
			SysSkillMainVo skillData = SkillUtility.GetSkillData(text, -1, -1);
			item.name = text;
			item.SetTexture(skillData.skill_icon);
		}

		[DebuggerHidden]
		private IEnumerator UpdateBuffIconMask()
		{
			BuffModule.<UpdateBuffIconMask>c__IteratorD3 <UpdateBuffIconMask>c__IteratorD = new BuffModule.<UpdateBuffIconMask>c__IteratorD3();
			<UpdateBuffIconMask>c__IteratorD.<>f__this = this;
			return <UpdateBuffIconMask>c__IteratorD;
		}
	}
}
