using System;
using UnityEngine;

public class JasonReplay : MonoBehaviour
{
	[Header("按空格键重播:)")]
	public float 播放速度 = 1f;

	private void Start()
	{
	}

	private void Update()
	{
		Time.timeScale = this.播放速度;
		if (Input.GetKey(KeyCode.Space))
		{
			this.Jason();
		}
	}

	public void Jason()
	{
		Application.LoadLevel(Application.loadedLevel);
	}
}
