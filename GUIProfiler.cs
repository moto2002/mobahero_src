using System;
using UnityEngine;

public class GUIProfiler : MonoBehaviour
{
	public Camera uiCamera;

	private int startX = 10;

	private int startY = 400;

	private int offsetX = 100;

	private int offsetY = 30;

	private bool mDebug;

	private bool mUICameraToggled = true;

	private bool mUIPanelToggled = true;

	private bool mBaseEntityRefresh = true;

	private bool mShadows = true;

	private bool mEnableDynamiceObstacle = true;

	private bool mUpdateBloodBar = true;

	private bool mUpdateMinimap = true;

	private void Start()
	{
		this.startY = Screen.height / 2;
	}

	private void OnGUI()
	{
		if (this.mDebug != GUI.Toggle(new Rect((float)this.startX, (float)this.startY, 180f, 50f), this.mDebug, "debug"))
		{
			this.mDebug = !this.mDebug;
		}
		if (this.mDebug)
		{
			if (this.mUICameraToggled != GUI.Toggle(new Rect((float)this.startX, (float)(this.startY + this.offsetY), 180f, 50f), this.mUICameraToggled, "UICamera"))
			{
				this.mUICameraToggled = !this.mUICameraToggled;
				this.uiCamera.enabled = this.mUICameraToggled;
			}
			if (this.mUIPanelToggled != GUI.Toggle(new Rect((float)this.startX, (float)(this.startY + this.offsetY * 2), 180f, 50f), this.mUIPanelToggled, "UIPanel"))
			{
				this.mUIPanelToggled = !this.mUIPanelToggled;
				UIPanel.IsRefresh = this.mUIPanelToggled;
			}
			if (this.mBaseEntityRefresh != GUI.Toggle(new Rect((float)this.startX, (float)(this.startY + this.offsetY * 3), 180f, 50f), this.mBaseEntityRefresh, "GameLogic"))
			{
				this.mBaseEntityRefresh = !this.mBaseEntityRefresh;
				BaseEntity.IsRefresh = this.mBaseEntityRefresh;
			}
			if (this.mShadows != GUI.Toggle(new Rect((float)this.startX, (float)(this.startY + this.offsetY * 4), 180f, 50f), this.mShadows, "Shadows"))
			{
				this.mShadows = !this.mShadows;
				QualitySettings.SetQualityLevel(this.mShadows ? 7 : 6);
			}
		}
	}
}
