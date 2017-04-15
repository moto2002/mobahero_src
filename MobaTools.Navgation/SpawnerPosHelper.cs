using System;
using System.Collections.Generic;
using UnityEngine;

namespace MobaTools.Navgation
{
	[ExecuteInEditMode]
	public class SpawnerPosHelper : MonoBehaviour
	{
		public const string LM = "LM";

		public const string BL = "BL";

		public const string NE = "NE";

		public const string Team3 = "Team3";

		[SerializeField]
		private List<Transform> BLSpawnPoints = new List<Transform>();

		[SerializeField]
		private List<Transform> LMSpawnPoints = new List<Transform>();

		[SerializeField]
		private List<Transform> NESpawnPoints = new List<Transform>();

		[SerializeField]
		private List<Transform> Team3SpawnPoints = new List<Transform>();

		public List<Transform> CameraSpawnPoints = new List<Transform>();

		[SerializeField]
		private bool isShowPosition;

		private bool showEnabledOnly = true;

		private void OnEnable()
		{
			this.FindPoints();
		}

		private void Start()
		{
			if (Application.isPlaying)
			{
				this.isShowPosition = false;
				this.DisableHelperCameras();
			}
		}

		private void FindPoints()
		{
			this.CollectPoints("Camera", this.CameraSpawnPoints);
			this.CollectPoints("LM", this.LMSpawnPoints);
			this.CollectPoints("BL", this.BLSpawnPoints);
			this.CollectPoints("NE", this.NESpawnPoints);
			this.CollectPoints("Team3", this.Team3SpawnPoints);
		}

		private void CollectPoints(string node, List<Transform> coll)
		{
			coll.Clear();
			Transform transform = base.transform.FindChild(node);
			if (!transform)
			{
				return;
			}
			for (int i = 0; i < transform.childCount; i++)
			{
				if (transform.GetChild(i).childCount == 0)
				{
					Transform child = transform.GetChild(i);
					coll.Add(child);
				}
				else
				{
					for (int j = 0; j < transform.GetChild(i).childCount; j++)
					{
						Transform child = transform.GetChild(i).GetChild(j);
						coll.Add(child);
					}
				}
			}
		}

		private void ShowPosition()
		{
			this.DrawPoints(this.LMSpawnPoints, Color.green);
			this.DrawPoints(this.BLSpawnPoints, Color.red);
			this.DrawPoints(this.Team3SpawnPoints, Color.blue);
			this.DrawPoints(this.NESpawnPoints, Color.yellow);
		}

		private void DrawSpawnPoint(Transform spawnPt)
		{
			if (this.showEnabledOnly && !spawnPt.gameObject.activeInHierarchy)
			{
				return;
			}
			Gizmos.DrawSphere(spawnPt.position, 0.2f);
			Gizmos.DrawRay(spawnPt.position, spawnPt.forward);
		}

		private void DrawPoints(IList<Transform> transList, Color color)
		{
			Gizmos.color = color;
			for (int i = 0; i < transList.Count; i++)
			{
				this.DrawSpawnPoint(transList[i]);
			}
		}

		private void OnDrawGizmos()
		{
			if (this.isShowPosition)
			{
				this.ShowPosition();
			}
		}

		private void DisableHelperCameras()
		{
			Camera[] componentsInChildren = base.GetComponentsInChildren<Camera>();
			Camera[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				Camera camera = array[i];
				camera.enabled = false;
			}
		}
	}
}
