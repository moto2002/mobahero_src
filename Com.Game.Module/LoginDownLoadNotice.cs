using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace Com.Game.Module
{
	public class LoginDownLoadNotice : MonoBehaviour
	{
		[SerializeField]
		private UILabel content;

		[SerializeField]
		private bool pixelPerfect = true;

		[HideInInspector]
		public bool isFromLocal;

		[HideInInspector]
		public string texName;

		[HideInInspector]
		public string url = string.Empty;

		private string notice;

		private void Awake()
		{
			this.content.text = string.Empty;
		}

		[DebuggerHidden]
		private IEnumerator Start()
		{
			LoginDownLoadNotice.<Start>c__Iterator161 <Start>c__Iterator = new LoginDownLoadNotice.<Start>c__Iterator161();
			<Start>c__Iterator.<>f__this = this;
			return <Start>c__Iterator;
		}

		private void SetNotice()
		{
			this.content.text = this.notice;
			UIScrollView component = this.content.transform.parent.GetComponent<UIScrollView>();
			Bounds bounds = this.content.CalculateBounds();
			BoxCollider component2 = this.content.GetComponent<BoxCollider>();
			if (null != component2)
			{
				component2.size = bounds.size;
				component2.center = new Vector3(0f, -component2.size.y / 2f, 0f);
			}
			component.ResetPosition();
		}

		private void OnDestroy()
		{
		}
	}
}
