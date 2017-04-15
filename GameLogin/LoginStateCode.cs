using System;

namespace GameLogin
{
	public enum LoginStateCode
	{
		LoginState_None,
		LoginState_Init,
		LoginState_downLoadAPK,
		LoginState_waitLogin,
		LoginState_masterLogin,
		LoginState_masterGuestRegist,
		LoginState_masterRegist,
		LoginState_selectServer,
		LoginState_success
	}
}
