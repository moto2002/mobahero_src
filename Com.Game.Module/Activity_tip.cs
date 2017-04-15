using System;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Game.Module
{
	public class Activity_tip : MonoBehaviour
	{
		public ActivityTip_common[] tipComs;

		public UILabel lb_typeName;

		public UILabel[] lb_name;

		public UISprite sp_left;

		public UISprite sp_right;

		private RewardItemBase info;

		private Dictionary<ERewardType, ActivityTip_common> dicComs;

		private ActivityTip_common curCom;

		public RewardItemBase Info
		{
			get
			{
				return this.info;
			}
			set
			{
				if (this.info != value)
				{
					this.info = value;
					this.RefreshUI();
					this.RefrehUI_card();
				}
			}
		}

		public Vector3 Pos
		{
			set
			{
				base.transform.localPosition = value;
			}
		}

		public bool Show
		{
			set
			{
				if (base.gameObject.activeSelf != value)
				{
					base.gameObject.SetActive(value);
				}
			}
		}

		private void Awake()
		{
			this.dicComs = new Dictionary<ERewardType, ActivityTip_common>();
			ERewardType[] array = (ERewardType[])Enum.GetValues(typeof(ERewardType));
			for (int i = 0; i < array.Length; i++)
			{
				for (int j = 0; j < this.tipComs.Length; j++)
				{
					if (this.tipComs[j].IsThisCom(array[i]))
					{
						this.dicComs.Add(array[i], this.tipComs[j]);
						break;
					}
				}
			}
		}

		private void Start()
		{
		}

		private void RefrehUI_card()
		{
			if (this.dicComs.ContainsKey(this.Info.RewardType))
			{
				if (null != this.curCom)
				{
					this.curCom.gameObject.SetActive(false);
				}
				this.curCom = this.dicComs[this.Info.RewardType];
				this.curCom.Info = this.Info;
				this.curCom.RefrehUI();
				this.curCom.gameObject.SetActive(true);
			}
		}

		private void RefreshUI()
		{
			this.lb_typeName.text = this.Info.TypeDes;
			int num = this.Info.Quality - 1;
			if (num >= 0 && num < this.lb_name.Length)
			{
				this.lb_name[num].text = this.Info.Des;
				for (int i = 0; i < this.lb_name.Length; i++)
				{
					this.lb_name[i].gameObject.SetActive(i == num);
				}
			}
		}
	}
}
