using Com.Game.Data;
using System;

namespace Assets.Scripts.Model
{
	public interface IBEItem
	{
		string ID
		{
			get;
		}

		SysBattleItemsVo Config
		{
			get;
		}

		ColumnType Level
		{
			get;
		}

		BattleEquipType Type
		{
			get;
		}

		int RealPrice
		{
			get;
		}

		int Price
		{
			get;
		}
	}
}
