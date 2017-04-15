using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace CruncherPlugin
{
	public class CruncherPluginManager
	{
		private class UserData
		{
			public Color[] colors;

			public BoneWeight[] boneWeights;

			public Vector4[] tangents;

			public Matrix4x4[] bindPoses;

			public Vector3[] originalVertices;
		}

		private enum ArrayIndex
		{
			Vertices,
			Triangles,
			Normals,
			Uv0s,
			Uv1s,
			LockedVertices,
			SubMeshes,
			SubMeshTriangles
		}

		private enum AdjustmentType
		{
			TargetQuality,
			TargetQuantity
		}

		private struct AdjustmentSettings
		{
			public CruncherPluginManager.AdjustmentType type;

			public float quality;

			public int quantity;

			public bool recalculateNormals;
		}

		private struct MeshConfiguration
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
		}

		private struct CruncherData
		{
			[MarshalAs(UnmanagedType.LPArray)]
			public Vector3[] vertices;

			[MarshalAs(UnmanagedType.LPArray)]
			public int[] triangles;

			[MarshalAs(UnmanagedType.LPArray)]
			public Vector3[] normals;

			[MarshalAs(UnmanagedType.LPArray)]
			public Vector2[] uv0s;

			[MarshalAs(UnmanagedType.LPArray)]
			public Vector2[] uv1s;

			[MarshalAs(UnmanagedType.LPArray)]
			public int[] subMeshTriangles;

			[MarshalAs(UnmanagedType.LPArray)]
			public int[] originalIndexes;

			[MarshalAs(UnmanagedType.LPArray)]
			public int[] lockedVertices;
		}

		private static CruncherPluginManager cruncherPluginManager;

		private static string errorOperationFailed;

		private CruncherPluginManager.AdjustmentSettings adjustmentSettings;

		private int[] arrayLengths;

		private CruncherPluginManager.CruncherData cruncherData;

		private string licenseKey;

		private CruncherPluginManager.MeshConfiguration meshConfiguration;

		private CruncherPluginManager.UserData userData;

		static CruncherPluginManager()
		{
			CruncherPluginManager.errorOperationFailed = "Plugin operation failed, aborting. Check the plugin log in the project's root directory for more information.";
			string environmentVariable = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process);
			string text = string.Concat(new object[]
			{
				Environment.CurrentDirectory,
				Path.DirectorySeparatorChar,
				"Assets",
				Path.DirectorySeparatorChar,
				"Plugins"
			});
			if (!environmentVariable.Contains(text))
			{
				Environment.SetEnvironmentVariable("PATH", text + Path.PathSeparator + environmentVariable, EnvironmentVariableTarget.Process);
			}
		}

		[DllImport("CruncherPlugin")]
		private static extern string PluginExpirationDate();

		[DllImport("CruncherPlugin")]
		private static extern bool PluginStartup(string licenseKey);

		[DllImport("CruncherPlugin")]
		private static extern void PluginShutdown();

		[DllImport("CruncherPlugin")]
		private static extern int PluginAddMesh([MarshalAs(UnmanagedType.LPStruct)] [In] [Out] CruncherPluginManager.CruncherData cruncherData, int[] arrayLengths, ref CruncherPluginManager.MeshConfiguration meshConfiguration);

		[DllImport("CruncherPlugin")]
		private static extern bool PluginAdjustMeshes(ref CruncherPluginManager.AdjustmentSettings adjustmentSettings);

		[DllImport("CruncherPlugin")]
		private static extern bool PluginQueryMeshSizes(int meshId, int[] arrayLengths);

		[DllImport("CruncherPlugin")]
		private static extern bool PluginRetrieveMesh(int meshId, [MarshalAs(UnmanagedType.LPStruct)] [In] [Out] CruncherPluginManager.CruncherData cruncherData, int[] arrayLengths, bool recalculateNormals);

		public static bool Startup(string licenseKey = "")
		{
			if (CruncherPluginManager.cruncherPluginManager != null)
			{
				CruncherPluginManager.Shutdown();
			}
			CruncherPluginManager.cruncherPluginManager = new CruncherPluginManager();
			if (CruncherPluginManager.cruncherPluginManager == null)
			{
				Debug.LogError("Failed to instantiate CruncherPluginManager, aborting.");
				return false;
			}
			CruncherPluginManager.cruncherPluginManager.adjustmentSettings = default(CruncherPluginManager.AdjustmentSettings);
			CruncherPluginManager.cruncherPluginManager.cruncherData = default(CruncherPluginManager.CruncherData);
			CruncherPluginManager.cruncherPluginManager.licenseKey = licenseKey;
			CruncherPluginManager.cruncherPluginManager.meshConfiguration = default(CruncherPluginManager.MeshConfiguration);
			if (CruncherPluginManager.PluginStartup(CruncherPluginManager.cruncherPluginManager.licenseKey))
			{
				return true;
			}
			CruncherPluginManager.Shutdown();
			Debug.LogError("Failed to start plugin, aborting.");
			return false;
		}

		public static void Shutdown()
		{
			CruncherPluginManager.PluginShutdown();
			if (CruncherPluginManager.cruncherPluginManager != null)
			{
				CruncherPluginManager.cruncherPluginManager.arrayLengths = null;
				CruncherPluginManager.cruncherPluginManager.licenseKey = null;
				if (CruncherPluginManager.cruncherPluginManager.userData != null)
				{
					CruncherPluginManager.cruncherPluginManager.userData.colors = null;
					CruncherPluginManager.cruncherPluginManager.userData.boneWeights = null;
					CruncherPluginManager.cruncherPluginManager.userData.tangents = null;
					CruncherPluginManager.cruncherPluginManager.userData.bindPoses = null;
				}
				CruncherPluginManager.cruncherPluginManager.userData = null;
				CruncherPluginManager.cruncherPluginManager = null;
			}
		}

		public static int AddMesh(CruncherIO cruncherIO, CruncherMeshConfiguration cruncherMeshConfiguration)
		{
			if (CruncherPluginManager.cruncherPluginManager == null && !CruncherPluginManager.Startup(""))
			{
				return -1;
			}
			if (cruncherIO == null || cruncherIO.vertices == null || cruncherIO.triangles == null)
			{
				Debug.LogError("Invalid CruncherIO object, aborting AddMesh.");
				return -1;
			}
			CruncherPluginManager.cruncherPluginManager.cruncherData.vertices = cruncherIO.vertices;
			CruncherPluginManager.cruncherPluginManager.cruncherData.triangles = cruncherIO.triangles;
			CruncherPluginManager.cruncherPluginManager.cruncherData.normals = cruncherIO.normals;
			CruncherPluginManager.cruncherPluginManager.cruncherData.uv0s = cruncherIO.uv0s;
			CruncherPluginManager.cruncherPluginManager.cruncherData.uv1s = cruncherIO.uv1s;
			int num = cruncherIO.subMeshTriangles.Length;
			CruncherPluginManager.cruncherPluginManager.arrayLengths = new int[7 + num];
			CruncherPluginManager.cruncherPluginManager.arrayLengths[6] = num;
			CruncherPluginManager.cruncherPluginManager.arrayLengths[0] = CruncherPluginManager.cruncherPluginManager.cruncherData.vertices.Length;
			CruncherPluginManager.cruncherPluginManager.arrayLengths[1] = CruncherPluginManager.cruncherPluginManager.cruncherData.triangles.Length;
			if (CruncherPluginManager.cruncherPluginManager.cruncherData.normals != null)
			{
				CruncherPluginManager.cruncherPluginManager.arrayLengths[2] = CruncherPluginManager.cruncherPluginManager.cruncherData.normals.Length;
			}
			if (CruncherPluginManager.cruncherPluginManager.cruncherData.uv0s != null)
			{
				CruncherPluginManager.cruncherPluginManager.arrayLengths[3] = CruncherPluginManager.cruncherPluginManager.cruncherData.uv0s.Length;
			}
			if (CruncherPluginManager.cruncherPluginManager.cruncherData.uv1s != null)
			{
				CruncherPluginManager.cruncherPluginManager.arrayLengths[4] = CruncherPluginManager.cruncherPluginManager.cruncherData.uv1s.Length;
			}
			CruncherPluginManager.cruncherPluginManager.cruncherData.lockedVertices = cruncherIO.lockedVertices;
			if (CruncherPluginManager.cruncherPluginManager.cruncherData.lockedVertices != null)
			{
				CruncherPluginManager.cruncherPluginManager.arrayLengths[5] = CruncherPluginManager.cruncherPluginManager.cruncherData.lockedVertices.Length;
			}
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				num2 += cruncherIO.subMeshTriangles[i].Length;
			}
			CruncherPluginManager.cruncherPluginManager.cruncherData.subMeshTriangles = new int[num2];
			int num3 = 0;
			for (int j = 0; j < num; j++)
			{
				CruncherPluginManager.cruncherPluginManager.arrayLengths[7 + j] = cruncherIO.subMeshTriangles[j].Length;
				Array.Copy(cruncherIO.subMeshTriangles[j], 0, CruncherPluginManager.cruncherPluginManager.cruncherData.subMeshTriangles, num3, cruncherIO.subMeshTriangles[j].Length);
				num3 += cruncherIO.subMeshTriangles[j].Length;
			}
			if (cruncherMeshConfiguration != null)
			{
				CruncherPluginManager.cruncherPluginManager.meshConfiguration.joinVertices = cruncherMeshConfiguration.joinVertices;
				CruncherPluginManager.cruncherPluginManager.meshConfiguration.joinVerticesThreshold = cruncherMeshConfiguration.joinVerticesThreshold;
				CruncherPluginManager.cruncherPluginManager.meshConfiguration.joinNormals = cruncherMeshConfiguration.joinNormals;
				CruncherPluginManager.cruncherPluginManager.meshConfiguration.joinNormalsThreshold = cruncherMeshConfiguration.joinNormalsThreshold;
				CruncherPluginManager.cruncherPluginManager.meshConfiguration.joinUvs = cruncherMeshConfiguration.joinUvs;
				CruncherPluginManager.cruncherPluginManager.meshConfiguration.joinUvsThreshold = cruncherMeshConfiguration.joinUvsThreshold;
				CruncherPluginManager.cruncherPluginManager.meshConfiguration.removeTJunctions = cruncherMeshConfiguration.removeTJunctions;
				CruncherPluginManager.cruncherPluginManager.meshConfiguration.removeTJunctionsThreshold = cruncherMeshConfiguration.removeTJunctionsThreshold;
				CruncherPluginManager.cruncherPluginManager.meshConfiguration.recalculateNormals = cruncherMeshConfiguration.recalculateNormals;
			}
			else
			{
				Debug.Log("cruncherMeshConfiguration == null, you must pass in a valid CruncherMeshConfiguration in order to process meshes.");
			}
			int num4 = CruncherPluginManager.PluginAddMesh(CruncherPluginManager.cruncherPluginManager.cruncherData, CruncherPluginManager.cruncherPluginManager.arrayLengths, ref CruncherPluginManager.cruncherPluginManager.meshConfiguration);
			CruncherPluginManager.cruncherPluginManager.cruncherData = default(CruncherPluginManager.CruncherData);
			if (num4 < 0)
			{
				Debug.LogError(CruncherPluginManager.errorOperationFailed);
				return -1;
			}
			CruncherPluginManager.UserData userData = new CruncherPluginManager.UserData();
			if (cruncherIO.colors != null && cruncherIO.colors.Length > 0)
			{
				userData.colors = new Color[cruncherIO.colors.Length];
				Array.Copy(cruncherIO.colors, userData.colors, cruncherIO.colors.Length);
			}
			if (cruncherIO.boneWeights != null && cruncherIO.boneWeights.Length > 0)
			{
				userData.boneWeights = new BoneWeight[cruncherIO.boneWeights.Length];
				Array.Copy(cruncherIO.boneWeights, userData.boneWeights, cruncherIO.boneWeights.Length);
			}
			if (cruncherIO.tangents != null && cruncherIO.tangents.Length > 0)
			{
				userData.tangents = new Vector4[cruncherIO.tangents.Length];
				Array.Copy(cruncherIO.tangents, userData.tangents, cruncherIO.tangents.Length);
			}
			userData.originalVertices = new Vector3[cruncherIO.vertices.Length];
			Array.Copy(cruncherIO.vertices, userData.originalVertices, cruncherIO.vertices.Length);
			userData.bindPoses = cruncherIO.bindPoses;
			CruncherPluginManager.cruncherPluginManager.userData = userData;
			return num4;
		}

		public static bool AdjustMeshes(CruncherAdjustment cruncherAdjustment)
		{
			CruncherPluginManager.cruncherPluginManager.adjustmentSettings.type = (CruncherPluginManager.AdjustmentType)cruncherAdjustment.type;
			CruncherPluginManager.cruncherPluginManager.adjustmentSettings.quality = (float)Math.Round((double)cruncherAdjustment.quality, 4);
			CruncherPluginManager.cruncherPluginManager.adjustmentSettings.quantity = cruncherAdjustment.quantity;
			if (!CruncherPluginManager.PluginAdjustMeshes(ref CruncherPluginManager.cruncherPluginManager.adjustmentSettings))
			{
				Debug.LogWarning(CruncherPluginManager.errorOperationFailed);
				return false;
			}
			cruncherAdjustment.type = (CruncherAdjustment.Type)CruncherPluginManager.cruncherPluginManager.adjustmentSettings.type;
			cruncherAdjustment.quality = (float)Math.Round((double)CruncherPluginManager.cruncherPluginManager.adjustmentSettings.quality, 4);
			cruncherAdjustment.quantity = CruncherPluginManager.cruncherPluginManager.adjustmentSettings.quantity;
			return true;
		}

		public static void Reset()
		{
			CruncherPluginManager.Startup("");
		}

		public static CruncherIO RetrieveMesh(int meshId, bool recalculateNormals, bool retrieveOriginalIndexes = false)
		{
			if (!CruncherPluginManager.PluginQueryMeshSizes(meshId, CruncherPluginManager.cruncherPluginManager.arrayLengths))
			{
				Debug.LogError(CruncherPluginManager.errorOperationFailed);
				return null;
			}
			int num = CruncherPluginManager.cruncherPluginManager.arrayLengths[0];
			CruncherPluginManager.cruncherPluginManager.cruncherData.vertices = new Vector3[num];
			CruncherPluginManager.cruncherPluginManager.cruncherData.triangles = new int[CruncherPluginManager.cruncherPluginManager.arrayLengths[1]];
			if (CruncherPluginManager.cruncherPluginManager.arrayLengths[2] == num)
			{
				CruncherPluginManager.cruncherPluginManager.cruncherData.normals = new Vector3[num];
			}
			else
			{
				Debug.Log("PluginQueryMeshSizes says normal array length is " + CruncherPluginManager.cruncherPluginManager.arrayLengths[2]);
			}
			if (CruncherPluginManager.cruncherPluginManager.arrayLengths[3] == num)
			{
				CruncherPluginManager.cruncherPluginManager.cruncherData.uv0s = new Vector2[num];
			}
			if (CruncherPluginManager.cruncherPluginManager.arrayLengths[4] == num)
			{
				CruncherPluginManager.cruncherPluginManager.cruncherData.uv1s = new Vector2[num];
			}
			int num2 = 0;
			for (int i = 0; i < CruncherPluginManager.cruncherPluginManager.arrayLengths[6]; i++)
			{
				num2 += CruncherPluginManager.cruncherPluginManager.arrayLengths[7 + i];
			}
			CruncherPluginManager.cruncherPluginManager.cruncherData.subMeshTriangles = new int[num2];
			CruncherPluginManager.UserData userData = CruncherPluginManager.cruncherPluginManager.userData;
			if (retrieveOriginalIndexes || userData.colors != null || userData.boneWeights != null || userData.tangents != null)
			{
				CruncherPluginManager.cruncherPluginManager.cruncherData.originalIndexes = new int[num];
			}
			if (!CruncherPluginManager.PluginRetrieveMesh(meshId, CruncherPluginManager.cruncherPluginManager.cruncherData, CruncherPluginManager.cruncherPluginManager.arrayLengths, recalculateNormals))
			{
				Debug.LogError(CruncherPluginManager.errorOperationFailed);
				CruncherPluginManager.cruncherPluginManager.cruncherData = default(CruncherPluginManager.CruncherData);
				return null;
			}
			CruncherIO cruncherIO = new CruncherIO();
			cruncherIO.vertices = CruncherPluginManager.cruncherPluginManager.cruncherData.vertices;
			cruncherIO.triangles = CruncherPluginManager.cruncherPluginManager.cruncherData.triangles;
			cruncherIO.normals = CruncherPluginManager.cruncherPluginManager.cruncherData.normals;
			cruncherIO.uv0s = CruncherPluginManager.cruncherPluginManager.cruncherData.uv0s;
			cruncherIO.uv1s = CruncherPluginManager.cruncherPluginManager.cruncherData.uv1s;
			cruncherIO.subMeshTriangles = new int[CruncherPluginManager.cruncherPluginManager.arrayLengths[6]][];
			int num3 = 0;
			for (int j = 0; j < cruncherIO.subMeshTriangles.Length; j++)
			{
				cruncherIO.subMeshTriangles[j] = new int[CruncherPluginManager.cruncherPluginManager.arrayLengths[7 + j]];
				Array.Copy(CruncherPluginManager.cruncherPluginManager.cruncherData.subMeshTriangles, num3, cruncherIO.subMeshTriangles[j], 0, cruncherIO.subMeshTriangles[j].Length);
				num3 += cruncherIO.subMeshTriangles[j].Length;
			}
			cruncherIO.bindPoses = userData.bindPoses;
			cruncherIO.originalIndexes = CruncherPluginManager.cruncherPluginManager.cruncherData.originalIndexes;
			if (cruncherIO.originalIndexes != null && cruncherIO.originalIndexes.Length == cruncherIO.vertices.Length && (userData.colors != null || userData.boneWeights != null || userData.tangents != null))
			{
				if (userData.colors != null && userData.colors.Length > 0)
				{
					cruncherIO.colors = new Color[num];
				}
				if (userData.boneWeights != null && userData.boneWeights.Length > 0)
				{
					cruncherIO.boneWeights = new BoneWeight[num];
				}
				if (userData.tangents != null && userData.tangents.Length > 0)
				{
					cruncherIO.tangents = new Vector4[num];
				}
				for (int k = 0; k < num; k++)
				{
					if (userData.colors != null && userData.colors.Length > 0)
					{
						cruncherIO.colors[k] = userData.colors[cruncherIO.originalIndexes[k]];
					}
					if (userData.boneWeights != null && userData.boneWeights.Length > 0)
					{
						cruncherIO.boneWeights[k] = userData.boneWeights[cruncherIO.originalIndexes[k]];
					}
					if (userData.tangents != null && userData.tangents.Length > 0)
					{
						cruncherIO.tangents[k] = userData.tangents[cruncherIO.originalIndexes[k]];
					}
				}
			}
			if (!retrieveOriginalIndexes)
			{
				cruncherIO.originalIndexes = null;
			}
			CruncherPluginManager.cruncherPluginManager.cruncherData = default(CruncherPluginManager.CruncherData);
			return cruncherIO;
		}
	}
}
