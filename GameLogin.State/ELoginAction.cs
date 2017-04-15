using System;

namespace GameLogin.State
{
	public enum ELoginAction
	{
		eNull,
		ePlayLogoMovieStart,
		ePlayLogoMovieFinish,
		eVedio1Ready,
		eVedio1Finish,
		eVedio2Start,
		eConnectMaster,
		eCheckVersion,
		eShowLoadView,
		eCheckDownload,
		eDownLoadBindataFinish,
		eDownLoadResourceFinish,
		eBeginLoad,
		eInitData
	}
}
