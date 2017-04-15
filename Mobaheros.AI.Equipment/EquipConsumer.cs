using Assets.Scripts.Model;
using System;
using System.Collections.Generic;

namespace Mobaheros.AI.Equipment
{
	public class EquipConsumer
	{
		public List<ItemInfo> EquipmentInfos
		{
			get;
			private set;
		}

		public int TotalCost
		{
			get;
			private set;
		}

		public bool TreeFinished
		{
			get;
			private set;
		}

		public EquipConsumer(List<ItemInfo> infos, bool finish)
		{
			this.EquipmentInfos = new List<ItemInfo>(infos);
			this.TreeFinished = finish;
		}
	}
}
