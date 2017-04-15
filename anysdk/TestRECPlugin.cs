using System;
using System.Collections.Generic;
using UnityEngine;

namespace anysdk
{
	public class TestRECPlugin : MonoBehaviour
	{
		private AnySDKREC _instance;

		private void Awake()
		{
		}

		private void Start()
		{
			this._instance = AnySDKREC.getInstance();
			this._instance.setListener(this, "RECExternalCall");
		}

		private void RECExternalCall(string msg)
		{
			Debug.Log("RECExternalCall(" + msg + ")");
			Dictionary<string, string> dictionary = AnySDKUtil.stringToDictionary(msg);
			int num = Convert.ToInt32(dictionary["code"]);
			string text = dictionary["msg"];
			switch (num)
			{
			case 0:
				Debug.Log("kRECInitSuccess\n");
				break;
			case 1:
				Debug.Log("kRECInitFail\n");
				break;
			case 2:
				Debug.Log("kRECStartRecording \n");
				break;
			case 3:
				Debug.Log("kRECStopRecording \n");
				break;
			case 4:
				Debug.Log("kRECPauseRecording \n");
				break;
			case 5:
				Debug.Log("kRECResumeRecording \n");
				break;
			case 6:
				Debug.Log("kRECEnterSDKPage \n");
				break;
			case 7:
				Debug.Log("kRECQuitSDKPage \n");
				break;
			case 8:
				Debug.Log("kRECShareSuccess \n");
				break;
			case 9:
				Debug.Log("kRECShareFail \n");
				break;
			}
		}

		private void OnDestroy()
		{
		}

		private void OnGUI()
		{
			GUI.color = Color.white;
			GUI.skin.button.fontSize = 30;
			if (GUI.Button(new Rect(5f, 5f, (float)(Screen.width - 10), 70f), "startRecording"))
			{
				string str = (!this.isAvailable()) ? "is not" : "is";
				Debug.Log("The device" + str + "supported.");
				this.startRecording();
			}
			if (GUI.Button(new Rect(5f, 80f, (float)(Screen.width - 10), 70f), "stopRecording"))
			{
				this.stopRecording();
			}
			if (GUI.Button(new Rect(5f, 155f, (float)(Screen.width - 10), 70f), "share"))
			{
				this.share();
			}
			if (GUI.Button(new Rect(5f, 230f, (float)(Screen.width - 10), 70f), "pauseRecording"))
			{
				string str2 = (!this.isRecording()) ? "is not" : "is";
				Debug.Log("The video" + str2 + "being recorded.");
				this.pauseRecording();
			}
			if (GUI.Button(new Rect(5f, 305f, (float)(Screen.width - 10), 70f), "resumeRecording"))
			{
				this.resumeRecording();
			}
			if (GUI.Button(new Rect(5f, 380f, (float)(Screen.width - 10), 70f), "showToolBar"))
			{
				this.showToolBar();
			}
			if (GUI.Button(new Rect(5f, 455f, (float)(Screen.width - 10), 70f), "hideToolBar"))
			{
				this.hideToolBar();
			}
			if (GUI.Button(new Rect(5f, 525f, (float)(Screen.width - 10), 70f), "showVideoCenter"))
			{
				this.showVideoCenter();
			}
			if (GUI.Button(new Rect(5f, 595f, (float)(Screen.width - 10), 70f), "enterPlatform"))
			{
				this.enterPlatform();
			}
			if (GUI.Button(new Rect(5f, 665f, (float)(Screen.width - 10), 70f), "setMetaData"))
			{
				this.setMetaData();
			}
			if (GUI.Button(new Rect(5f, 735f, (float)(Screen.width - 10), 70f), "return"))
			{
				UnityEngine.Object.Destroy(base.GetComponent("TestRECPlugin"));
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

		private void startRecording()
		{
			this._instance.startRecording();
		}

		private void stopRecording()
		{
			this._instance.stopRecording();
		}

		private void share()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["Video_Title"] = "AnySDK";
			dictionary["Video_Desc"] = "AnySDK是一个神奇的SDK";
			AnySDKREC.getInstance().share(dictionary);
		}

		private void pauseRecording()
		{
			if (this._instance.isFunctionSupported("pauseRecording"))
			{
				this._instance.callFuncWithParam("pauseRecording", null);
			}
		}

		private void resumeRecording()
		{
			if (this._instance.isFunctionSupported("resumeRecording"))
			{
				this._instance.callFuncWithParam("resumeRecording", null);
			}
		}

		private bool isAvailable()
		{
			return this._instance.isFunctionSupported("isAvailable") && this._instance.callBoolFuncWithParam("isAvailable", null);
		}

		private void showToolBar()
		{
			if (this._instance.isFunctionSupported("showToolBar"))
			{
				this._instance.callFuncWithParam("showToolBar", null);
			}
		}

		private void hideToolBar()
		{
			if (this._instance.isFunctionSupported("hideToolBar"))
			{
				this._instance.callFuncWithParam("hideToolBar", null);
			}
		}

		private bool isRecording()
		{
			return this._instance.isFunctionSupported("isRecording") && this._instance.callBoolFuncWithParam("isRecording", null);
		}

		private void showVideoCenter()
		{
			if (this._instance.isFunctionSupported("showVideoCenter"))
			{
				this._instance.callFuncWithParam("showVideoCenter", null);
			}
		}

		private void enterPlatform()
		{
			if (this._instance.isFunctionSupported("enterPlatform"))
			{
				this._instance.callFuncWithParam("enterPlatform", null);
			}
		}

		private void setMetaData()
		{
			if (this._instance.isFunctionSupported("setMetaData"))
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary["ext"] = "anysdk";
				AnySDKParam param = new AnySDKParam(dictionary);
				this._instance.callFuncWithParam("setMetaData", param);
			}
		}
	}
}
