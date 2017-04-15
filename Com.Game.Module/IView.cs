using GUIFramework;
using System;
using UnityEngine;

namespace Com.Game.Module
{
	public interface IView
	{
		WinResurceCfg WinResCfg
		{
			get;
			set;
		}

		TUIWindow uiWindow
		{
			get;
			set;
		}

		WindowID WinId
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
		}

		GameObject AnimationRoot
		{
			get;
			set;
		}

		bool IsOpened
		{
			get;
			set;
		}

		string WindowTitle
		{
			get;
			set;
		}

		void Init();

		void Destroy();

		void HandleAfterOpenView();

		void HandleBeforeCloseView();

		void RegisterUpdateHandler();

		void CancelUpdateHandler();

		void OnRestart();

		void RefreshUI();

		void DataUpdated(object data);
	}
}
