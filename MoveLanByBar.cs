using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class MoveLanByBar : MonoBehaviour
{
	public UIProgressBar bar;

	public float length = 654f;

	public float addLength = 5f;

	public float offX;

	private float tempValue;

	private float oldValue;

	private float oldLength;

	private float oldAddLength;

	private float oldOffX;

	public UILabel prompt;

	private AudioClipInfo mpNotEnoughClipInfo = default(AudioClipInfo);

	[SerializeField]
	private UILabel range;

	[SerializeField]
	private UILabel speed;

	private float oldRestore;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void FixMoveLan()
	{
		this.tempValue = this.bar.value;
		if (this.tempValue != this.oldValue || this.length != this.oldLength || this.addLength != this.oldAddLength || this.oldOffX != this.offX)
		{
			this.oldValue = this.bar.value;
			this.oldLength = this.length;
			this.oldAddLength = this.addLength;
			this.oldOffX = this.offX;
		}
	}

	public void ShowQueLan()
	{
		this.prompt.gameObject.SetActive(true);
		this.prompt.enabled = true;
		this.prompt.text = "魔法值不足";
		if (this.mpNotEnoughClipInfo.clipName != "sd_int_close")
		{
			this.mpNotEnoughClipInfo = default(AudioClipInfo);
			this.mpNotEnoughClipInfo.clipName = "sd_int_close";
			this.mpNotEnoughClipInfo.audioSourceType = eAudioSourceType.UI;
			this.mpNotEnoughClipInfo.audioPriority = 128;
			this.mpNotEnoughClipInfo.volume = 1f;
		}
		AudioMgr.Play(this.mpNotEnoughClipInfo, null);
		base.StartCoroutine(this.DelayHide());
	}

	[DebuggerHidden]
	private IEnumerator DelayHide()
	{
		MoveLanByBar.<DelayHide>c__IteratorE7 <DelayHide>c__IteratorE = new MoveLanByBar.<DelayHide>c__IteratorE7();
		<DelayHide>c__IteratorE.<>f__this = this;
		return <DelayHide>c__IteratorE;
	}

	public void SetCurrentAndMax(int cur, int max)
	{
		this.range.text = cur.ToString() + "/" + max.ToString();
	}

	public void SetRestoreSpeed(float restore)
	{
		if (Math.Abs(this.oldRestore - restore) > 0.1f)
		{
			this.oldRestore = restore;
			this.speed.text = restore.ToString("0.0") + "/s";
		}
	}
}
