using System;

namespace anysdk
{
	public enum AdsResultCode
	{
		kAdsReceived,
		kAdsShown,
		kAdsDismissed,
		kPointsSpendSucceed,
		kPointsSpendFailed,
		kNetworkError,
		kUnknownError,
		kOfferWallOnPointsChanged,
		kRewardedVideoWithReward,
		kInAppPurchaseFinished,
		kAdsClicked,
		kAdsExtension = 40000
	}
}
