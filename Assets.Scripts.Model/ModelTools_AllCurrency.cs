using System;

namespace Assets.Scripts.Model
{
	public static class ModelTools_AllCurrency
	{
		public static CurrencyData GetCurrencyDataList(this ModelManager mmng)
		{
			CurrencyData result = null;
			if (mmng != null && mmng.ValidData(EModelType.Model_AllCurrency))
			{
				result = mmng.GetData<CurrencyData>(EModelType.Model_AllCurrency);
			}
			return result;
		}

		public static long Get_CurrencyData_Diamond(this ModelManager mmng)
		{
			CurrencyData currencyData = mmng.GetCurrencyDataList();
			if (currencyData == null)
			{
				currencyData = mmng.GetData<CurrencyData>(EModelType.Model_AllCurrency);
			}
			return currencyData.Diamond;
		}

		public static long Get_CurrencyData_Gold(this ModelManager mmng)
		{
			CurrencyData currencyData = mmng.GetCurrencyDataList();
			if (currencyData == null)
			{
				currencyData = mmng.GetData<CurrencyData>(EModelType.Model_AllCurrency);
			}
			return currencyData.Gold;
		}

		public static int Get_CurrencyData_Cap(this ModelManager mmng)
		{
			CurrencyData currencyData = mmng.GetCurrencyDataList();
			if (currencyData == null)
			{
				currencyData = mmng.GetData<CurrencyData>(EModelType.Model_AllCurrency);
			}
			return currencyData.Cap;
		}

		public static long Get_CurrencyData_Trumpet(this ModelManager mmng)
		{
			CurrencyData currencyData = mmng.GetCurrencyDataList();
			if (currencyData == null)
			{
				currencyData = mmng.GetData<CurrencyData>(EModelType.Model_AllCurrency);
			}
			return (long)currencyData.Trumpet;
		}
	}
}
