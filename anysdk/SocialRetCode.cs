using System;

namespace anysdk
{
	public enum SocialRetCode
	{
		kScoreSubmitSucceed = 1,
		kScoreSubmitfail,
		kAchUnlockSucceed,
		kAchUnlockFail,
		kSocialSignInSucceed,
		kSocialSignInFail,
		kSocialSignOutSucceed,
		kSocialSignOutFail,
		kSocialGetGameFriends,
		kSocialExtensionCode = 20000
	}
}
