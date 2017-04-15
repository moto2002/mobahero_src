using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	public static class ModelTools_exchange
	{
		public static List<ExchangeData> Get_exchangeList_X(this ModelManager mmng)
		{
			return mmng.GetExchangeData();
		}

		private static List<ExchangeData> GetExchangeData(this ModelManager mmng)
		{
			List<ExchangeData> result = null;
			if (mmng != null && mmng.ValidData(EModelType.Model_exchange))
			{
				result = mmng.GetData<List<ExchangeData>>(EModelType.Model_exchange);
			}
			return result;
		}
	}
}
