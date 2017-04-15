using System;
using UnityEngine;

public class StartAndStopTween : MonoBehaviour
{
	public GameObject target;

	private void OnGUI()
	{
		if (GUILayout.Button("Start Bounce", new GUILayoutOption[0]))
		{
			iTweenEvent.GetEvent(this.target, "Bounce").Play();
		}
		if (GUILayout.Button("Stop Bounce", new GUILayoutOption[0]))
		{
			iTweenEvent.GetEvent(this.target, "Bounce").Stop();
		}
		if (GUILayout.Button("Start Color Fade", new GUILayoutOption[0]))
		{
			iTweenEvent.GetEvent(this.target, "Color Fade").Play();
		}
		if (GUILayout.Button("Stop Color Fade", new GUILayoutOption[0]))
		{
			iTweenEvent.GetEvent(this.target, "Color Fade").Stop();
		}
	}
}
