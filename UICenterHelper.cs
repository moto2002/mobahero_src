using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class UICenterHelper : MonoBehaviour
{
	[Serializable]
	public class SingleComp
	{
		public UIWidget widget;

		public float xOffset;
	}

	private enum RepositionType
	{
		Manual,
		OnEnable,
		OnUpdate,
		AfterEnable
	}

	[SerializeField]
	private UICenterHelper.RepositionType WhenToReposite = UICenterHelper.RepositionType.AfterEnable;

	[SerializeField]
	private List<UICenterHelper.SingleComp> ComponentsList = new List<UICenterHelper.SingleComp>();

	public void OnEnable()
	{
		if (this.WhenToReposite == UICenterHelper.RepositionType.OnEnable && this.ComponentsList != null && this.ComponentsList.Count > 0)
		{
			this.Reposition();
		}
		else if (this.WhenToReposite == UICenterHelper.RepositionType.AfterEnable)
		{
			base.StartCoroutine(this.DelayReposition());
		}
	}

	public void Update()
	{
		if (this.WhenToReposite == UICenterHelper.RepositionType.OnUpdate && this.ComponentsList != null && this.ComponentsList.Count > 0)
		{
			this.Reposition();
		}
	}

	[DebuggerHidden]
	private IEnumerator DelayReposition()
	{
		UICenterHelper.<DelayReposition>c__Iterator17E <DelayReposition>c__Iterator17E = new UICenterHelper.<DelayReposition>c__Iterator17E();
		<DelayReposition>c__Iterator17E.<>f__this = this;
		return <DelayReposition>c__Iterator17E;
	}

	[ContextMenu("Execute")]
	public void Reposition()
	{
		if (this.ComponentsList.Count == 0)
		{
			return;
		}
		float num = 0f;
		foreach (UICenterHelper.SingleComp current in this.ComponentsList)
		{
			num += (float)current.widget.width + current.xOffset;
		}
		float num2 = 0f;
		foreach (UICenterHelper.SingleComp current2 in this.ComponentsList)
		{
			num2 += (float)(current2.widget.width / 2);
			current2.widget.transform.localPosition = new Vector3(num2 - num / 2f, 0f);
			num2 += (float)(current2.widget.width / 2) + current2.xOffset;
		}
	}
}
