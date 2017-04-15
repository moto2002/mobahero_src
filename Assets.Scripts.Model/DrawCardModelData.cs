using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	public class DrawCardModelData
	{
		public List<DropItemData> normalDrop;

		public List<DropItemData> repeatDrop;

		public List<EquipmentInfoData> equipList;

		public List<HeroInfoData> heroinfoList;

		public DrawCardModelData()
		{
			this.normalDrop = new List<DropItemData>();
			this.repeatDrop = new List<DropItemData>();
			this.equipList = new List<EquipmentInfoData>();
			this.heroinfoList = new List<HeroInfoData>();
		}
	}
}
