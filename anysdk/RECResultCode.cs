using System;

namespace anysdk
{
	public enum RECResultCode
	{
		kRECInitSuccess,
		kRECInitFail,
		kRECStartRecording,
		kRECStopRecording,
		kRECPauseRecording,
		kRECResumeRecording,
		kRECEnterSDKPage,
		kRECQuitSDKPage,
		kRECShareSuccess,
		kRECShareFail,
		kRECExtension = 90000
	}
}
