using Com.Game.Module;
using System;
using UnityEngine;

public class KHDragDropItem : UIDragDropItem
{
	public GameObject prefab;

	public Callback<MatchItem> Release;

	protected override void OnDragDropStart()
	{
		AudioMgr.PlayUI("Play_Menu_click", null, false, false);
		Singleton<PvpRoomView>.Instance.IsLock = true;
		if (this.mDragScrollView != null)
		{
			this.mDragScrollView.enabled = false;
		}
		if (this.mCollider != null)
		{
			this.mCollider.enabled = false;
		}
		this.mTouchID = UICamera.currentTouchID;
		this.mParent = this.mTrans.parent;
		this.mRoot = NGUITools.FindInParents<UIRoot>(this.mParent);
		this.mGrid = NGUITools.FindInParents<UIGrid>(this.mParent);
		this.mTable = NGUITools.FindInParents<UITable>(this.mParent);
		if (UIDragDropRoot.root != null)
		{
			this.mTrans.parent = UIDragDropRoot.root;
		}
		Vector3 localPosition = this.mTrans.localPosition;
		localPosition.z = 0f;
		this.mTrans.localPosition = localPosition;
		TweenPosition component = base.GetComponent<TweenPosition>();
		if (component != null)
		{
			component.enabled = false;
		}
		SpringPosition component2 = base.GetComponent<SpringPosition>();
		if (component2 != null)
		{
			component2.enabled = false;
		}
		NGUITools.MarkParentAsChanged(base.gameObject);
		if (this.mTable != null)
		{
			this.mTable.repositionNow = true;
		}
		if (this.mGrid != null)
		{
			this.mGrid.repositionNow = true;
		}
	}

	protected override void OnDragDropMove(Vector3 delta)
	{
		Singleton<PvpRoomView>.Instance.ShowMatchItemFrame(base.GetComponent<MatchItem>());
		if (delta.y != 0f && !Singleton<PvpRoomView>.Instance.JudgeRelativePosition(base.gameObject, delta.y > 0f))
		{
			return;
		}
		this.mTrans.localPosition = new Vector3(130f, this.mTrans.localPosition.y + delta.y, this.mTrans.localPosition.z);
		Singleton<PvpRoomView>.Instance.Glide(base.transform);
	}

	protected override void OnDragDropRelease(GameObject surface)
	{
		if (surface != null)
		{
			ExampleDragDropSurface component = surface.GetComponent<ExampleDragDropSurface>();
			if (component != null)
			{
				GameObject gameObject = NGUITools.AddChild(component.gameObject, this.prefab);
				gameObject.transform.localScale = component.transform.localScale;
				Transform transform = gameObject.transform;
				transform.position = UICamera.lastHit.point;
				if (component.rotatePlacedObject)
				{
					transform.rotation = Quaternion.LookRotation(UICamera.lastHit.normal) * Quaternion.Euler(90f, 0f, 0f);
				}
				NGUITools.Destroy(base.gameObject);
				Singleton<PvpRoomView>.Instance.IsLock = false;
				return;
			}
		}
		if (!this.cloneOnDrag)
		{
			this.mTouchID = -2147483648;
			if (this.mCollider != null)
			{
				this.mCollider.enabled = true;
			}
			UIDragDropContainer uIDragDropContainer = (!surface) ? null : NGUITools.FindInParents<UIDragDropContainer>(surface);
			if (uIDragDropContainer != null)
			{
				this.mTrans.parent = ((!(uIDragDropContainer.reparentTarget != null)) ? uIDragDropContainer.transform : uIDragDropContainer.reparentTarget);
				Vector3 localPosition = this.mTrans.localPosition;
				localPosition.z = 0f;
				this.mTrans.localPosition = localPosition;
			}
			else
			{
				this.mTrans.parent = this.mParent;
			}
			this.mParent = this.mTrans.parent;
			this.mGrid = NGUITools.FindInParents<UIGrid>(this.mParent);
			this.mTable = NGUITools.FindInParents<UITable>(this.mParent);
			if (this.mDragScrollView != null)
			{
				this.mDragScrollView.enabled = true;
			}
			NGUITools.MarkParentAsChanged(base.gameObject);
			if (this.mTable != null)
			{
				this.mTable.repositionNow = true;
			}
			if (this.mGrid != null)
			{
				this.mGrid.repositionNow = true;
			}
		}
		else
		{
			Singleton<PvpRoomView>.Instance.ReplaceMatchPosition(base.GetComponent<MatchItem>());
			NGUITools.Destroy(base.gameObject);
		}
		Singleton<PvpRoomView>.Instance.IsLock = false;
		AudioMgr.PlayUI("Play_UI_Page", null, false, false);
	}
}
