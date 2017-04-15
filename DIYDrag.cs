using System;
using UnityEngine;

public class DIYDrag : MonoBehaviour
{
	private bool isFirstTime = true;

	private UIDragScrollView[] usv;

	[SerializeField]
	private UIDragScrollView ugH;

	[SerializeField]
	private UIDragScrollView ugV;

	private void Start()
	{
		this.usv = base.GetComponents<UIDragScrollView>();
	}

	private void Update()
	{
	}

	private void OnEnable()
	{
		this.isFirstTime = true;
		this.usv = base.GetComponents<UIDragScrollView>();
	}

	private void OnDisable()
	{
		this.isFirstTime = true;
	}

	private void OnDrag(Vector2 delta)
	{
		if (this.isFirstTime)
		{
			bool isSerilized = null != this.ugH && null != this.ugV;
			this.MakeItEnabled(delta, isSerilized);
			this.isFirstTime = false;
		}
	}

	private void OnPress(bool isPress)
	{
		if (!isPress)
		{
			if (null != this.ugH && null != this.ugV)
			{
				this.ugH.enabled = true;
				this.ugV.enabled = true;
			}
			else
			{
				this.usv[0].enabled = true;
				this.usv[1].enabled = true;
			}
			this.isFirstTime = true;
		}
	}

	private void MakeItEnabled(Vector2 delta, bool isSerilized)
	{
		if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
		{
			if (isSerilized)
			{
				if (null != this.ugH && null != this.ugV)
				{
					this.ugH.enabled = true;
					this.ugV.enabled = false;
				}
			}
			else if (this.usv != null && this.usv.Length > 1)
			{
				this.usv[0].enabled = true;
				this.usv[1].enabled = false;
			}
		}
		else if (isSerilized)
		{
			if (null != this.ugH && null != this.ugV)
			{
				this.ugH.enabled = false;
				this.ugV.enabled = true;
			}
		}
		else if (this.usv != null && this.usv.Length > 1)
		{
			this.usv[0].enabled = false;
			this.usv[1].enabled = true;
		}
	}
}
