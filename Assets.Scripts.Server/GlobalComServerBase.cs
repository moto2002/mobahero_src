using System;

namespace Assets.Scripts.Server
{
	public class GlobalComServerBase : IGlobalComServer
	{
		public virtual void OnAwake()
		{
		}

		public virtual void OnStart()
		{
		}

		public virtual void OnUpdate()
		{
		}

		public virtual void OnDestroy()
		{
		}

		public virtual void Enable(bool b)
		{
		}

		public virtual void OnRestart()
		{
		}

		public virtual void OnApplicationQuit()
		{
		}

		public virtual void OnApplicationFocus(bool isFocus)
		{
		}

		public void OnApplicationPause(bool isPause)
		{
		}
	}
}
