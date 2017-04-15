using System;
using UnityEngine;

public class FXMakerLayout : NgLayout
{
	public enum WINDOWID
	{
		NONE,
		TOP_LEFT = 10,
		TOP_CENTER,
		TOP_RIGHT,
		EFFECT_LIST,
		EFFECT_HIERARCHY,
		EFFECT_CONTROLS,
		PANEL_TEST,
		TOOLIP_CURSOR,
		MODAL_MSG,
		MENUPOPUP,
		SPRITEPOPUP,
		POPUP = 100,
		RESOURCE_START = 200,
		HINTRECT = 300
	}

	public enum MODAL_TYPE
	{
		MODAL_NONE,
		MODAL_MSG,
		MODAL_OK,
		MODAL_YESNO,
		MODAL_OKCANCEL
	}

	public enum MODALRETURN_TYPE
	{
		MODALRETURN_SHOW,
		MODALRETURN_OK,
		MODALRETURN_CANCEL
	}

	public const string m_CurrentVersion = "v1.5.0";

	public const int m_nMaxResourceListCount = 100;

	public const int m_nMaxPrefabListCount = 500;

	public const int m_nMaxTextureListCount = 500;

	public const int m_nMaxMaterialListCount = 1000;

	public const float m_fScreenShotEffectZoomRate = 1f;

	public const float m_fScreenShotBackZoomRate = 0.6f;

	public const float m_fScrollButtonAspect = 0.55f;

	public const float m_fReloadPreviewTime = 0.5f;

	public const int m_nThumbCaptureSize = 512;

	public const int m_nThumbImageSize = 128;

	protected static float m_fFixedWindowWidth = -1f;

	protected static float m_fTopMenuHeight = -1f;

	protected static bool m_bLastStateTopMenuMini = false;

	public static bool m_bDevelopState = false;

	public static bool m_bDevelopPrefs = false;

	public static Rect m_rectOuterMargin = new Rect(2f, 2f, 0f, 0f);

	public static Rect m_rectInnerMargin = new Rect(7f, 19f, 7f, 4f);

	public static int m_nSidewindowWidthCount = 2;

	public static float m_fButtonMargin = 3f;

	public static float m_fScrollButtonHeight = 70f;

	public static bool m_bMinimizeTopMenu = false;

	public static bool m_bMinimizeAll = false;

	public static float m_fMinimizeClickWidth = 60f;

	public static float m_fMinimizeClickHeight = 20f;

	public static float m_fOriActionToolbarHeight = 126f;

	public static float m_fActionToolbarHeight = FXMakerLayout.m_fOriActionToolbarHeight;

	public static float m_MinimizeHeight = 43f;

	public static float m_fToolMessageHeight = 50f;

	public static float m_fTooltipHeight = 60f;

	public static float m_fModalMessageWidth = 500f;

	public static float m_fModalMessageHeight = 200f;

	public static float m_fTestPanelWidth = 150f;

	public static float m_fTestPanelHeight = 120f;

	public static float m_fOriTestPanelHeight = FXMakerLayout.m_fTestPanelHeight;

	public static Color m_ColorButtonHover = new Color(0.7f, 1f, 0.9f, 1f);

	public static Color m_ColorButtonActive = new Color(1f, 1f, 0.6f, 1f);

	public static Color m_ColorButtonMatNormal = new Color(0.5f, 0.7f, 0.7f, 1f);

	public static Color m_ColorButtonUnityEngine = new Color(0.7f, 0.7f, 0.7f, 1f);

	public static Color m_ColorDropFocused = new Color(0.2f, 1f, 0.6f, 0.8f);

	public static Color m_ColorHelpBox = new Color(1f, 0.1f, 0.1f, 1f);

	protected static float m_fArrowIntervalStartTime = 0.2f;

	protected static float m_fArrowIntervalRepeatTime = 0.1f;

	protected static float m_fKeyLastTime;

	public static float GetFixedWindowWidth()
	{
		return 115f;
	}

	public static float GetTopMenuHeight()
	{
		return (!FXMakerLayout.m_bMinimizeAll && !FXMakerLayout.m_bMinimizeTopMenu) ? 92f : FXMakerLayout.m_MinimizeHeight;
	}

	public static int GetWindowId(FXMakerLayout.WINDOWID id)
	{
		return (int)id;
	}

	public static Rect GetChildTopRect(Rect rectParent, int topMargin, int nHeight)
	{
		return new Rect(FXMakerLayout.m_rectInnerMargin.x, (float)topMargin + FXMakerLayout.m_rectInnerMargin.y, rectParent.width - FXMakerLayout.m_rectInnerMargin.x - FXMakerLayout.m_rectInnerMargin.width, (float)nHeight);
	}

	public static Rect GetChildBottomRect(Rect rectParent, int nHeight)
	{
		return new Rect(FXMakerLayout.m_rectInnerMargin.x, rectParent.height - (float)nHeight - FXMakerLayout.m_rectInnerMargin.height, rectParent.width - FXMakerLayout.m_rectInnerMargin.x - FXMakerLayout.m_rectInnerMargin.width, (float)nHeight);
	}

	public static Rect GetChildVerticalRect(Rect rectParent, int topMargin, int count, int pos, int sumCount)
	{
		return new Rect(FXMakerLayout.m_rectInnerMargin.x, (float)topMargin + FXMakerLayout.m_rectInnerMargin.y + (rectParent.height - (float)topMargin - FXMakerLayout.m_rectInnerMargin.y - FXMakerLayout.m_rectInnerMargin.height) / (float)count * (float)pos, rectParent.width - FXMakerLayout.m_rectInnerMargin.x - FXMakerLayout.m_rectInnerMargin.width, (rectParent.height - (float)topMargin - FXMakerLayout.m_rectInnerMargin.y - FXMakerLayout.m_rectInnerMargin.height) / (float)count * (float)sumCount - FXMakerLayout.m_fButtonMargin);
	}

	public static Rect GetInnerVerticalRect(Rect rectBase, int count, int pos, int sumCount)
	{
		return new Rect(rectBase.x, rectBase.y + (rectBase.height + FXMakerLayout.m_fButtonMargin) / (float)count * (float)pos, rectBase.width, (rectBase.height + FXMakerLayout.m_fButtonMargin) / (float)count * (float)sumCount - FXMakerLayout.m_fButtonMargin);
	}

	public static Rect GetChildHorizontalRect(Rect rectParent, int topMargin, int count, int pos, int sumCount)
	{
		return new Rect(FXMakerLayout.m_rectInnerMargin.x + (rectParent.width - FXMakerLayout.m_rectInnerMargin.x - FXMakerLayout.m_rectInnerMargin.width) / (float)count * (float)pos, (float)topMargin + FXMakerLayout.m_rectInnerMargin.y, (rectParent.width - FXMakerLayout.m_rectInnerMargin.x - FXMakerLayout.m_rectInnerMargin.width) / (float)count * (float)sumCount - FXMakerLayout.m_fButtonMargin, rectParent.height - FXMakerLayout.m_rectInnerMargin.y - FXMakerLayout.m_rectInnerMargin.height);
	}

	public static Rect GetInnerHorizontalRect(Rect rectBase, int count, int pos, int sumCount)
	{
		return new Rect(rectBase.x + (rectBase.width + FXMakerLayout.m_fButtonMargin) / (float)count * (float)pos, rectBase.y, (rectBase.width + FXMakerLayout.m_fButtonMargin) / (float)count * (float)sumCount - FXMakerLayout.m_fButtonMargin, rectBase.height);
	}

	public static Rect GetMenuChangeRect()
	{
		return new Rect(FXMakerLayout.m_rectOuterMargin.x, FXMakerLayout.m_rectOuterMargin.y, FXMakerLayout.GetFixedWindowWidth(), FXMakerLayout.GetTopMenuHeight());
	}

	public static Rect GetMenuToolbarRect()
	{
		return new Rect(FXMakerLayout.GetMenuChangeRect().xMax + FXMakerLayout.m_rectOuterMargin.x, FXMakerLayout.m_rectOuterMargin.y, (float)Screen.width - FXMakerLayout.GetMenuChangeRect().width - FXMakerLayout.GetMenuTopRightRect().width - FXMakerLayout.m_rectOuterMargin.x * 4f, FXMakerLayout.GetTopMenuHeight());
	}

	public static Rect GetMenuTopRightRect()
	{
		return new Rect((float)Screen.width - FXMakerLayout.GetFixedWindowWidth() - FXMakerLayout.m_rectOuterMargin.x, FXMakerLayout.m_rectOuterMargin.y, FXMakerLayout.GetFixedWindowWidth(), FXMakerLayout.GetTopMenuHeight());
	}

	public static Rect GetResListRect(int nIndex)
	{
		return new Rect(FXMakerLayout.m_rectOuterMargin.x + (FXMakerLayout.GetFixedWindowWidth() + FXMakerLayout.m_rectOuterMargin.x) * (float)nIndex, FXMakerLayout.GetMenuChangeRect().yMax + FXMakerLayout.m_rectOuterMargin.y, FXMakerLayout.GetFixedWindowWidth(), (float)Screen.height - FXMakerLayout.GetMenuChangeRect().yMax - FXMakerLayout.m_rectOuterMargin.y * 2f);
	}

	public static Rect GetEffectListRect()
	{
		return new Rect(FXMakerLayout.m_rectOuterMargin.x, FXMakerLayout.GetMenuChangeRect().yMax + FXMakerLayout.m_rectOuterMargin.y, FXMakerLayout.GetFixedWindowWidth() * (float)FXMakerLayout.m_nSidewindowWidthCount + FXMakerLayout.m_rectOuterMargin.x, (float)Screen.height - FXMakerLayout.GetMenuChangeRect().yMax - FXMakerLayout.m_rectOuterMargin.y * 2f);
	}

	public static Rect GetEffectHierarchyRect()
	{
		return new Rect((float)Screen.width - (FXMakerLayout.GetFixedWindowWidth() + FXMakerLayout.m_rectOuterMargin.x) * (float)FXMakerLayout.m_nSidewindowWidthCount, FXMakerLayout.GetMenuChangeRect().yMax + FXMakerLayout.m_rectOuterMargin.y, FXMakerLayout.GetFixedWindowWidth() * (float)FXMakerLayout.m_nSidewindowWidthCount + FXMakerLayout.m_rectOuterMargin.x, (float)Screen.height - FXMakerLayout.GetMenuChangeRect().yMax - FXMakerLayout.m_rectOuterMargin.y * 2f);
	}

	public static Rect GetActionToolbarRect()
	{
		return new Rect(FXMakerLayout.m_rectOuterMargin.x * 3f + FXMakerLayout.GetFixedWindowWidth() * (float)FXMakerLayout.m_nSidewindowWidthCount, (float)Screen.height - FXMakerLayout.m_fActionToolbarHeight - FXMakerLayout.m_rectOuterMargin.y, (float)Screen.width - FXMakerLayout.GetMenuChangeRect().width * 4f - FXMakerLayout.m_rectOuterMargin.x * 6f, FXMakerLayout.m_fActionToolbarHeight);
	}

	public static Rect GetToolMessageRect()
	{
		return new Rect(FXMakerLayout.GetFixedWindowWidth() * 2.1f, (float)Screen.height - FXMakerLayout.m_fActionToolbarHeight - FXMakerLayout.m_rectOuterMargin.y - FXMakerLayout.m_fToolMessageHeight - FXMakerLayout.m_fTooltipHeight, (float)Screen.width - FXMakerLayout.GetFixedWindowWidth() * (float)FXMakerLayout.m_nSidewindowWidthCount * 2f - FXMakerLayout.m_rectOuterMargin.x * 2f - FXMakerLayout.m_fTestPanelWidth, FXMakerLayout.m_fToolMessageHeight);
	}

	public static Rect GetTooltipRect()
	{
		return new Rect(FXMakerLayout.m_rectOuterMargin.x * 3f + FXMakerLayout.GetFixedWindowWidth() * (float)FXMakerLayout.m_nSidewindowWidthCount, (float)Screen.height - FXMakerLayout.m_fActionToolbarHeight - FXMakerLayout.m_rectOuterMargin.y - FXMakerLayout.m_fTooltipHeight, (float)Screen.width - FXMakerLayout.GetMenuChangeRect().width * 4f - FXMakerLayout.m_rectOuterMargin.x * 6f - FXMakerLayout.m_fTestPanelWidth, FXMakerLayout.m_fTooltipHeight);
	}

	public static Rect GetCursorTooltipRect(Vector2 size)
	{
		return NgLayout.ClampWindow(new Rect(Input.mousePosition.x + 15f, (float)Screen.height - Input.mousePosition.y + 80f, size.x, size.y));
	}

	public static Rect GetModalMessageRect()
	{
		return new Rect(((float)Screen.width - FXMakerLayout.m_fModalMessageWidth) / 2f, ((float)Screen.height - FXMakerLayout.m_fModalMessageHeight - FXMakerLayout.m_fModalMessageHeight / 8f) / 2f, FXMakerLayout.m_fModalMessageWidth, FXMakerLayout.m_fModalMessageHeight);
	}

	public static Rect GetMenuGizmoRect()
	{
		return new Rect(FXMakerLayout.m_rectOuterMargin.x * 3f + FXMakerLayout.GetFixedWindowWidth() * (float)FXMakerLayout.m_nSidewindowWidthCount, FXMakerLayout.GetTopMenuHeight() + FXMakerLayout.m_rectOuterMargin.y, 490f, 26f);
	}

	public static Rect GetMenuTestPanelRect()
	{
		return new Rect((float)Screen.width - FXMakerLayout.GetFixedWindowWidth() * 2f - FXMakerLayout.m_fTestPanelWidth - FXMakerLayout.m_rectOuterMargin.x * 2f, (float)Screen.height - FXMakerLayout.m_fActionToolbarHeight - FXMakerLayout.m_rectOuterMargin.y - FXMakerLayout.m_fTestPanelHeight, FXMakerLayout.m_fTestPanelWidth, FXMakerLayout.m_fTestPanelHeight);
	}

	public static Rect GetClientRect()
	{
		return new Rect(FXMakerLayout.m_rectOuterMargin.x * 3f + FXMakerLayout.GetFixedWindowWidth() * (float)FXMakerLayout.m_nSidewindowWidthCount, FXMakerLayout.GetTopMenuHeight() + FXMakerLayout.m_rectOuterMargin.y, (float)Screen.width - (FXMakerLayout.m_rectOuterMargin.x * 3f + FXMakerLayout.GetFixedWindowWidth() * (float)FXMakerLayout.m_nSidewindowWidthCount) * 2f, (float)Screen.height - FXMakerLayout.m_fActionToolbarHeight - FXMakerLayout.m_rectOuterMargin.y * 3f - FXMakerLayout.GetTopMenuHeight());
	}

	public static Rect GetScrollViewRect(int nWidth, int nButtonCount, int nColumn)
	{
		return new Rect(0f, 0f, (float)(nWidth - 2), FXMakerLayout.m_fScrollButtonHeight * (float)(nButtonCount / nColumn + ((0 >= nButtonCount % nColumn) ? 0 : 1)) + 25f);
	}

	public static Rect GetScrollGridRect(int nWidth, int nButtonCount, int nColumn)
	{
		return new Rect(0f, 0f, (float)(nWidth - 2), FXMakerLayout.m_fScrollButtonHeight * (float)(nButtonCount / nColumn + ((0 >= nButtonCount % nColumn) ? 0 : 1)));
	}

	public static Rect GetAspectScrollViewRect(int nWidth, float fAspect, int nButtonCount, int nColumn, bool bImageOnly)
	{
		return new Rect(0f, 0f, (float)(nWidth - 4), ((float)((nWidth - 4) / nColumn) * fAspect + (float)((!bImageOnly) ? 10 : 0)) * (float)(nButtonCount / nColumn + ((0 >= nButtonCount % nColumn) ? 0 : 1)) + 25f);
	}

	public static Rect GetAspectScrollGridRect(int nWidth, float fAspect, int nButtonCount, int nColumn, bool bImageOnly)
	{
		return new Rect(0f, 0f, (float)(nWidth - 4), ((float)((nWidth - 4) / nColumn) * fAspect + (float)((!bImageOnly) ? 10 : 0)) * (float)(nButtonCount / nColumn + ((0 >= nButtonCount % nColumn) ? 0 : 1)));
	}

	public static KeyCode GetVaildInputKey(KeyCode key, bool bPress)
	{
		if (bPress || FXMakerLayout.m_fKeyLastTime + FXMakerLayout.m_fArrowIntervalRepeatTime * Time.timeScale < Time.time)
		{
			FXMakerLayout.m_fKeyLastTime = ((!bPress) ? Time.time : (Time.time + FXMakerLayout.m_fArrowIntervalStartTime));
			return key;
		}
		return KeyCode.None;
	}

	public static int GetGridHoverIndex(Rect windowRect, Rect listRect, Rect gridRect, int nCount, int nColumn, GUIStyle style)
	{
		int num = (style != null) ? style.margin.left : 0;
		int num2 = nCount / nColumn + ((0 >= nCount % nColumn) ? 0 : 1);
		float num3 = gridRect.width / (float)nColumn;
		float num4 = gridRect.height / (float)num2;
		Vector2 point = NgLayout.GetGUIMousePosition() - new Vector2(windowRect.x, windowRect.y);
		if (!listRect.Contains(point))
		{
			return -1;
		}
		for (int i = 0; i < nCount; i++)
		{
			Rect rect = new Rect(listRect.x + num3 * (float)(i % nColumn) + (float)num, listRect.y + num4 * (float)(i / nColumn) + (float)num, num3 - (float)(num * 2), num4 - (float)(num * 2));
			if (rect.Contains(point))
			{
				return i;
			}
		}
		return -1;
	}

	public static int TooltipToolbar(Rect windowRect, Rect gridRect, int nGridIndex, GUIContent[] cons)
	{
		return FXMakerLayout.TooltipToolbar(windowRect, gridRect, nGridIndex, cons, null);
	}

	public static int TooltipToolbar(Rect windowRect, Rect gridRect, int nGridIndex, GUIContent[] cons, GUIStyle style)
	{
		int result = GUI.Toolbar(gridRect, nGridIndex, cons, style);
		int gridHoverIndex = FXMakerLayout.GetGridHoverIndex(windowRect, gridRect, gridRect, cons.Length, cons.Length, null);
		if (0 <= gridHoverIndex)
		{
			GUI.tooltip = cons[gridHoverIndex].tooltip;
		}
		return result;
	}

	public static int TooltipSelectionGrid(Rect windowRect, Rect listRect, int nGridIndex, GUIContent[] cons, int nColumCount)
	{
		return FXMakerLayout.TooltipSelectionGrid(windowRect, listRect, listRect, nGridIndex, cons, nColumCount, null);
	}

	public static int TooltipSelectionGrid(Rect windowRect, Rect listRect, int nGridIndex, GUIContent[] cons, int nColumCount, GUIStyle style)
	{
		return FXMakerLayout.TooltipSelectionGrid(windowRect, listRect, listRect, nGridIndex, cons, nColumCount, null);
	}

	public static int TooltipSelectionGrid(Rect windowRect, Rect listRect, Rect gridRect, int nGridIndex, GUIContent[] cons, int nColumCount)
	{
		return FXMakerLayout.TooltipSelectionGrid(windowRect, listRect, gridRect, nGridIndex, cons, nColumCount, null);
	}

	public static int TooltipSelectionGrid(Rect windowRect, Rect listRect, Rect gridRect, int nGridIndex, GUIContent[] cons, int nColumCount, GUIStyle style)
	{
		int result = GUI.SelectionGrid(gridRect, nGridIndex, cons, nColumCount, style);
		int gridHoverIndex = FXMakerLayout.GetGridHoverIndex(windowRect, listRect, gridRect, cons.Length, nColumCount, null);
		if (0 <= gridHoverIndex)
		{
			GUI.tooltip = cons[gridHoverIndex].tooltip;
		}
		return result;
	}

	public static void ModalWindow(Rect rect, GUI.WindowFunction func, string title)
	{
		GUI.Window(GUIUtility.GetControlID(FocusType.Passive), rect, delegate(int id)
		{
			GUI.depth = 0;
			int controlID = GUIUtility.GetControlID(FocusType.Native);
			if (GUIUtility.hotControl < controlID)
			{
				FXMakerLayout.setHotControl(0);
			}
			func(id);
			int controlID2 = GUIUtility.GetControlID(FocusType.Native);
			if (GUIUtility.hotControl < controlID || (GUIUtility.hotControl > controlID2 && controlID2 != -1))
			{
				FXMakerLayout.setHotControl(-1);
			}
			GUI.FocusWindow(id);
			GUI.BringWindowToFront(id);
		}, title);
	}

	private static void setHotControl(int id)
	{
		Rect rect = new Rect(0f, 0f, (float)Screen.width, (float)Screen.height);
		if (rect.Contains(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)))
		{
			GUIUtility.hotControl = id;
		}
	}
}
