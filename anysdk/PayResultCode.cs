using System;

namespace anysdk
{
	public enum PayResultCode
	{
		kPaySuccess,
		kPayFail,
		kPayCancel,
		kPayNetworkError,
		kPayProductionInforIncomplete,
		kPayInitSuccess,
		kPayInitFail,
		kPayNowPaying,
		kPayRechargeSuccess,
		kPayExtension = 30000
	}
}
