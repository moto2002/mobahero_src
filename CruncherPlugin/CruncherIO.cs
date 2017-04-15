using System;
using UnityEngine;

namespace CruncherPlugin
{
	[Serializable]
	public class CruncherIO
	{
		public Vector3[] vertices;

		public int[] triangles;

		public Vector3[] normals;

		public Vector2[] uv0s;

		public Vector2[] uv1s;

		public Color[] colors;

		public BoneWeight[] boneWeights;

		public Vector4[] tangents;

		public Matrix4x4[] bindPoses;

		public int[][] subMeshTriangles;

		public int[] originalIndexes;

		public int[] lockedVertices;
	}
}
