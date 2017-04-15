using Com.Game.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace MobaHeros
{
	public class MapPointContainer
	{
		private readonly Dictionary<TeamType, Dictionary<int, Transform>> _spawnPointsDict = new Dictionary<TeamType, Dictionary<int, Transform>>(default(EnumEqualityComparer<TeamType>));

		private readonly Dictionary<string, Transform> _lmGuidePoints = new Dictionary<string, Transform>();

		private readonly Dictionary<string, Transform> _blGuidePoints = new Dictionary<string, Transform>();

		private readonly Dictionary<int, Transform> _cameraSpawnPoints = new Dictionary<int, Transform>();

		private Transform _spawnRoot;

		public Dictionary<int, Transform> CameraSpawnPoints
		{
			get
			{
				return this._cameraSpawnPoints;
			}
		}

		public Transform GetSpawnPos(TeamType team, int key)
		{
			Dictionary<int, Transform> dictionary = this._spawnPointsDict[team];
			Transform result;
			if (dictionary.TryGetValue(key, out result))
			{
				return result;
			}
			ClientLogger.Warn(string.Concat(new object[]
			{
				"GetSpawnPos failed for: ",
				team,
				" ",
				key
			}));
			Transform transform = dictionary.Values.FirstOrDefault<Transform>();
			return transform ? transform : this._spawnPointsDict[TeamType.LM].First<KeyValuePair<int, Transform>>().Value;
		}

		public Dictionary<string, Transform> GetGuidePoints(TeamType team)
		{
			if (team == TeamType.LM)
			{
				return this._lmGuidePoints;
			}
			return this._blGuidePoints;
		}

		public Transform GetGuidePoint(TeamType team, int line, int pos)
		{
			Dictionary<string, Transform> dictionary = this._lmGuidePoints;
			if (team == TeamType.BL)
			{
				dictionary = this._blGuidePoints;
			}
			string key = pos.ToString();
			if (line > 0)
			{
				key = line + "_" + pos;
			}
			if (dictionary == null || !dictionary.ContainsKey(key))
			{
				return null;
			}
			return dictionary[key];
		}

		private void InitSpawnPoints()
		{
			Dictionary<TeamType, string> dictionary = new Dictionary<TeamType, string>
			{
				{
					TeamType.LM,
					"LM"
				},
				{
					TeamType.BL,
					"BL"
				},
				{
					TeamType.Neutral,
					"NE"
				},
				{
					TeamType.Team_3,
					"Team3"
				}
			};
			for (int i = 0; i < 4; i++)
			{
				this._spawnPointsDict[(TeamType)i] = new Dictionary<int, Transform>();
			}
			foreach (KeyValuePair<TeamType, string> current in dictionary)
			{
				this.Collect(this._spawnPointsDict[current.Key], current.Value);
			}
		}

		private void Collect(Dictionary<int, Transform> dict, string nodeName)
		{
			dict.Clear();
			Transform transform = this._spawnRoot.FindChild(nodeName);
			if (!transform)
			{
				return;
			}
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				int num = 0;
				if (int.TryParse(child.name, out num))
				{
					dict.Add(int.Parse(child.name), child);
				}
				else if (child.childCount != 0)
				{
					Transform child2 = child.GetChild(0);
					if (!(child2.name == "Door") && !(child2.name == "Sphere"))
					{
						if (int.TryParse(child2.name, out num))
						{
							dict.Add(int.Parse(child2.name), child2);
						}
						else
						{
							Debug.LogError("Collect point not int:" + child2.name + " parent:" + transform.name);
						}
					}
				}
				else
				{
					Debug.LogError("Collect point not int:" + child.name + " parent:" + transform.name);
				}
			}
		}

		private void CollectGuide(Dictionary<string, Transform> dict, string nodeName)
		{
			dict.Clear();
			Transform transform = this._spawnRoot.FindChild(nodeName);
			if (!transform)
			{
				return;
			}
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				dict.Add(child.name, child);
			}
		}

		public void Init()
		{
			this._spawnRoot = GameObject.FindGameObjectWithTag("SpawnPoint").transform;
			this.InitSpawnPoints();
			this.Collect(this._cameraSpawnPoints, "Camera");
			this.CollectGuide(this._lmGuidePoints, "Guide_LM");
			this.CollectGuide(this._blGuidePoints, "Guide_BL");
		}

		public void Uninit()
		{
			this.CameraSpawnPoints.Clear();
			this._spawnPointsDict.Clear();
			this._lmGuidePoints.Clear();
			this._blGuidePoints.Clear();
		}
	}
}
