using System;

namespace Assets.Scripts.Character.Control
{
	public class CmdRunningBase
	{
		public bool isInterrupted;

		public virtual void Finish(bool inIsInterrupted = false)
		{
			this.isInterrupted = inIsInterrupted;
		}
	}
}
