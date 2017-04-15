using System;
using UnityEngine;

public class FxmReplayDemo : MonoBehaviour
{
	public GameObject m_TargetPrefab;

	public GameObject m_InstanceObj;

	private void Start()
	{
		this.CreateEffect();
	}

	private void Update()
	{
	}

	private void CreateEffect()
	{
		if (this.m_TargetPrefab == null)
		{
			return;
		}
		this.m_InstanceObj = NsEffectManager.CreateReplayEffect(this.m_TargetPrefab);
		NsEffectManager.PreloadResource(this.m_InstanceObj);
	}

	private void Replay(bool bClearOldParticle)
	{
		NsEffectManager.RunReplayEffect(this.m_InstanceObj, bClearOldParticle);
	}

	private void OnGUI()
	{
		if (GUI.Button(FxmReplayDemo.GetButtonRect(0), "Replay"))
		{
			this.Replay(false);
		}
		if (GUI.Button(FxmReplayDemo.GetButtonRect(1), "Replay(ClearParticle)"))
		{
			this.Replay(true);
		}
	}

	public static Rect GetButtonRect(int nIndex)
	{
		return new Rect((float)(Screen.width - Screen.width / 8 * (nIndex + 1)), (float)(Screen.height - Screen.height / 10), (float)(Screen.width / 8), (float)(Screen.height / 10));
	}

	public static void SetActiveRecursively(GameObject target, bool bActive)
	{
		int num = target.transform.childCount - 1;
		while (0 <= num)
		{
			if (num < target.transform.childCount)
			{
				FxmReplayDemo.SetActiveRecursively(target.transform.GetChild(num).gameObject, bActive);
			}
			num--;
		}
		target.SetActive(bActive);
	}
}
