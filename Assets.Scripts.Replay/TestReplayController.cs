using System;
using UnityEngine;

namespace Assets.Scripts.Replay
{
	internal class TestReplayController : MonoBehaviour
	{
		private string[] _actions = new string[]
		{
			"none",
			"playing",
			"recording"
		};

		private string _timeScale = "1.0";

		private void OnGUI()
		{
			if (GUILayout.Button("startrecord", new GUILayoutOption[0]))
			{
			}
			if (GUILayout.Button("endrecord", new GUILayoutOption[0]))
			{
			}
			if (GUILayout.Button("ReplayLastRecord", new GUILayoutOption[0]))
			{
			}
			if (GUILayout.Button("EndReplay", new GUILayoutOption[0]))
			{
				GameManager.Instance.ReplayController.EndReplay();
			}
			this._timeScale = GUILayout.TextField(this._timeScale, new GUILayoutOption[0]);
			if (GUILayout.Button("timescale", new GUILayoutOption[0]))
			{
				Time.timeScale = float.Parse(this._timeScale);
			}
		}
	}
}
