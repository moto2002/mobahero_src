using System;
using UnityEngine;

namespace CruncherPlugin
{
	public class SkinnedCruncherTarget : CruncherTarget
	{
		public SkinnedMeshRenderer skinnedMeshRenderer;

		public override void SetMesh(Mesh mesh)
		{
			if (this.skinnedMeshRenderer != null)
			{
				this.skinnedMeshRenderer.sharedMesh = mesh;
			}
		}
	}
}
