using System;

namespace anysdk
{
	public enum UserActionResultCode
	{
		kInitSuccess,
		kInitFail,
		kLoginSuccess,
		kLoginNetworkError,
		kLoginNoNeed,
		kLoginFail,
		kLoginCancel,
		kLogoutSuccess,
		kLogoutFail,
		kPlatformEnter,
		kPlatformBack,
		kPausePage,
		kExitPage,
		kAntiAddictionQuery,
		kRealNameRegister,
		kAccountSwitchSuccess,
		kAccountSwitchFail,
		kOpenShop,
		kAccountSwitchCancel,
		kGameExitPage,
		kUserExtension = 50000
	}
}
