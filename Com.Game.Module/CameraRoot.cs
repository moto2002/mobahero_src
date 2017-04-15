using System;
using UnityEngine;

namespace Com.Game.Module
{
	public class CameraRoot : MonoBehaviour
	{
		public Camera m_uiCamera;

		public Camera m_uiEffectCamera;

		public Camera m_battleCamera;

		public Transform m_battleCameraRoot;

		public Light m_Light;

		public Mist3Adapter mist;

		private Camera m_MainCamera;

		[SerializeField]
		private bool isFixedApplicationRate;

		[SerializeField]
		private int FixedApplicationRate = 60;

		private static int targetFPS = 30;

		private static bool setApplicationRate;

		private static bool m_disableOrDestroy;

		private static CameraRoot m_instance;

		public Camera UICamera
		{
			get
			{
				return this.m_uiCamera;
			}
		}

		public static CameraRoot Instance
		{
			get
			{
				if (CameraRoot.m_instance == null && !CameraRoot.m_disableOrDestroy)
				{
					Debug.LogError("CameraRoot script is missing");
				}
				return CameraRoot.m_instance;
			}
		}

		private void OnDisable()
		{
			CameraRoot.m_disableOrDestroy = true;
		}

		private void OnDestroy()
		{
			CameraRoot.m_disableOrDestroy = true;
		}

		private void Awake()
		{
			CameraRoot.m_instance = this;
			this.m_uiEffectCamera.transform.position = this.m_uiCamera.transform.position;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			this.ClipCameras(false);
			this.ClipCameras(true);
			CameraRoot.setApplicationRate = this.isFixedApplicationRate;
			if (CameraRoot.setApplicationRate)
			{
				CameraRoot.targetFPS = this.FixedApplicationRate;
			}
			if (CameraRoot.targetFPS > 0)
			{
				Application.targetFrameRate = CameraRoot.targetFPS;
			}
			int @int = PlayerPrefs.GetInt("QualitySetting", 1);
			if (this.m_Light != null)
			{
				this.m_Light.shadows = ((@int <= 0) ? LightShadows.None : LightShadows.Hard);
			}
		}

		public void SetCamera(SceneType scene)
		{
			if (scene != SceneType.Login && scene != SceneType.Home)
			{
				this.m_battleCameraRoot.gameObject.SetActive(true);
				if (this.mist != null)
				{
					this.mist.CheckMistSettings();
				}
			}
			else
			{
				this.m_battleCameraRoot.gameObject.SetActive(false);
			}
			this.ClipCameras(false);
			this.ClipCameras(true);
		}

		public void ClipCameras(bool clip)
		{
			if (clip)
			{
				CamRatio.SetupCamera(CameraRoot.Instance.m_uiCamera, 0f);
				CamRatio.SetupCamera(CameraRoot.Instance.m_uiEffectCamera, 0f);
				CamRatio.SetupCamera(CameraRoot.Instance.m_battleCamera, 0f);
			}
			else
			{
				CamRatio.RestoreCamera(CameraRoot.Instance.m_uiCamera);
				CamRatio.RestoreCamera(CameraRoot.Instance.m_uiEffectCamera);
				CamRatio.RestoreCamera(CameraRoot.Instance.m_battleCamera);
			}
		}

		public static void SetTargetFPS(int targetfps)
		{
			if (CameraRoot.setApplicationRate)
			{
				if (Application.targetFrameRate != CameraRoot.targetFPS)
				{
					Application.targetFrameRate = CameraRoot.targetFPS;
				}
			}
			else
			{
				CameraRoot.targetFPS = targetfps;
				if (Application.targetFrameRate != CameraRoot.targetFPS)
				{
					Application.targetFrameRate = CameraRoot.targetFPS;
				}
			}
		}
	}
}
