using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class GhostShadow : MonoBehaviour
{
	[SerializeField]
	private SkinnedMeshRenderer m_renderer;

	[SerializeField]
	private Color m_startColor = Color.red;

	[SerializeField]
	private Color m_endColor = new Color(0.5f, 0f, 0f, 0f);

	[SerializeField]
	private float m_duration = 0.5f;

	[SerializeField]
	private bool m_autoBakeShadow = true;

	[SerializeField]
	private int m_shadowNumPerSecond = 6;

	public bool PlayEffect;

	private float m_time;

	private new string name;

	private Shader m_ghostShader;

	private bool isPlay;

	private void Awake()
	{
		this.m_renderer = base.gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
		this.name = base.gameObject.name;
		this.m_ghostShader = Shader.Find("MyShader/GhostShadow");
	}

	private void Update()
	{
		if (this.PlayEffect)
		{
			base.StartCoroutine("PlayEffect_Coroutine");
			this.PlayEffect = false;
		}
		if (this.m_autoBakeShadow)
		{
			this.m_time += Time.deltaTime;
			if (this.m_time > 1f / (float)this.m_shadowNumPerSecond)
			{
				this.m_time = 0f;
				base.StartCoroutine("PlayEffect_Coroutine");
			}
		}
	}

	private void OnDestroy()
	{
		base.StopAllCoroutines();
	}

	[DebuggerHidden]
	private IEnumerator PlayEffect_Coroutine()
	{
		GhostShadow.<PlayEffect_Coroutine>c__Iterator93 <PlayEffect_Coroutine>c__Iterator = new GhostShadow.<PlayEffect_Coroutine>c__Iterator93();
		<PlayEffect_Coroutine>c__Iterator.<>f__this = this;
		return <PlayEffect_Coroutine>c__Iterator;
	}

	public void Play(bool b)
	{
		if (b)
		{
			this.PlayEffect = b;
		}
		this.isPlay = b;
	}
}
