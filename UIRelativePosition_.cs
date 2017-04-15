using System;
using UnityEngine;

public class UIRelativePosition_ : MonoBehaviour
{
	public enum RelatType
	{
		top = 1,
		down,
		left,
		right
	}

	public Transform Target;

	public UIRelativePosition_.RelatType relattype = UIRelativePosition_.RelatType.down;

	public float OffSet;

	private UIWidget TargetWidget;

	private UIWidget SelfWidget;

	private float t_top_y;

	private float t_down_y;

	private float s_top_y;

	private float s_down_y;

	private bool done;

	private int s_height;

	private float o_offset;

	private bool isVisable = true;

	private void OnEnable()
	{
		this.done = false;
		if (this.Target != null)
		{
			this.getxy();
		}
	}

	public void SetInvisiable()
	{
		base.transform.GetComponent<UIWidget>().height = 0;
		this.OffSet = 0f;
		base.transform.localScale = new Vector3(1f, 0f, 1f);
		this.isVisable = false;
	}

	public void SetVisiable()
	{
		base.transform.GetComponent<UIWidget>().height = 86;
		this.OffSet = 18f;
		base.transform.localScale = new Vector3(1f, 1f, 1f);
		this.isVisable = true;
	}

	public void SetPosition()
	{
		this.getxy();
		if (this.relattype == UIRelativePosition_.RelatType.top)
		{
			base.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y - this.s_down_y + this.t_top_y + this.OffSet, base.transform.localPosition.z);
		}
		else if (this.relattype == UIRelativePosition_.RelatType.down)
		{
			if (this.Target.GetComponent<UIRelativePosition_>() != null && this.Target.gameObject.activeSelf && this.Target.GetComponent<UIRelativePosition_>().isVisable)
			{
				base.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y - this.s_top_y + this.t_down_y - this.OffSet, base.transform.localPosition.z);
			}
			else
			{
				base.transform.localPosition = new Vector3(base.transform.localPosition.x, this.Target.transform.localPosition.y, base.transform.localPosition.z);
			}
		}
		else if (this.relattype != UIRelativePosition_.RelatType.left)
		{
			if (this.relattype == UIRelativePosition_.RelatType.right)
			{
			}
		}
	}

	private void getxy()
	{
		if (this.Target == null)
		{
			base.enabled = false;
			return;
		}
		if (null == this.TargetWidget)
		{
			this.TargetWidget = this.Target.GetComponent<UIWidget>();
			this.o_offset = this.OffSet;
		}
		if (null == this.SelfWidget)
		{
			this.SelfWidget = base.transform.GetComponent<UIWidget>();
			this.s_height = this.SelfWidget.height;
		}
		switch (this.TargetWidget.pivot)
		{
		case UIWidget.Pivot.TopLeft:
		case UIWidget.Pivot.Top:
		case UIWidget.Pivot.TopRight:
			this.t_top_y = this.Target.localPosition.y;
			this.t_down_y = this.Target.localPosition.y - (float)this.TargetWidget.height;
			break;
		case UIWidget.Pivot.Left:
		case UIWidget.Pivot.Center:
		case UIWidget.Pivot.Right:
			this.t_down_y = this.Target.localPosition.y - (float)(this.TargetWidget.height / 2);
			this.t_top_y = this.Target.localPosition.y + (float)(this.TargetWidget.height / 2);
			break;
		case UIWidget.Pivot.BottomLeft:
		case UIWidget.Pivot.Bottom:
		case UIWidget.Pivot.BottomRight:
			this.t_down_y = this.Target.localPosition.y;
			this.t_top_y = this.Target.localPosition.y + (float)this.TargetWidget.height;
			break;
		}
		switch (this.SelfWidget.pivot)
		{
		case UIWidget.Pivot.TopLeft:
		case UIWidget.Pivot.Top:
		case UIWidget.Pivot.TopRight:
			this.s_top_y = base.transform.localPosition.y;
			this.s_down_y = base.transform.localPosition.y - (float)this.SelfWidget.height;
			break;
		case UIWidget.Pivot.Left:
		case UIWidget.Pivot.Center:
		case UIWidget.Pivot.Right:
			this.s_down_y = base.transform.localPosition.y - (float)(this.SelfWidget.height / 2);
			this.s_top_y = base.transform.localPosition.y + (float)(this.SelfWidget.height / 2);
			break;
		case UIWidget.Pivot.BottomLeft:
		case UIWidget.Pivot.Bottom:
		case UIWidget.Pivot.BottomRight:
			this.s_down_y = base.transform.localPosition.y;
			this.s_top_y = base.transform.localPosition.y + (float)this.SelfWidget.height;
			break;
		}
	}
}
