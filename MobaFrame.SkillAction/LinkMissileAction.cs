using System;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class LinkMissileAction : MissileAction
	{
		private LineRenderer[] mLineRenders;

		protected override void OnInit()
		{
			base.OnInit();
			this.mLineRenders = base.gameObject.GetComponentsInChildren<LineRenderer>();
		}

		public void LinkTargets(LineRenderer lineRenderer, Vector3 startPos, Vector3 endPos)
		{
			lineRenderer.SetVertexCount(2);
			lineRenderer.SetPosition(0, startPos);
			lineRenderer.SetPosition(1, endPos);
		}
	}
}
