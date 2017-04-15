using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	public struct TaskAward
	{
		public int taskId;

		public List<EquipmentInfoData> equipmentList;

		public List<HeroInfoData> heroList;

		public List<DropItemData> itemList;

		public List<DropItemData> repeatList;
	}
}
