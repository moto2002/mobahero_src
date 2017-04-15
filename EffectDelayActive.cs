using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

[RequireComponent(typeof(EffectPlayTool))]
public class EffectDelayActive : MonoBehaviour
{
	private EffectPlayTool playTool;

	private CoroutineManager cMgr;

	private Task activeTask;

	[HideInInspector]
	public bool activeInHierarchy
	{
		get
		{
			return base.gameObject.activeInHierarchy;
		}
	}

	private void Awake()
	{
		this.playTool = base.GetComponent<EffectPlayTool>();
	}

	private void OnDestroy()
	{
		if (this.cMgr != null)
		{
			this.cMgr.StopAllCoroutine();
			this.activeTask = null;
			this.cMgr = null;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void SetActive(bool _isActive, float _delay = 0f)
	{
		EffectQualityAdapter.SetReactiveAction(base.transform, delegate
		{
			this.SetActive(_isActive, _delay);
		});
		if (EffectQualityAdapter.IsEffectShutOff(base.transform))
		{
			return;
		}
		if (this.cMgr == null)
		{
			this.cMgr = new CoroutineManager();
		}
		if (this.activeTask != null)
		{
			this.cMgr.StopCoroutine(this.activeTask);
		}
		this.activeTask = this.cMgr.StartCoroutine(this.SetActive_IEnumerator(_isActive, _delay), true);
	}

	[DebuggerHidden]
	private IEnumerator SetActive_IEnumerator(bool _isActive, float _delay)
	{
		EffectDelayActive.<SetActive_IEnumerator>c__IteratorA2 <SetActive_IEnumerator>c__IteratorA = new EffectDelayActive.<SetActive_IEnumerator>c__IteratorA2();
		<SetActive_IEnumerator>c__IteratorA._delay = _delay;
		<SetActive_IEnumerator>c__IteratorA._isActive = _isActive;
		<SetActive_IEnumerator>c__IteratorA.<$>_delay = _delay;
		<SetActive_IEnumerator>c__IteratorA.<$>_isActive = _isActive;
		<SetActive_IEnumerator>c__IteratorA.<>f__this = this;
		return <SetActive_IEnumerator>c__IteratorA;
	}
}
