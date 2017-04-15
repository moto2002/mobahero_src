using System;
using System.Collections.Generic;
using UnityEngine;

public class MeshClip1
{
	public List<Mesh> m_MeshFrames;

	public int GetFrameCount()
	{
		return this.m_MeshFrames.Count;
	}
}
