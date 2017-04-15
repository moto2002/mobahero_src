using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	public interface IBEquip_data
	{
		List<RItemData> GenerateChilren(int level);
	}
}
