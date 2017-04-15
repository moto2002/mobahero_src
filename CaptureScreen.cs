using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class CaptureScreen
{
	public static string CapturePath = Application.streamingAssetsPath + "/Xml/Persistence/Save/";

	public static CoroutineManager corMrg = null;

	public static void CaptureScreenshot()
	{
	}

	[DebuggerHidden]
	private static IEnumerator StartCaptureScreenshot(bool isSaveUI)
	{
		CaptureScreen.<StartCaptureScreenshot>c__Iterator1D1 <StartCaptureScreenshot>c__Iterator1D = new CaptureScreen.<StartCaptureScreenshot>c__Iterator1D1();
		<StartCaptureScreenshot>c__Iterator1D.isSaveUI = isSaveUI;
		<StartCaptureScreenshot>c__Iterator1D.<$>isSaveUI = isSaveUI;
		return <StartCaptureScreenshot>c__Iterator1D;
	}
}
