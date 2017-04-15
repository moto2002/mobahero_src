using System;
using UnityEngine;

public class ScreenRotation : MonoBehaviour
{
	private void Awake()
	{
		Screen.orientation = ScreenOrientation.LandscapeRight;
	}

	private void Start()
	{
		Screen.orientation = ScreenOrientation.AutoRotation;
		Screen.autorotateToLandscapeLeft = true;
		Screen.autorotateToLandscapeRight = true;
		Screen.autorotateToPortrait = false;
		Screen.autorotateToPortraitUpsideDown = false;
	}

	private void Update()
	{
	}
}
