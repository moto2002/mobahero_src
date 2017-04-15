using System;
using UnityEngine;

namespace anysdk
{
	public class TestCrashPlugin : MonoBehaviour
	{
		private AnySDKCrash _instance;

		private void Awake()
		{
		}

		private void Start()
		{
			this._instance = AnySDKCrash.getInstance();
		}

		private void OnDestroy()
		{
		}

		private void OnGUI()
		{
			GUI.color = Color.white;
			GUI.skin.button.fontSize = 30;
			if (GUI.Button(new Rect(5f, 5f, (float)(Screen.width - 10), 70f), "setUserIdentifier"))
			{
				this.setUserIdentifier("AnySDK");
			}
			if (GUI.Button(new Rect(5f, 80f, (float)(Screen.width - 10), 70f), "reportException"))
			{
				this.reportException("error", "test");
			}
			if (GUI.Button(new Rect(5f, 155f, (float)(Screen.width - 10), 70f), "leaveBreadcrumb"))
			{
				this.leaveBreadcrumb("AnySDK");
			}
			if (GUI.Button(new Rect(5f, 225f, (float)(Screen.width - 10), 70f), "return"))
			{
				UnityEngine.Object.Destroy(base.GetComponent("TestCrashPlugin"));
				base.gameObject.AddComponent<Test>();
			}
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Home))
			{
				Application.Quit();
				AnySDK.getInstance().release();
			}
		}

		private void setUserIdentifier(string identifier)
		{
			this._instance.setUserIdentifier(identifier);
		}

		private void reportException(string message, string exception)
		{
			this._instance.reportException(message, exception);
		}

		private void leaveBreadcrumb(string bread)
		{
			this._instance.leaveBreadcrumb(bread);
		}
	}
}
