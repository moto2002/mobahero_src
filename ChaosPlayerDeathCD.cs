using Com.Game.Module;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class ChaosPlayerDeathCD : MonoBehaviour
{
	public UISprite HeadIcon;

	public GameObject CDMask;

	public UILabel CDLabel;

	private CoroutineManager cMgr;

	private Units unit;

	private void Start()
	{
		this.cMgr = new CoroutineManager();
	}

	private void OnDestroy()
	{
		if (this.cMgr != null)
		{
			this.cMgr.StopAllCoroutine();
			this.cMgr = null;
		}
	}

	public void SetData(Units _unit)
	{
		this.HeadIcon.spriteName = Singleton<HUDModuleManager>.Instance.GetSpriteNameById(_unit.npc_id);
	}

	public void ShowCd(float _time)
	{
		this.CDMask.SetActive(true);
		this.CDLabel.gameObject.SetActive(true);
		this.cMgr.StartCoroutine(this.Timer(_time), true);
	}

	[DebuggerHidden]
	private IEnumerator Timer(float _time)
	{
		ChaosPlayerDeathCD.<Timer>c__IteratorD6 <Timer>c__IteratorD = new ChaosPlayerDeathCD.<Timer>c__IteratorD6();
		<Timer>c__IteratorD._time = _time;
		<Timer>c__IteratorD.<$>_time = _time;
		<Timer>c__IteratorD.<>f__this = this;
		return <Timer>c__IteratorD;
	}
}
