using System;
using UnityEngine;

public class UniWebViewFacade : MonoBehaviour
{
	public static UniWebViewFacade Instance;

	private void Awake()
	{
		UniWebViewFacade.Instance = this;
	}

	private void Start()
	{
	}

	public void OpenUrl(string url)
	{
		GameObject gameObject = GameObject.Find("WebView");
		if (gameObject == null)
		{
			gameObject = new GameObject("WebView");
		}
		UniWebView uniWebView = gameObject.AddComponent<UniWebView>();
		uniWebView.OnLoadComplete += new UniWebView.LoadCompleteDelegate(this.OnLoadComplete);
		uniWebView.InsetsForScreenOreitation += new UniWebView.InsetsForScreenOreitationDelegate(this.InsetsForScreenOreitation);
		uniWebView.toolBarShow = true;
		uniWebView.url = url;
		uniWebView.Load();
		uniWebView.Show(false, UniWebViewTransitionEdge.None, 0.4f, null);
	}

	private void OnLoadComplete(UniWebView webView, bool success, string errorMessage)
	{
		if (!success)
		{
			Debug.Log("Something wrong in webview loading: " + errorMessage);
		}
	}

	private UniWebViewEdgeInsets InsetsForScreenOreitation(UniWebView webView, UniWebViewOrientation orientation)
	{
		if (orientation == UniWebViewOrientation.Portrait)
		{
			return new UniWebViewEdgeInsets(5, 5, 5, 5);
		}
		return new UniWebViewEdgeInsets(5, 5, 5, 5);
	}
}
