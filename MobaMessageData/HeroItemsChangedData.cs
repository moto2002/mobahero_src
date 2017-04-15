using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace MobaMessageData
{
	public class HeroItemsChangedData
	{
		public int _uid;

		public List<ItemDynData> _list;

		public HeroItemsChangedData(int uid, List<ItemDynData> list)
		{
			this._uid = uid;
			this._list = list;
		}
	}
}
