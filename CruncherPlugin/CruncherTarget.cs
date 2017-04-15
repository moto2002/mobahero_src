using System;
using UnityEngine;

namespace CruncherPlugin
{
	public abstract class CruncherTarget : MonoBehaviour
	{
		[Serializable]
		public class InputMesh
		{
			public Mesh mesh;

			public Material[] materials;
		}

		public CruncherTarget.InputMesh[] inputMeshes;

		public Mesh outputMesh;

		public Renderer outputRenderer;

		public float quality = 0.5f;

		public int quantity;

		public int inputVerticeCount;

		public int inputTriangleIndexCount;

		public int outputVerticeCount;

		public int outputTriangleIndexCount;

		public abstract void SetMesh(Mesh mesh);
	}
}
