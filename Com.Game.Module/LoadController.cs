using System;
using UnityEngine;

namespace Com.Game.Module
{
	public class LoadController : MonoBehaviour
	{
		[SerializeField]
		private UILabel progressText;

		[SerializeField]
		private UIProgressBar loadSlider;

		[SerializeField]
		private UITexture bgTextrue;

		private void Awake()
		{
		}

		private void OnDestroy()
		{
			this.ClearResources();
		}

		public void ClearResources()
		{
			if (this.bgTextrue != null && this.bgTextrue.mainTexture != null)
			{
				Resources.UnloadAsset(this.bgTextrue.mainTexture);
			}
		}

		public void SetLoadingPercentage(int progress)
		{
			if (progress >= 100)
			{
				progress = 100;
			}
			this.progressText.text = progress + "%";
			this.loadSlider.value = (float)progress / 100f;
		}

		public void SetLoadingBackground(int Style)
		{
			if (this.bgTextrue != null)
			{
				this.bgTextrue.mainTexture = (Resources.Load("Texture/Bg/bg" + Style.ToString()) as Texture2D);
				this.bgTextrue.gameObject.SetActive(true);
			}
		}
	}
}
