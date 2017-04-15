using System;
using UnityEngine;

public class VedioController : MonoBehaviour, IVedioController
{
	public enum ShowModel
	{
		heightFirst,
		weightFirst,
		full
	}

	private const string FormatStr = ".ogv";

	[SerializeField]
	private int controllerID;

	[SerializeField]
	private Camera vCamera;

	[SerializeField]
	private VideoPlayer vPlayer;

	[SerializeField]
	private GameObject plane;

	[SerializeField]
	private float wIndex = 16f;

	[SerializeField]
	private float hIndex = 9f;

	[SerializeField]
	private VedioController.ShowModel showMode;

	private bool bPlay;

	private bool bLoop;

	private PlayState state;

	private string path;

	private float vedioRatio;

	private new bool active = true;

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
		get
		{
			return this.controllerID;
		}
		set
		{
			this.controllerID = value;
		}
	}

	public bool Enable
	{
		get
		{
			return this.active;
		}
		set
		{
			if (this.active != value)
			{
				this.active = value;
				if (!this.active)
				{
					this.Play = false;
				}
				this.State = PlayState.unActive;
				this.vCamera.gameObject.SetActive(this.active);
			}
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
			if (this.bPlay != value)
			{
				this.bPlay = value;
				this.vPlayer.Play = this.bPlay;
				if (this.state != PlayState.stop)
				{
					this.state = ((!this.bPlay) ? PlayState.pause : PlayState.playing);
				}
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
			this.bLoop = value;
			this.vPlayer.Loop = this.bLoop;
		}
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

	public string Resource
	{
		get
		{
			return this.path;
		}
		set
		{
			this.path = value;
			this.vPlayer.Video = this.path + ".ogv";
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
		MediaKitProcessor mediaKitProcessor = UnityEngine.Object.FindObjectOfType<MediaKitProcessor>();
		if (null != mediaKitProcessor)
		{
			UnityEngine.Object.Destroy(mediaKitProcessor.gameObject);
		}
	}

	private void Awake()
	{
		this.State = PlayState.idle;
		this.Play = false;
		this.Loop = false;
		this.Resource = string.Empty;
		if (this.hIndex == 0f)
		{
			this.hIndex = 1f;
		}
		this.vedioRatio = this.wIndex / this.hIndex;
		this.AdjustAspect();
		this.vCamera.depth = (float)(-(float)(2 + this.ID));
		CamRatio.SetupCamera(this.vCamera, 0f);
	}

	private void Start()
	{
		this.Loop = this.vPlayer.Loop;
		this.Play = this.vPlayer.Play;
		this.vPlayer.OnStart.Add(base.gameObject);
		this.vPlayer.OnReady.Add(base.gameObject);
		this.vPlayer.OnStop.Add(base.gameObject);
	}

	private void OnVideoStart()
	{
		this.State = PlayState.playing;
		if (this.callback_start != null)
		{
			this.callback_start(this);
		}
	}

	private void OnVideoStop()
	{
		this.State = PlayState.stop;
		if (this.callback_end != null)
		{
			this.callback_end(this);
		}
	}

	private void OnVideoReady()
	{
		this.State = PlayState.ready;
		if (this.callback_ready != null)
		{
			this.callback_ready(this);
		}
	}

	private void AdjustAspect()
	{
		float num = 1f;
		float num2 = 1f;
		switch (this.showMode)
		{
		case VedioController.ShowModel.heightFirst:
			num = this.vCamera.orthographicSize * 2f / 10f;
			num2 = num * this.vedioRatio;
			break;
		case VedioController.ShowModel.weightFirst:
			num2 = this.vCamera.orthographicSize * 2f * this.vCamera.aspect / 10f;
			num = num2 / this.vedioRatio;
			break;
		case VedioController.ShowModel.full:
			if (this.vCamera.aspect > this.vedioRatio)
			{
				num = this.vCamera.orthographicSize * 2f / 10f;
				num2 = num * this.vedioRatio;
			}
			else
			{
				num2 = this.vedioRatio;
				num = 1f;
			}
			break;
		}
		this.plane.transform.localScale = new Vector3(num2, 1f, num);
	}
}
