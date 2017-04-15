using System;

namespace CsSdk
{
	public interface ICsTvListener
	{
		GameUserInfo getUserInfo();

		void startPay(CsGoodsInfo goodsInfo);

		CpAccountBalance queryAccountBalance();

		void notifyGiftResult(int code, string message, int accountBalance);
	}
}
