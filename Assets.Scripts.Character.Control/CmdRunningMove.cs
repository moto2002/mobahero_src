using System;
using UnityEngine;

namespace Assets.Scripts.Character.Control
{
	public class CmdRunningMove : CmdRunningBase
	{
		public bool isMoving;

		public Vector3 targetPos;

		public void SetMoving(Vector3 inTargetPos)
		{
			this.targetPos = inTargetPos;
			this.isMoving = true;
			this.isInterrupted = false;
		}

		public override void Finish(bool inIsInterrupted = false)
		{
			base.Finish(inIsInterrupted);
			this.isMoving = false;
		}
	}
}
