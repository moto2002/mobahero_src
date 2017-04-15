using System;
using System.Collections.Generic;
using UnityEngine;

public class MeshClip : MonoBehaviour
{
	public List<Mesh> m_MeshFrames;

	public int GetFrameCount()
	{
		return this.m_MeshFrames.Count;
	}
}
