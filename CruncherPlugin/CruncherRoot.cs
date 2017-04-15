using System;
using UnityEngine;

namespace CruncherPlugin
{
	[Serializable]
	public class CruncherRoot : MonoBehaviour
	{
		public CruncherTarget[] cruncherTargets;

		public CruncherMeshConfiguration cmc;

		private CruncherRoot()
		{
			if (this.cmc == null)
			{
				this.cmc = new CruncherMeshConfiguration();
				this.cmc.removeTJunctions = true;
				this.cmc.removeTJunctionsThreshold = 0.1f;
				this.cmc.joinVertices = true;
				this.cmc.joinVerticesThreshold = 0.0001f;
				this.cmc.joinUvs = false;
				this.cmc.joinUvsThreshold = 0.0001f;
				this.cmc.joinNormals = false;
				this.cmc.joinNormalsThreshold = 60f;
				this.cmc.recalculateNormals = false;
			}
		}
	}
}
