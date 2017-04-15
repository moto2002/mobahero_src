using System;

namespace Com.Game.Manager.Asset
{
	public enum DownLoadState
	{
		Init,
		Loading,
		Loaded,
		Stored,
		LoadFailure,
		StoreFailure,
		Cached
	}
}
