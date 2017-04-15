using System;
using UnityEngine;

public class MeshVideoRenderer : MonoBehaviour
{
	public VideoPlayer VideoPlayer;

	private MeshFilter meshFilter_;

	private Renderer renderer_;

	private void Start()
	{
		this.renderer_ = base.renderer;
		this.meshFilter_ = base.GetComponent<MeshFilter>();
	}

	private void Update()
	{
		if (this.VideoPlayer != null)
		{
			if (this.VideoPlayer.IsPlaying)
			{
				if (this.renderer_ != null)
				{
					this.renderer_.enabled = false;
				}
				Graphics.DrawMesh(this.meshFilter_.sharedMesh, base.transform.localToWorldMatrix, this.VideoPlayer.VideoOutput, 0);
			}
			else if (this.renderer_ != null)
			{
				this.renderer_.enabled = true;
			}
		}
	}
}
