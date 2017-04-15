using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace Com.Game.Module
{
	public class LoginDwonLoadTexture : MonoBehaviour
	{
		public UITexture uiTex;

		public string url = string.Empty;

		public bool pixelPerfect = true;

		[HideInInspector]
		public bool isFromLocal;

		[HideInInspector]
		public string texName;

		private Texture2D mTex;

		private void Awake()
		{
			this.mTex = new Texture2D(1050, 453, TextureFormat.RGB24, false);
			this.mTex.name = "LoginDownloadTexture";
		}

		[DebuggerHidden]
		private IEnumerator Start()
		{
			LoginDwonLoadTexture.<Start>c__Iterator162 <Start>c__Iterator = new LoginDwonLoadTexture.<Start>c__Iterator162();
			<Start>c__Iterator.<>f__this = this;
			return <Start>c__Iterator;
		}

		private void SetTexture()
		{
			if (this.uiTex != null)
			{
				this.uiTex.mainTexture = this.mTex;
			}
		}

		private void OnDestroy()
		{
			if (this.mTex != null)
			{
				UnityEngine.Object.Destroy(this.mTex);
			}
		}
	}
}
