using System;
using UnityEngine;

public class Gray : MonoBehaviour
{
	public Shader sh;

	public Camera cam;

	public bool doGray = true;

	private Material mat;

	private bool fin;

	private float fadeIn = 1f;

	private float fadeInTimer;

	private bool fout;

	private float fadeOut = 1f;

	private float fadeOutTimer;

	public static int rtScal = 1;

	public static Gray instance;

	public RenderTexture rt;

	public Material pMat;

	public GameObject gr;

	private void Start()
	{
		Gray.instance = this;
		this.sh = Shader.Find("MyShader/PostEffects/GrayScrAnd");
		if (this.sh == null)
		{
			this.doGray = false;
			return;
		}
		this.cam = base.camera;
		this.SetPlane(this.cam);
		this.rt = new RenderTexture((int)this.cam.pixelWidth / Gray.rtScal, (int)this.cam.pixelHeight / Gray.rtScal, 16);
		this.cam.targetTexture = this.rt;
		this.pMat.SetTexture("_MainTex", this.rt);
	}

	private void Update()
	{
		if (!this.doGray)
		{
			return;
		}
		if (this.fin)
		{
			this.fadeInTimer += Time.deltaTime;
			if (this.fadeInTimer > this.fadeIn)
			{
				this.fadeInTimer = this.fadeIn;
			}
			this.pMat.SetFloat("amt", Mathf.Min(0.9f, this.fadeInTimer / this.fadeIn));
		}
		if (this.fout)
		{
			this.fadeOutTimer += Time.deltaTime;
			if (this.fadeOutTimer > this.fadeOut)
			{
				this.fadeOutTimer = this.fadeOut;
				this.doGray = false;
				base.enabled = false;
				this.cam.targetTexture = null;
				if (this.gr != null)
				{
					this.gr.SetActive(false);
				}
			}
			this.pMat.SetFloat("amt", 1f - this.fadeOutTimer / this.fadeOut);
		}
	}

	public void DoGrayOnEffect(float fadeIn = 1f)
	{
		base.enabled = true;
		this.doGray = true;
		this.fin = true;
		this.fadeIn = fadeIn;
		this.fadeInTimer = 0f;
		this.fout = false;
		if (this.rt != null && this.cam != null)
		{
			this.cam.targetTexture = this.rt;
		}
		if (this.gr != null)
		{
			this.gr.SetActive(true);
		}
	}

	public void Finish(float fadeOut = 1f)
	{
		this.doGray = true;
		this.fout = true;
		this.fadeOut = fadeOut;
		this.fadeOutTimer = 0f;
		this.fin = false;
	}

	public void SetPlane(Camera cam)
	{
		GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
		gameObject.layer = LayerMask.NameToLayer("LockUI");
		MeshCollider component = gameObject.GetComponent<MeshCollider>();
		UnityEngine.Object.Destroy(component);
		Shader shader = Shader.Find("MyShader/PostEffects/GrayScrAnd");
		Renderer component2 = gameObject.GetComponent<MeshRenderer>();
		component2.material = new Material(shader);
		component2.material.hideFlags = HideFlags.DontSave;
		component2.castShadows = false;
		component2.receiveShadows = false;
		this.gr = new GameObject("Gray Camera");
		this.gr.transform.parent = cam.transform;
		this.gr.transform.position = Vector3.zero;
		this.gr.transform.localScale = Vector3.one;
		this.gr.transform.localRotation = Quaternion.identity;
		Camera camera = this.gr.AddComponent<Camera>();
		camera.CopyFrom(cam);
		camera.cullingMask = LayerMask.GetMask(new string[]
		{
			"LockUI"
		});
		camera.depth = cam.depth + 0.2f;
		camera.farClipPlane = camera.nearClipPlane + 2f;
		Transform transform = gameObject.transform;
		transform.parent = this.gr.transform;
		transform.localRotation = Quaternion.identity;
		transform.localPosition = new Vector3(0f, 0f, this.gr.camera.nearClipPlane + 0.02f);
		float num = 0.0174532924f * cam.fieldOfView;
		float num2 = Mathf.Tan(num / 2f) * (this.gr.camera.nearClipPlane + 0.02f) * 2f;
		transform.localScale = new Vector3(num2 * this.gr.camera.aspect, num2, 1f);
		this.pMat = component2.sharedMaterial;
	}
}
