using System;

namespace Assets.Scripts.Server
{
	internal interface IGlobalComServer
	{
		void OnAwake();

		void OnStart();

		void OnUpdate();

		void OnDestroy();

		void Enable(bool b);

		void OnRestart();

		void OnApplicationQuit();

		void OnApplicationFocus(bool isFocus);

		void OnApplicationPause(bool isPause);
	}
}
