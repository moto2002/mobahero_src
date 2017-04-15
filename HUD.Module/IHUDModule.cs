using GUIFramework;
using System;
using UnityEngine;

namespace HUD.Module
{
	public interface IHUDModule
	{
		WinResurceCfg WinResCfg
		{
			get;
			set;
		}

		EHUDModule module
		{
			get;
			set;
		}

		GameObject gameObject
		{
			get;
			set;
		}

		Transform transform
		{
			get;
			set;
		}

		void Init();

		void Destroy();

		void HandleAfterOpenModule();

		void HandleBeforeCloseModule();

		void RegisterUpdateHandler();

		void CancelUpdateHandler();

		void onFlyOut();

		void onFlyIn();

		void SetActive(bool isActive);
	}
}
