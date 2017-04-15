using System;
using UnityEngine;

namespace CruncherPlugin
{
	public class SimpleCruncherTarget : CruncherTarget
	{
		public MeshFilter meshFilter;

		public override void SetMesh(Mesh mesh)
		{
			if (this.meshFilter != null)
			{
				this.meshFilter.sharedMesh = mesh;
			}
		}
	}
}
