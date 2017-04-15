using System;
using System.Collections.Generic;
using UnityEngine;

public class ModelAnimation
{
	private class TriAndUVInfo
	{
		public Vector2[] m_uv;

		public int[] m_triangles;
	}

	public class AnimationInfo
	{
		public int iFrameCount;

		public int iFrameRate;

		public List<AnimationEvent> listEvent;

		public AnimationInfo()
		{
			this.iFrameCount = 0;
			this.iFrameRate = 0;
			this.listEvent = new List<AnimationEvent>();
		}
	}

	public static Material m_material;

	private static Dictionary<string, ModelAnimation.AnimationInfo> s_mapAnimationInfo = null;

	private static Dictionary<string, Dictionary<string, List<Mesh>>> s_mapMeshCenter = null;

	private static Dictionary<string, string> s_meshmat = null;

	public static Dictionary<string, CDummys[]> s_dummy = new Dictionary<string, CDummys[]>();

	private static int m_iEventPostIndex;

	private static float m_fCurPlayTime;

	private static WrapMode m_WarpMode;

	private static float m_fSpeed;

	private static ModelAnimation.AnimationInfo m_AnimationInfo;

	private static List<AnimationEvent> m_listEvent = new List<AnimationEvent>();

	private static int playingstartframe = 0;

	public static MeshClip1 getMeshClips(AnimPlayer ctrl, string name)
	{
		if (ModelAnimation.s_mapMeshCenter.ContainsKey(name))
		{
			Dictionary<string, List<Mesh>> dictionary = ModelAnimation.s_mapMeshCenter[name];
			MeshClip1 meshClip = new MeshClip1();
			string text = ctrl.meshanim[0].name;
			text = text.Substring(0, text.Length - 1);
			if (ctrl.meshanim != null && dictionary.ContainsKey(text))
			{
				meshClip.m_MeshFrames = dictionary[text];
				return meshClip;
			}
		}
		return null;
	}

	public static void ClearData()
	{
		if (ModelAnimation.s_mapAnimationInfo != null)
		{
			ModelAnimation.s_mapAnimationInfo.Clear();
		}
		if (ModelAnimation.s_mapMeshCenter != null)
		{
			Dictionary<string, List<Mesh>> dictionary = null;
			foreach (KeyValuePair<string, Dictionary<string, List<Mesh>>> current in ModelAnimation.s_mapMeshCenter)
			{
				dictionary = current.Value;
				if (dictionary != null)
				{
					foreach (KeyValuePair<string, List<Mesh>> current2 in dictionary)
					{
						if (current2.Value != null)
						{
							current2.Value.Clear();
						}
					}
					dictionary.Clear();
				}
			}
			ModelAnimation.s_mapMeshCenter.Clear();
		}
		if (ModelAnimation.s_meshmat != null)
		{
			ModelAnimation.s_meshmat.Clear();
		}
		if (ModelAnimation.s_dummy != null)
		{
			ModelAnimation.s_dummy.Clear();
		}
	}

	public static ModelAnimation.AnimationInfo GetAnimationInfo(string aniName)
	{
		if (!ModelAnimation.s_mapAnimationInfo.ContainsKey(aniName))
		{
			return null;
		}
		return ModelAnimation.s_mapAnimationInfo[aniName];
	}

	public static void UpdateEvent(GameObject gm)
	{
		if (ModelAnimation.m_AnimationInfo == null)
		{
			return;
		}
		if (ModelAnimation.m_AnimationInfo.listEvent.Count == 0)
		{
			return;
		}
		float deltaTime = Time.deltaTime;
		float num = (float)ModelAnimation.m_AnimationInfo.iFrameCount * (1f / (float)ModelAnimation.m_AnimationInfo.iFrameRate);
		ModelAnimation.m_fCurPlayTime += deltaTime;
		if (ModelAnimation.m_WarpMode == WrapMode.Loop)
		{
			while (ModelAnimation.m_listEvent.Count != 0)
			{
				AnimationEvent animationEvent = ModelAnimation.m_listEvent[0];
				if (animationEvent.time > ModelAnimation.m_fCurPlayTime)
				{
					break;
				}
				if (animationEvent.stringParameter.Length > 0)
				{
					gm.SendMessage(animationEvent.functionName, animationEvent.stringParameter, animationEvent.messageOptions);
				}
				else
				{
					gm.SendMessage(animationEvent.functionName, animationEvent.messageOptions);
				}
				ModelAnimation.m_listEvent.RemoveAt(0);
			}
			if (ModelAnimation.m_fCurPlayTime >= num)
			{
				ModelAnimation.m_fCurPlayTime = 0f;
				for (int i = 0; i < ModelAnimation.m_AnimationInfo.listEvent.Count; i++)
				{
					AnimationEvent item = ModelAnimation.m_AnimationInfo.listEvent[i];
					ModelAnimation.m_listEvent.Add(item);
				}
			}
		}
		else if (ModelAnimation.m_WarpMode != WrapMode.PingPong)
		{
			if (ModelAnimation.m_WarpMode == WrapMode.ClampForever || ModelAnimation.m_WarpMode == WrapMode.Once || ModelAnimation.m_WarpMode == WrapMode.Once)
			{
				while (ModelAnimation.m_listEvent.Count != 0)
				{
					AnimationEvent animationEvent2 = ModelAnimation.m_listEvent[0];
					if (animationEvent2.time > ModelAnimation.m_fCurPlayTime)
					{
						break;
					}
					if (animationEvent2.stringParameter.Length > 0)
					{
						gm.SendMessage(animationEvent2.functionName, animationEvent2.stringParameter, animationEvent2.messageOptions);
					}
					else
					{
						gm.SendMessage(animationEvent2.functionName, animationEvent2.messageOptions);
					}
					ModelAnimation.m_listEvent.RemoveAt(0);
				}
			}
		}
	}

	public static bool isPlaying(GameObject gm, string name)
	{
		MeshAnimation[] componentsInChildren = gm.GetComponentsInChildren<MeshAnimation>();
		return componentsInChildren[0].IsPlaying();
	}

	public static bool isPlayingContain(GameObject gm, string name)
	{
		MeshAnimation[] componentsInChildren = gm.GetComponentsInChildren<MeshAnimation>();
		return componentsInChildren[0].IsPlaying();
	}

	public static void setpercent(ModelAnimation.AnimationInfo ai, float pct)
	{
		if (ai != null)
		{
			ModelAnimation.playingstartframe = (int)(pct * (float)ai.iFrameCount);
		}
	}

	public static void Play(ModelAnimation.AnimationInfo ai, GameObject gm, MeshAnimation[] tma, string res, string name, WrapMode mode, float time)
	{
		if (gm.animation.enabled)
		{
			return;
		}
		ModelAnimation.m_AnimationInfo = ai;
		if (ModelAnimation.m_AnimationInfo == null)
		{
			return;
		}
		float num = 1f / (float)ModelAnimation.m_AnimationInfo.iFrameRate;
		if (time < 0.1f)
		{
			time = 0.1f;
		}
		float num2 = num * (float)ModelAnimation.m_AnimationInfo.iFrameCount;
		float fSpeed = num2 / time;
		ModelAnimation.m_WarpMode = mode;
		ModelAnimation.m_fSpeed = fSpeed;
		ModelAnimation.m_iEventPostIndex = -1;
		int num3 = 0;
		ModelAnimation.m_fCurPlayTime = (float)num3 * num;
		for (int i = 0; i < ModelAnimation.m_AnimationInfo.listEvent.Count; i++)
		{
			AnimationEvent animationEvent = ModelAnimation.m_AnimationInfo.listEvent[i];
			if (animationEvent.time >= ModelAnimation.m_fCurPlayTime)
			{
				ModelAnimation.m_listEvent.Add(animationEvent);
			}
		}
		for (int j = 0; j < tma.Length; j++)
		{
			tma[j].PlayAnimation(res, name, ModelAnimation.m_AnimationInfo.iFrameRate, mode, fSpeed, ModelAnimation.playingstartframe);
		}
		ModelAnimation.playingstartframe = 0;
	}

	public static void Play(GameObject gm, MeshAnimation[] tma, string res, string name, WrapMode mode, float fSpeed, bool bRandomFrameIndex)
	{
		ModelAnimation.m_AnimationInfo = ModelAnimation.GetAnimationInfo(res + "/" + name);
		if (ModelAnimation.m_AnimationInfo == null)
		{
			return;
		}
		ModelAnimation.m_WarpMode = mode;
		ModelAnimation.m_fSpeed = fSpeed;
		ModelAnimation.m_iEventPostIndex = -1;
		int num = 0;
		if (bRandomFrameIndex)
		{
			num = UnityEngine.Random.Range(0, ModelAnimation.m_AnimationInfo.iFrameCount);
		}
		ModelAnimation.m_fCurPlayTime = (float)num * (1f / (float)ModelAnimation.m_AnimationInfo.iFrameRate);
		for (int i = 0; i < ModelAnimation.m_AnimationInfo.listEvent.Count; i++)
		{
			AnimationEvent animationEvent = ModelAnimation.m_AnimationInfo.listEvent[i];
			if (animationEvent.time >= ModelAnimation.m_fCurPlayTime)
			{
				ModelAnimation.m_listEvent.Add(animationEvent);
			}
		}
		for (int j = 0; j < tma.Length; j++)
		{
			tma[j].PlayAnimation(res, name, ModelAnimation.m_AnimationInfo.iFrameRate, mode, fSpeed, ModelAnimation.playingstartframe);
		}
		ModelAnimation.playingstartframe = 0;
	}

	public static void Stop(GameObject gm, MeshAnimation[] tma, string res, string name)
	{
		if (gm.animation.enabled)
		{
			return;
		}
		ModelAnimation.m_AnimationInfo = ModelAnimation.GetAnimationInfo(res + "/" + name);
		if (ModelAnimation.m_AnimationInfo == null)
		{
			return;
		}
		ModelAnimation.m_WarpMode = WrapMode.Once;
		ModelAnimation.m_fSpeed = 1.2f;
		ModelAnimation.m_iEventPostIndex = -1;
		int num = 1000;
		ModelAnimation.m_fCurPlayTime = (float)num * (1f / (float)ModelAnimation.m_AnimationInfo.iFrameRate);
		for (int i = 0; i < tma.Length; i++)
		{
			tma[i].PlayAnimation(res, name, ModelAnimation.m_AnimationInfo.iFrameRate, ModelAnimation.m_WarpMode, ModelAnimation.m_fSpeed, num);
		}
	}

	public static Transform GetDummyOnAni(GameObject gm, MeshAnimation[] tma, int idx)
	{
		for (int i = 0; i < tma.Length; i++)
		{
			if (tma[i].dummyidxcvt.Count > 0 && tma[i].dummytrs != null && idx < tma[i].dummytrs.Length)
			{
				return tma[i].dummytrs[idx];
			}
		}
		return null;
	}

	protected static GameObject CreateParts(GameObject gm, Vector3 ps, Quaternion rot, string partName, Material material)
	{
		Transform transform = gm.transform.FindChild(partName);
		if (null == transform)
		{
			GameObject gameObject = new GameObject(partName);
			gameObject.transform.parent = gm.transform;
			gameObject.transform.localPosition = Vector3.zero;
			MeshAnimation meshAnimation = gameObject.AddComponent<MeshAnimation>();
			gameObject.AddComponent<MeshFilter>();
			MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
			meshRenderer.material = material;
			return gameObject;
		}
		return transform.gameObject;
	}

	protected static MeshClip CreatePartAnimation(GameObject partObj, string path, string aniName)
	{
		Transform transform = partObj.transform.FindChild(aniName);
		MeshClip result;
		if (null == transform)
		{
			GameObject gameObject = new GameObject(aniName);
			gameObject.transform.parent = partObj.transform;
			gameObject.transform.localPosition = Vector3.zero;
			result = gameObject.AddComponent<MeshClip>();
			Transform transform2 = gameObject.transform;
		}
		else
		{
			result = transform.gameObject.GetComponent<MeshClip>();
		}
		return result;
	}
}
