using System;
using UnityEngine;

namespace Com.Game.Module
{
	[AddComponentMenu("NGUI/UI/ViewTree")]
	public class ViewTree : MonoBehaviour
	{
		public static GameObject go;

		public static GameObject home;

		public static ViewTree Instance;

		public static GameObject slashEffect1;

		public static GameObject slashEffect2;

		public static MyAnchorCamera anchorCamera;

		public int battleType;

		public bool isUpdate;

		public Transform cameraTrans;

		private void Awake()
		{
			ViewTree.Instance = this;
			ViewTree.go = base.gameObject;
			this.isUpdate = false;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			ViewTree.SetRoot();
			this.UpdateAnchorCamera();
		}

		private void OnDestroy()
		{
		}

		public static void SetRoot()
		{
			if (!object.ReferenceEquals(ViewTree.go, null))
			{
				ViewTree.home = ViewTree.go.transform.FindChild("Camera/Home").gameObject;
				ViewTree.slashEffect1 = ViewTree.go.transform.FindChild("UIEffectCamera/Slash1").gameObject;
				ViewTree.slashEffect2 = ViewTree.go.transform.FindChild("UIEffectCamera/Slash2").gameObject;
			}
		}

		public void UpdateAnchorCamera()
		{
			ViewTree.anchorCamera = UICamera.mainCamera.gameObject.GetComponent<MyAnchorCamera>();
			if (ViewTree.anchorCamera == null)
			{
				ViewTree.anchorCamera = UICamera.mainCamera.gameObject.AddComponent<MyAnchorCamera>();
				ViewTree.anchorCamera.Model = MyAnchorCamera.AnchorModel.Width;
				ViewTree.anchorCamera.suitableUI_width = 1920f;
				ViewTree.anchorCamera.suitableUI_height = 1080f;
				ViewTree.anchorCamera.isNGUIHierarchy = true;
			}
			ViewTree.anchorCamera.UpdateCameraMatrix();
		}
	}
}
