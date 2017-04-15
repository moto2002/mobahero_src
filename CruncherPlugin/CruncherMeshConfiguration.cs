using System;

namespace CruncherPlugin
{
	[Serializable]
	public class CruncherMeshConfiguration
	{
		public bool removeTJunctions;

		public float removeTJunctionsThreshold;

		public bool joinVertices;

		public float joinVerticesThreshold;

		public bool joinUvs;

		public float joinUvsThreshold;

		public bool joinNormals;

		public float joinNormalsThreshold;

		public bool recalculateNormals;

		public static void Copy(CruncherMeshConfiguration from, CruncherMeshConfiguration to)
		{
			to.removeTJunctions = from.removeTJunctions;
			to.removeTJunctionsThreshold = from.removeTJunctionsThreshold;
			to.joinVertices = from.joinVertices;
			to.joinVerticesThreshold = from.joinVerticesThreshold;
			to.joinUvs = from.joinUvs;
			to.joinUvsThreshold = from.joinUvsThreshold;
			to.joinNormals = from.joinNormals;
			to.joinNormalsThreshold = from.joinNormalsThreshold;
			to.recalculateNormals = from.recalculateNormals;
		}
	}
}
