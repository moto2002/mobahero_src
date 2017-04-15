using System;
using UnityEngine;

namespace Com.Game.Module
{
	public interface ILoginNewsItem
	{
		DownLoadAPKNewsItem Info
		{
			get;
			set;
		}

		GameObject Obj
		{
			get;
		}

		Action<GameObject, bool> OnHandle
		{
			get;
			set;
		}

		void CheckResource();
	}
}
