using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class CoroutineCom : MonoBehaviour
{
	public class CoroutineInfo
	{
		public string key;

		public IEnumerator ie;

		public Coroutine coroutine;
	}

	private Dictionary<string, CoroutineCom.CoroutineInfo> dic;

	private void Awake()
	{
		this.dic = new Dictionary<string, CoroutineCom.CoroutineInfo>();
	}

	public void StopAll()
	{
		base.StopAllCoroutines();
	}

	public void Begin(string key, IEnumerator ie)
	{
		if (!string.IsNullOrEmpty(key) && !this.dic.ContainsKey(key) && ie != null)
		{
			Coroutine coroutine = base.StartCoroutine(this.DoCoroutine(key, ie));
			this.dic.Add(key, new CoroutineCom.CoroutineInfo
			{
				key = key,
				ie = ie,
				coroutine = coroutine
			});
		}
	}

	public void End(string key)
	{
		if (!string.IsNullOrEmpty(key) && this.dic.ContainsKey(key))
		{
			base.StopCoroutine(this.dic[key].coroutine);
		}
	}

	[DebuggerHidden]
	private IEnumerator DoCoroutine(string key, IEnumerator ie)
	{
		CoroutineCom.<DoCoroutine>c__Iterator11F <DoCoroutine>c__Iterator11F = new CoroutineCom.<DoCoroutine>c__Iterator11F();
		<DoCoroutine>c__Iterator11F.ie = ie;
		<DoCoroutine>c__Iterator11F.key = key;
		<DoCoroutine>c__Iterator11F.<$>ie = ie;
		<DoCoroutine>c__Iterator11F.<$>key = key;
		<DoCoroutine>c__Iterator11F.<>f__this = this;
		return <DoCoroutine>c__Iterator11F;
	}
}
