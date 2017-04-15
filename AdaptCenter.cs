using System;
using UnityEngine;

public class AdaptCenter : MonoBehaviour
{
	public bool adaptOn = true;

	public float acc;

	public int step;

	private int frameCount = 10;

	private int fps = 60;

	public int particleFPS = 30;

	private bool checkShadow = true;

	public int shadowFPS = 25;

	private int shadowAcc;

	private static AdaptCenter _instance;

	public static AdaptCenter Instance
	{
		get
		{
			if (AdaptCenter._instance == null)
			{
				AdaptCenter.Init();
			}
			return AdaptCenter._instance;
		}
	}

	private void Start()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Update()
	{
		if (!this.adaptOn)
		{
			return;
		}
		this.step++;
		this.acc += Time.deltaTime;
		if (this.step % this.frameCount == 0)
		{
			this.fps = (int)((float)this.frameCount / this.acc);
			this.step = 0;
			this.acc = 0f;
			if (this.fps < this.particleFPS)
			{
				ParticleAdapter.AdaptDown();
			}
			if (this.checkShadow)
			{
				if (this.fps < this.shadowFPS)
				{
					this.shadowAcc++;
					if (this.shadowAcc > 10)
					{
						AdaptCenter.TurnOffShadows();
						this.checkShadow = false;
					}
				}
				else
				{
					this.shadowAcc = 0;
				}
			}
		}
	}

	private static void Init()
	{
		GameObject gameObject = new GameObject("AdaptCenter");
		AdaptCenter._instance = gameObject.AddComponent<AdaptCenter>();
	}

	public static void EnableAdapter()
	{
		AdaptCenter.Instance.adaptOn = true;
		AdaptCenter.Instance.acc = 0f;
		AdaptCenter.Instance.step = 0;
	}

	public static void DisableAdapter()
	{
		AdaptCenter.Instance.adaptOn = false;
	}

	public static void SetVault(int fpsVault)
	{
		AdaptCenter.Instance.particleFPS = fpsVault;
	}

	private static void TurnOffShadows()
	{
		Light[] array = UnityEngine.Object.FindObjectsOfType<Light>();
		Light[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			Light light = array2[i];
			light.shadows = LightShadows.None;
		}
	}
}
