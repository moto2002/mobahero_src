using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Utils;
using GUIFramework;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Game.Module
{
	public class AwardView : BaseView<AwardView>
	{
		private UILabel TitleLabel;

		private AwardViewContainer proxy;

		private List<RewardModel> rewards;

		private RewardItem sample;

		private Action onClose;

		private string titleText;

		public AwardView()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/NewUI/Public/AwardView");
		}

		public void SetAwardList(List<RewardModel> rewards, Action onClose = null, string titleName = "获得奖励")
		{
			this.rewards = rewards;
			this.onClose = onClose;
			this.titleText = titleName;
		}

		public override void Init()
		{
			base.Init();
			this.TitleLabel = this.transform.Find("Anchor/Panel/Title").GetComponent<UILabel>();
			this.proxy = this.transform.GetComponent<AwardViewContainer>();
			this.sample = this.proxy.rewardItem;
			UIEventListener.Get(this.proxy.buttonGetAward.gameObject).onClick = delegate
			{
				CtrlManager.CloseWindow(WindowID.AwardView);
				if (this.onClose != null)
				{
					this.onClose();
				}
			};
		}

		public override void HandleAfterOpenView()
		{
			base.HandleAfterOpenView();
			this.TitleLabel.text = this.titleText;
			int count = (this.rewards != null) ? this.rewards.Count : 0;
			GridHelper.FillGrid<RewardItem>(this.proxy.grid, this.sample, count, delegate(int idx, RewardItem cmp)
			{
				AwardView.BindReward(cmp, this.rewards[idx]);
			});
			this.proxy.grid.Reposition();
		}

		private static void BindReward(RewardItem cmp, RewardModel model)
		{
			cmp.gameObject.SetActive(true);
			switch (model.Type)
			{
			case 1:
				cmp.normalIcon.SetActive(true);
				if (model.Id == "1")
				{
					cmp.itemName.text = string.Empty;
					cmp.normalIcon.spriteName = "public_ui_02";
				}
				else if (model.Id == "2")
				{
					cmp.itemName.text = string.Empty;
					cmp.normalIcon.spriteName = "public_ui_03";
				}
				cmp.itemCount.text = "X" + model.Count.ToString();
				cmp.normalIcon.SetActive(true);
				cmp.specialIcon.SetActive(false);
				break;
			case 2:
				cmp.normalIcon.SetActive(true);
				cmp.itemName.text = "经验";
				cmp.normalIcon.spriteName = string.Empty;
				cmp.itemCount.text = model.Count.ToString();
				break;
			case 3:
				cmp.normalIcon.SetActive(true);
				cmp.itemName.text = "体力";
				cmp.normalIcon.spriteName = string.Empty;
				cmp.itemCount.text = model.Count.ToString();
				break;
			case 4:
			{
				cmp.normalIcon.SetActive(false);
				SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(model.Id);
				cmp.specialIcon.mainTexture = ResourceManager.Load<Texture>(dataById.icon, true, true, null, 0, false);
				cmp.itemBorder.spriteName = CharacterDataMgr.instance.GetFrame_EquipIcon(dataById.quality);
				cmp.itemName.text = dataById.name;
				cmp.itemCount.text = model.Count.ToString();
				cmp.normalIcon.SetActive(false);
				cmp.specialIcon.SetActive(true);
				break;
			}
			}
		}
	}
}
