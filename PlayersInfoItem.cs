using Com.Game.Data;
using Com.Game.Manager;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class PlayersInfoItem : MonoBehaviour
{
	public UISprite enemyIcon;

	public UISprite mask;

	public UILabel timer;

	private CoroutineManager cMgr = new CoroutineManager();

	private void Awake()
	{
	}

	private void OnDisable()
	{
		this.cMgr.StopAllCoroutine();
	}

	private void OnDestroy()
	{
		this.cMgr.StopAllCoroutine();
		this.cMgr = null;
	}

	public void Play(int _uniqId, string _npcId, float _time)
	{
		this.cMgr.StartCoroutine(this.Timer(_uniqId, _time), true);
		SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(_npcId);
		if (heroMainData != null)
		{
			this.enemyIcon.spriteName = heroMainData.avatar_icon;
		}
	}

	[DebuggerHidden]
	private IEnumerator Timer(int _uniqId, float _time)
	{
		PlayersInfoItem.<Timer>c__IteratorDD <Timer>c__IteratorDD = new PlayersInfoItem.<Timer>c__IteratorDD();
		<Timer>c__IteratorDD._time = _time;
		<Timer>c__IteratorDD._uniqId = _uniqId;
		<Timer>c__IteratorDD.<$>_time = _time;
		<Timer>c__IteratorDD.<$>_uniqId = _uniqId;
		<Timer>c__IteratorDD.<>f__this = this;
		return <Timer>c__IteratorDD;
	}
}
