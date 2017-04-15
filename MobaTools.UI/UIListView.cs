using System;
using UnityEngine;

namespace MobaTools.UI
{
	[AddComponentMenu("MobaTools/Scripts/List View")]
	public class UIListView : MonoBehaviour
	{
		public delegate void OnItemChange(Transform go, int targetIndex);

		public delegate void OnClickItem(Transform go, int i);

		public int itemSize = 100;

		public int dataCount = -1;

		public int itemCount = -1;

		public bool mHorizontal;

		public bool mAuto;

		private bool cullContent;

		private bool OpenLog;

		private Transform m_Trans;

		private UIPanel m_Panel;

		private UIScrollView m_Scroll;

		private UIGrid m_Grid;

		private UICenterOnChild m_Center;

		private BetterList<Transform> mChildren = new BetterList<Transform>();

		private BetterList<int> mChildrenDataIndex = new BetterList<int>();

		private int m_startIndex;

		private int m_MaxCount;

		private int m_dataIndex = -1;

		private int m_seekIndex;

		private UIListView.OnItemChange m_pItemChangeCallBack;

		private UIListView.OnClickItem m_pOnClickItemCallBack;

		public Transform mTrans
		{
			get
			{
				if (this.m_Trans == null)
				{
					this.m_Trans = base.transform;
				}
				return this.m_Trans;
			}
		}

		public UIPanel mPanel
		{
			get
			{
				if (this.m_Panel == null)
				{
					this.m_Panel = NGUITools.FindInParents<UIPanel>(base.gameObject);
				}
				return this.m_Panel;
			}
		}

		public UIScrollView mScroll
		{
			get
			{
				if (this.m_Scroll == null)
				{
					this.m_Scroll = this.mPanel.GetComponent<UIScrollView>();
				}
				return this.m_Scroll;
			}
		}

		public UIGrid mGrid
		{
			get
			{
				if (this.m_Grid == null)
				{
				}
				return this.m_Grid;
			}
		}

		protected void Start()
		{
			if (this.mAuto)
			{
				this.InitListView(this.dataCount, this.itemCount, this.m_seekIndex);
			}
		}

		protected virtual void OnMove(UIPanel panel)
		{
			this.WrapContent();
		}

		public void OnUpdate()
		{
			this.WrapContent();
		}

		[ContextMenu("Sort Based on Scroll Movement")]
		public void SortBasedOnScrollMovement(int seekIndex = 0)
		{
			this.mChildren.Clear();
			this.mChildrenDataIndex.Clear();
			for (int i = 0; i < this.mTrans.childCount; i++)
			{
				if (i < this.itemCount)
				{
					this.mChildren.Add(this.mTrans.GetChild(i));
					this.mChildrenDataIndex.Add(seekIndex + i);
				}
				else
				{
					this.mTrans.GetChild(i).gameObject.SetActive(false);
				}
			}
			this.CachePanel();
			this.CacheScrollView();
			this.CacheGridView();
			this.ResetChildPositions();
		}

		protected bool CachePanel()
		{
			this.mPanel.onClipMove = new UIPanel.OnClippingMoved(this.OnMove);
			return true;
		}

		protected bool CacheScrollView()
		{
			if (this.mScroll == null)
			{
				return false;
			}
			this.mScroll.restrictWithinPanel = true;
			this.mScroll.dragEffect = UIScrollView.DragEffect.MomentumAndSpring;
			if (this.mScroll.movement == UIScrollView.Movement.Horizontal)
			{
				this.mHorizontal = true;
			}
			else
			{
				if (this.mScroll.movement != UIScrollView.Movement.Vertical)
				{
					return false;
				}
				this.mHorizontal = false;
			}
			return true;
		}

		protected bool CacheGridView()
		{
			if (this.mGrid == null)
			{
				return false;
			}
			if (this.mHorizontal)
			{
				this.mGrid.arrangement = UIGrid.Arrangement.Horizontal;
				this.mGrid.cellWidth = (float)this.itemSize;
			}
			else
			{
				this.mGrid.arrangement = UIGrid.Arrangement.Vertical;
				this.mGrid.cellHeight = (float)this.itemSize;
			}
			return true;
		}

		public void ResetChildPositions()
		{
			for (int i = 0; i < this.mChildren.size; i++)
			{
				Transform transform = this.mChildren[i];
				transform.localPosition = ((!this.mHorizontal) ? new Vector3(0f, (float)(-(float)i * this.itemSize), 0f) : new Vector3((float)(i * this.itemSize), 0f, 0f));
			}
			if (this.mScroll != null)
			{
				this.mScroll.ResetPosition();
			}
		}

		public void WrapContent()
		{
			float num = (float)(this.itemSize * this.mChildren.size) * 0.5f;
			Vector3[] worldCorners = this.mPanel.worldCorners;
			for (int i = 0; i < 4; i++)
			{
				Vector3 vector = worldCorners[i];
				vector = this.mTrans.InverseTransformPoint(vector);
				worldCorners[i] = vector;
			}
			Vector3 vector2 = Vector3.Lerp(worldCorners[0], worldCorners[2], 0.5f);
			if (this.mHorizontal)
			{
				float num2 = worldCorners[0].x - (float)this.itemSize;
				float num3 = worldCorners[2].x + (float)this.itemSize;
				for (int j = 0; j < this.mChildren.size; j++)
				{
					Transform transform = this.mChildren[j];
					float num4 = transform.localPosition.x - vector2.x;
					if (num4 < -num)
					{
						if (this.OpenLog)
						{
						}
						this.UpdateItem(false, transform, j);
						num4 = transform.localPosition.x - vector2.x;
					}
					else if (num4 > num)
					{
						if (this.OpenLog)
						{
						}
						this.UpdateItem(true, transform, j);
						num4 = transform.localPosition.x - vector2.x;
					}
					if (this.cullContent)
					{
						num4 += this.mPanel.clipOffset.x - this.mTrans.localPosition.x;
						if (!UICamera.IsPressed(transform.gameObject))
						{
							NGUITools.SetActive(transform.gameObject, num4 > num2 && num4 < num3, false);
						}
					}
				}
			}
			else
			{
				float num5 = worldCorners[0].y - (float)this.itemSize;
				float num6 = worldCorners[2].y + (float)this.itemSize;
				for (int k = 0; k < this.mChildren.size; k++)
				{
					Transform transform2 = this.mChildren[k];
					float num7 = transform2.localPosition.y - vector2.y;
					if (num7 < -num)
					{
						if (this.OpenLog)
						{
						}
						this.UpdateItem(true, transform2, k);
						num7 = transform2.localPosition.y - vector2.y;
					}
					else if (num7 > num)
					{
						if (this.OpenLog)
						{
						}
						this.UpdateItem(false, transform2, k);
						num7 = transform2.localPosition.y - vector2.y;
					}
					if (this.cullContent)
					{
						num7 += this.mPanel.clipOffset.y - this.mTrans.localPosition.y;
						if (!UICamera.IsPressed(transform2.gameObject))
						{
							NGUITools.SetActive(transform2.gameObject, num7 > num5 && num7 < num6, false);
						}
					}
				}
			}
		}

		protected void UpdateItem(bool firstVislable, Transform item, int activeIndex)
		{
			int num;
			int num2;
			int num3;
			if (firstVislable)
			{
				num = activeIndex;
				num2 = 0;
				num3 = 1;
			}
			else
			{
				num = activeIndex;
				num2 = this.mChildren.size - 1;
				num3 = -1;
			}
			if (this.OpenLog)
			{
			}
			int num4 = this.mChildrenDataIndex[num];
			int num5 = this.mChildrenDataIndex[num2];
			if (this.OpenLog)
			{
			}
			if ((firstVislable && num5 <= this.m_startIndex) || (!firstVislable && num5 >= this.m_MaxCount - 1))
			{
				if (this.OpenLog)
				{
				}
				return;
			}
			int dataIndex = (num4 <= num5) ? (num5 + 1) : (num5 - 1);
			if (this.OpenLog)
			{
			}
			this.m_dataIndex = dataIndex;
			if (this.mHorizontal)
			{
				if (num3 > 0)
				{
					item.localPosition = this.mChildren[num2].localPosition - new Vector3((float)this.itemSize, 0f, 0f);
				}
				else if (num3 < 0)
				{
					item.localPosition = this.mChildren[num2].localPosition + new Vector3((float)this.itemSize, 0f, 0f);
				}
			}
			else if (num3 > 0)
			{
				item.localPosition = this.mChildren[num2].localPosition + new Vector3(0f, (float)this.itemSize, 0f);
			}
			else if (num3 < 0)
			{
				item.localPosition = this.mChildren[num2].localPosition - new Vector3(0f, (float)this.itemSize, 0f);
			}
			Transform item2 = this.mChildren[num];
			this.mChildren.RemoveAt(num);
			this.mChildren.Insert(num2, item2);
			if (this.OpenLog)
			{
			}
			this.mChildrenDataIndex.RemoveAt(num);
			this.mChildrenDataIndex.Insert(num2, this.m_dataIndex);
			if (this.m_pItemChangeCallBack != null && this.checkDataIndex(this.m_dataIndex))
			{
				this.m_pItemChangeCallBack(item, this.m_dataIndex);
			}
		}

		protected void ClickItem(Transform item, int index)
		{
			if (this.m_pOnClickItemCallBack != null)
			{
				this.m_pOnClickItemCallBack(item, index);
			}
		}

		public void InitListView(int _dataCount, int _itemCount, int _seekIndex = 0)
		{
			if (_dataCount < 0 || _itemCount < 0 || _itemCount > _dataCount)
			{
				if (this.OpenLog)
				{
				}
				return;
			}
			if (this.OpenLog)
			{
			}
			this.m_dataIndex = -1;
			this.itemCount = _itemCount;
			this.dataCount = _dataCount;
			this.m_MaxCount = this.dataCount;
			if (_seekIndex > 0)
			{
				this.Seek(_seekIndex);
			}
			else
			{
				this.Seek(0);
			}
		}

		public int GetItemDataIndex(int targetIndex)
		{
			if (targetIndex >= 0 && targetIndex < this.mChildrenDataIndex.size)
			{
				return this.mChildrenDataIndex[targetIndex];
			}
			return -1;
		}

		public void SetDelegate(UIListView.OnItemChange _onItemChange, UIListView.OnClickItem _onClickItem)
		{
			this.m_pItemChangeCallBack = _onItemChange;
			if (_onClickItem != null)
			{
				this.m_pOnClickItemCallBack = _onClickItem;
			}
		}

		public void Seek(int seekIndex)
		{
			if (seekIndex < 0 || seekIndex > this.m_MaxCount - 1)
			{
				if (this.OpenLog)
				{
				}
				return;
			}
			this.mAuto = false;
			this.m_seekIndex = seekIndex;
			if (this.OpenLog)
			{
			}
			this.SortBasedOnScrollMovement(this.m_seekIndex);
			this.WrapContent();
			if (this.m_pItemChangeCallBack != null)
			{
				for (int i = 0; i < this.mChildren.size; i++)
				{
					if (this.checkDataIndex(this.mChildrenDataIndex[i]))
					{
						this.m_pItemChangeCallBack(this.mChildren[i], this.mChildrenDataIndex[i]);
					}
				}
			}
		}

		protected bool checkDataIndex(int dataIndex)
		{
			return dataIndex >= 0 && dataIndex <= this.m_MaxCount - 1;
		}
	}
}
