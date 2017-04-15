using System;
using System.Collections.Generic;
using UnityEngine;

public class MeshAnimation : MonoBehaviour
{
	public CDummys[] dummys;

	public AnimPlayer m_ctrl;

	private MeshClip1 m_AnimaitonClip;

	private WrapMode m_WarpMode;

	private float m_fSpeed;

	private float m_fStartTime;

	private float m_fNextFrameTime;

	private int m_iCurrentFrame;

	private bool m_bIncrease;

	private int m_iFrameRate;

	private MeshFilter m_MeshFilter;

	private MeshCollider m_MeshCollider;

	public Transform[] dummytrs;

	public List<int> dummyidxcvt = new List<int>();

	public int currtimeeps = 2;

	public void setcurrtimeeps(int idx)
	{
		this.currtimeeps = idx;
	}

	public void CreateDummys()
	{
		this.dummytrs = new Transform[5];
		for (int i = 0; i < 5; i++)
		{
			this.dummytrs[i] = base.transform.Find("Dummy_" + i);
			if (this.dummytrs[i] != null)
			{
				this.dummyidxcvt.Add(i);
			}
		}
	}

	public void PlayAnimation(string path, string name, int rate, WrapMode mode, float fSpeed, int iFrameIndex)
	{
		this.dummys = ModelAnimation.s_dummy[path + "/" + name];
		MeshClip1 meshClips = ModelAnimation.getMeshClips(this.m_ctrl, path + "/" + name);
		if (meshClips != null)
		{
			this.PlayAnimation(meshClips, rate, mode, fSpeed, iFrameIndex);
		}
	}

	public bool IsPlaying()
	{
		return this.m_AnimaitonClip != null;
	}

	private void PlayAnimation(MeshClip1 clip, int frameRate, WrapMode mode, float speed, int iFrameIndex)
	{
		if (null == this.m_MeshFilter)
		{
			this.m_MeshFilter = base.gameObject.GetComponent<MeshFilter>();
		}
		this.m_AnimaitonClip = clip;
		this.m_WarpMode = mode;
		this.m_iFrameRate = frameRate;
		this.m_fSpeed = speed;
		this.m_fStartTime = 0f;
		this.m_fNextFrameTime = 0f;
		this.m_bIncrease = true;
		this.m_iCurrentFrame = iFrameIndex;
		if (this.m_iCurrentFrame > this.m_AnimaitonClip.m_MeshFrames.Count)
		{
			this.m_iCurrentFrame = this.m_AnimaitonClip.m_MeshFrames.Count - 1;
		}
		this.m_MeshFilter.mesh = this.m_AnimaitonClip.m_MeshFrames[this.m_iCurrentFrame];
	}

	private void Update()
	{
		if (this.m_AnimaitonClip == null)
		{
			return;
		}
		float num = Time.deltaTime * this.m_fSpeed;
		if (this.m_WarpMode == WrapMode.Loop)
		{
			this.m_fStartTime += num;
			this.m_fNextFrameTime += num;
			if (this.m_fNextFrameTime < 1f / (float)this.m_iFrameRate)
			{
				return;
			}
			this.m_fNextFrameTime = 0f;
			this.m_iCurrentFrame++;
			if (this.m_iCurrentFrame >= this.m_AnimaitonClip.GetFrameCount())
			{
				this.m_iCurrentFrame = 0;
			}
			this.SetCurrentFrame();
		}
		else if (this.m_WarpMode == WrapMode.PingPong)
		{
			this.m_fStartTime += num;
			this.m_fNextFrameTime += num;
			if (this.m_fNextFrameTime < 1f / (float)this.m_iFrameRate)
			{
				return;
			}
			this.m_fNextFrameTime = 0f;
			if (this.m_bIncrease)
			{
				this.m_iCurrentFrame++;
				if (this.m_iCurrentFrame == this.m_AnimaitonClip.GetFrameCount() - 1)
				{
					this.m_bIncrease = false;
				}
			}
			else
			{
				this.m_iCurrentFrame--;
				if (this.m_iCurrentFrame == 0)
				{
					this.m_bIncrease = true;
				}
			}
			if (this.m_iCurrentFrame >= this.m_AnimaitonClip.GetFrameCount())
			{
				this.m_iCurrentFrame = 0;
			}
			this.SetCurrentFrame();
		}
		else if (this.m_WarpMode == WrapMode.ClampForever)
		{
			this.m_fStartTime += num;
			if (this.m_iCurrentFrame < this.m_AnimaitonClip.GetFrameCount() - 1)
			{
				this.m_fNextFrameTime += num;
				if (this.m_fNextFrameTime < 1f / (float)this.m_iFrameRate)
				{
					return;
				}
				this.m_fNextFrameTime = 0f;
				this.m_iCurrentFrame++;
				this.SetCurrentFrame();
			}
		}
		else if (this.m_WarpMode == WrapMode.Once)
		{
			this.m_fStartTime += num;
			if (this.m_iCurrentFrame < this.m_AnimaitonClip.GetFrameCount() - 1)
			{
				this.m_fNextFrameTime += num;
				if (this.m_fNextFrameTime < 1f / (float)this.m_iFrameRate)
				{
					return;
				}
				this.m_fNextFrameTime = 0f;
				this.m_iCurrentFrame++;
				this.SetCurrentFrame();
			}
			else
			{
				this.m_AnimaitonClip = null;
			}
		}
		else if (this.m_WarpMode == WrapMode.Once)
		{
			this.m_fStartTime += num;
			this.m_fNextFrameTime += num;
			if (this.m_fNextFrameTime < 1f / (float)this.m_iFrameRate)
			{
				return;
			}
			this.m_fNextFrameTime = 0f;
			this.m_iCurrentFrame++;
			if (this.m_iCurrentFrame >= this.m_AnimaitonClip.GetFrameCount())
			{
				this.m_iCurrentFrame = 0;
			}
			this.SetCurrentFrame();
			if (this.m_iCurrentFrame == 0)
			{
				this.m_AnimaitonClip = null;
			}
		}
	}

	private void SetCurrentFrame()
	{
		this.m_MeshFilter.mesh = this.m_AnimaitonClip.m_MeshFrames[this.m_iCurrentFrame];
		for (int i = 0; i < this.dummyidxcvt.Count; i++)
		{
			if (this.dummytrs[this.dummyidxcvt[i]] != null)
			{
				this.dummytrs[this.dummyidxcvt[i]].localPosition = this.dummys[i].dummylist_pos[this.m_iCurrentFrame];
				this.dummytrs[this.dummyidxcvt[i]].localRotation = this.dummys[i].dummylist_rot[this.m_iCurrentFrame];
			}
		}
	}

	public int GetCurrentFrame()
	{
		return this.m_iCurrentFrame;
	}

	public float GetPercentage()
	{
		if (this.m_AnimaitonClip == null)
		{
			return 0f;
		}
		return (float)this.m_iCurrentFrame / (float)this.m_AnimaitonClip.GetFrameCount();
	}
}
