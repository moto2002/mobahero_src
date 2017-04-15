using Assets.Scripts.Model;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace Com.Game.Module
{
	public class Sub_DropItemBase
	{
		public UserData UserDATA
		{
			get
			{
				return ModelManager.Instance.Get_userData_X();
			}
		}

		public List<DropItemData> RepeatList
		{
			get
			{
				return ModelManager.Instance.Get_RepeatData_X();
			}
		}

		public DropItemID DropID
		{
			get;
			set;
		}

		public int ItemID
		{
			get;
			set;
		}

		public int ItemCount
		{
			get;
			set;
		}

		public Sub_DropItemBase(DropItemID ID)
		{
			this.DropID = ID;
		}

		public virtual void Init(DropItemData data)
		{
			if (data == null)
			{
				return;
			}
			this.ItemID = data.itemId;
			this.ItemCount = data.itemCount;
		}

		public virtual void SetData()
		{
		}
	}
}
