using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.PropertyView
{
	public class RunesItem : MonoBehaviour
	{
		[SerializeField]
		private Transform level_bg;

		[SerializeField]
		private UISprite level_icon;

		[SerializeField]
		private UISprite level_num;

		[SerializeField]
		private Transform level_frame;

		[SerializeField]
		private Transform level_frame_hightlight;

		[SerializeField]
		private UISprite level_rune;

		private int rank_rune;

		private int modelID;

		private long equipID;

		private Limit_lock limit_lock;

		public Callback<GameObject> ClickCallBack;

		private Rune_Position rune_position;

		public int ModelID
		{
			get
			{
				return this.modelID;
			}
		}

		public int RankRune
		{
			get
			{
				return this.rank_rune;
			}
		}

		public Limit_lock LimitLock
		{
			get
			{
				return this.limit_lock;
			}
		}

		public Rune_Position RunePosition
		{
			get
			{
				return this.rune_position;
			}
		}

		public void Init(int rank, int idx, List<string> list_key, HeroInfoData heroinfodata)
		{
			Dictionary<string, SysHeroRunsUnlockVo> typeDicByType = BaseDataMgr.instance.GetTypeDicByType<SysHeroRunsUnlockVo>();
			this.level_frame_hightlight.gameObject.SetActive(false);
			switch (idx + 1)
			{
			case 1:
				this.modelID = heroinfodata.Ep_1;
				break;
			case 2:
				this.modelID = heroinfodata.Ep_2;
				break;
			case 3:
				this.modelID = heroinfodata.Ep_3;
				break;
			case 4:
				this.modelID = heroinfodata.Ep_4;
				break;
			case 5:
				this.modelID = heroinfodata.Ep_5;
				break;
			case 6:
				this.modelID = heroinfodata.Ep_6;
				break;
			}
			EquipmentInfoData equipmentInfoData = ModelManager.Instance.Get_equipmentList_X().Find((EquipmentInfoData obj) => obj.ModelId == this.modelID);
			this.equipID = ((equipmentInfoData != null) ? equipmentInfoData.EquipmentId : 0L);
			if (idx + 1 <= rank)
			{
				this.level_rune.gameObject.SetActive(true);
				this.level_num.gameObject.SetActive(false);
				this.level_icon.spriteName = "Hero_rune_icon_sword";
				this.level_icon.MakePixelPerfect();
				this.limit_lock = Limit_lock.unlocked;
				if (this.modelID > 0)
				{
					SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(this.modelID.ToString());
					this.level_rune.spriteName = dataById.icon;
				}
				else
				{
					this.level_rune.spriteName = string.Empty;
				}
			}
			else
			{
				this.level_rune.gameObject.SetActive(false);
				this.level_num.gameObject.SetActive(true);
				this.level_num.spriteName = "Hero_rune_level_" + typeDicByType[list_key[idx]].unlock_level;
				this.level_num.MakePixelPerfect();
				this.rank_rune = typeDicByType[list_key[idx]].unlock_level;
				this.limit_lock = Limit_lock.locked;
			}
			this.rune_position = (Rune_Position)int.Parse(list_key[idx]);
			UIEventListener.Get(base.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickRune);
		}

		private void ClickRune(GameObject obj = null)
		{
			if (this.ClickCallBack != null)
			{
				this.ClickCallBack(obj);
			}
		}

		public void ChangeState(RunesItem item)
		{
			if (item.rune_position == this.rune_position)
			{
				this.level_frame_hightlight.gameObject.SetActive(true);
			}
			else
			{
				this.level_frame_hightlight.gameObject.SetActive(false);
			}
		}

		public void ChangeRunesTexture(int modelid)
		{
			this.modelID = modelid;
			if (modelid > 0)
			{
				SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(modelid.ToString());
				this.level_rune.spriteName = dataById.icon;
			}
			else
			{
				this.level_rune.spriteName = string.Empty;
			}
		}
	}
}
