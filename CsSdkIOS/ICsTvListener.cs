using System;

namespace CsSdkIOS
{
	public interface ICsTvListener
	{
		void queryUserInfo();

		void startPay(CsGoodsInfo goodsInfo);

		CpAccountBalance queryAccountBalance();

		void notifyGiftResult(int code, string message);
	}
}
