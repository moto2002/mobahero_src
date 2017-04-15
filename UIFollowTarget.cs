using System;
using UnityEngine;

[AddComponentMenu("NGUI/Examples/Follow Target")]
public class UIFollowTarget : MonoBehaviour
{
	public Transform target;

	public Camera gameCamera;

	public Camera uiCamera;

	public bool disableIfInvisible;

	private Transform mTrans;

	private bool mIsVisible;

	public float real_margin_x;

	public float real_margin_y;

	public float real_margin_z;

	private Vector3 margin;

	private Transform gameCameraTransform;

	private Vector3 lastFrameGameCameraPosition;

	private Transform targetTransform;

	private Vector3 lastFrameTargetPosition;

	private Vector3 lastFrameScreenPosition;

	private int visibleMarginX = 10;

	private int visibleMarginY = 10;

	private void Awake()
	{
		this.mTrans = base.transform;
		this.margin = new Vector3(this.real_margin_x, this.real_margin_y, this.real_margin_z);
	}

	private void Start()
	{
		if (this.target != null)
		{
			if (this.gameCamera == null)
			{
				this.gameCamera = Camera.main;
			}
			if (this.uiCamera == null)
			{
				this.uiCamera = NGUITools.FindCameraForLayer(base.gameObject.layer);
			}
			this.gameCameraTransform = this.gameCamera.transform;
			this.lastFrameTargetPosition = this.target.transform.position;
			this.targetTransform = this.target.transform;
			this.lastFrameGameCameraPosition = this.gameCameraTransform.position;
		}
		else
		{
			Debug.LogError("Expected to have 'target' set to a valid transform", this);
			base.enabled = false;
		}
	}

	private void SetVisible(bool val)
	{
		if (this.mIsVisible != val)
		{
			this.mIsVisible = val;
			NGUITools.SetActive(this.mTrans.gameObject, this.mIsVisible);
		}
	}

	private void LateUpdate()
	{
		if (this.targetTransform == null)
		{
			return;
		}
		if (this.target == null)
		{
			this.SetVisible(false);
			return;
		}
		if (this.targetTransform.position == this.lastFrameTargetPosition && this.gameCameraTransform.position == this.lastFrameGameCameraPosition)
		{
			return;
		}
		this.lastFrameTargetPosition = this.targetTransform.position;
		this.lastFrameGameCameraPosition = this.gameCameraTransform.position;
		Vector3 vector = this.gameCamera.WorldToScreenPoint(this.target.position + this.margin);
		if (vector == this.lastFrameGameCameraPosition)
		{
			return;
		}
		this.lastFrameGameCameraPosition = vector;
		if (this.isVisible(vector))
		{
			this.SetVisible(true);
			float d = 1f;
			if (Gray.instance != null && Gray.instance.enabled && Gray.instance.doGray)
			{
				d = (float)Gray.rtScal;
			}
			Vector3 vector2 = this.uiCamera.ScreenToWorldPoint(vector * d);
			this.mTrans.position = new Vector3(vector2.x, vector2.y, 0f);
		}
		else
		{
			this.SetVisible(false);
		}
	}

	private bool isVisible(Vector3 screen_position)
	{
		return true;
	}

	protected virtual void OnUpdate(bool isVisible)
	{
	}
}
