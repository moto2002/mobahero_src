using System;
using UnityEngine;

namespace GameLogin.State
{
	internal class VedioController2 : MonoBehaviour, IVedioController
	{
		private const string FormatStr = ".mp4";

		[SerializeField]
		private MediaPlayerCtrl scrMedia;

		private bool bPlay;

		private string vedioName = string.Empty;

		private bool bLoop;

		private bool bStart;

		private PlayState state;

		private int m_iOrgWidth;

		private int m_iOrgHeight;

		private Action<IVedioController> callback_ready;

		private Action<IVedioController> callback_end;

		private Action<IVedioController> callback_start;

		Action<IVedioController> IVedioController.Callback_Ready
		{
			get
			{
				return this.callback_ready;
			}
			set
			{
				this.callback_ready = value;
			}
		}

		Action<IVedioController> IVedioController.Callback_End
		{
			get
			{
				return this.callback_end;
			}
			set
			{
				this.callback_end = value;
			}
		}

		Action<IVedioController> IVedioController.Callback_Start
		{
			get
			{
				return this.callback_start;
			}
			set
			{
				this.callback_start = value;
			}
		}

		public int ID
		{
			get;
			set;
		}

		public bool Enable
		{
			get;
			set;
		}

		public PlayState State
		{
			get
			{
				return this.state;
			}
			private set
			{
				this.state = value;
			}
		}

		public bool Play
		{
			get
			{
				return this.bPlay;
			}
			set
			{
				this.bPlay = value;
				if (this.bPlay)
				{
					this.scrMedia.Play();
				}
				else
				{
					this.scrMedia.Pause();
				}
			}
		}

		public bool Loop
		{
			get
			{
				return this.bLoop;
			}
			set
			{
				Debug.Log("VedioController2 setLoop");
				this.bLoop = value;
				this.scrMedia.m_bLoop = this.bLoop;
			}
		}

		public string Resource
		{
			get
			{
				return this.vedioName;
			}
			set
			{
				this.bStart = false;
				this.vedioName = value;
				this.scrMedia.Load(this.vedioName + ".mp4");
			}
		}

		public GameObject Obj
		{
			get
			{
				return base.gameObject;
			}
		}

		public void Unload()
		{
			this.scrMedia.UnLoad();
		}

		private void Awake()
		{
			Debug.Log("VedioController2 Awake");
			Transform transform = base.transform.FindChild("BG_PLANE");
			transform.localPosition = new Vector3(0f, -1f, 110f);
			transform.localRotation = Quaternion.Euler(new Vector3(0f, -180f, -180f));
			transform.localScale = new Vector3(250f, 250f, 100f);
			Transform transform2 = base.transform.FindChild("VideoPlayer");
			transform2.localPosition = new Vector3(0f, 0f, 10f);
			transform2.localRotation = Quaternion.identity;
			transform2.localScale = new Vector3(20f, 20f, 1f);
			base.transform.localPosition = new Vector3(1000f, 0f, -10f);
			base.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			base.transform.localScale = Vector3.one;
		}

		private void Start()
		{
			this.Resize();
			this.scrMedia.OnEnd = new MediaPlayerCtrl.VideoEnd(this.OnEnd);
			this.scrMedia.OnReady = new MediaPlayerCtrl.VideoReady(this.OnReady);
		}

		private void Update()
		{
			if (this.m_iOrgWidth != Screen.width)
			{
				this.Resize();
			}
			if (this.m_iOrgHeight != Screen.height)
			{
				this.Resize();
			}
			if (this.bPlay && !this.bStart && this.scrMedia.GetSeekPosition() > 0)
			{
				this.bStart = true;
				this.OnStart();
				Debug.Log("VedioController2     OnStart");
			}
		}

		private void OnReady()
		{
			if (this.callback_ready != null)
			{
				this.callback_ready(this);
			}
			Debug.Log("VedioController2      movie player:on ready");
		}

		private void OnEnd()
		{
			this.bStart = false;
			if (this.callback_end != null)
			{
				this.callback_end(this);
			}
		}

		private void OnStart()
		{
			if (this.callback_start != null)
			{
				this.callback_start(this);
			}
		}

		private void Resize()
		{
			this.m_iOrgWidth = Screen.width;
			this.m_iOrgHeight = Screen.height;
			float num = (float)this.m_iOrgHeight / (float)this.m_iOrgWidth;
			this.scrMedia.transform.localScale = new Vector3(20f / num, 20f / num, 1f);
			this.scrMedia.transform.GetComponent<MediaPlayerCtrl>().Resize();
			Debug.Log("VedioController2 Resize");
		}
	}
}
