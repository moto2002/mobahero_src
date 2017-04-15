using System;
using UnityEngine;

public class Circle : MonoBehaviour
{
	private Units curPlayer;

	private Renderer mainRender;

	private Color curColor;

	private Transform curTrans;

	private float curRaduio = 3f;

	private void Awake()
	{
		this.mainRender = base.GetComponentInChildren<Renderer>();
		this.curColor = this.mainRender.material.GetColor("_TintColor");
		this.curColor.a = 0f;
		this.curTrans = base.transform;
	}

	private void OnSpawned()
	{
		this.curPlayer = PlayerControlMgr.Instance.GetPlayer();
		this.curColor.a = this.DistanceToAlpha();
		this.mainRender.material.SetColor("_TintColor", this.curColor);
	}

	private void FixedUpdate()
	{
		this.curPlayer = PlayerControlMgr.Instance.GetPlayer();
		this.curColor.a = this.DistanceToAlpha();
		this.mainRender.material.SetColor("_TintColor", this.curColor);
	}

	private float DistanceToAlpha()
	{
		if (this.curPlayer == null)
		{
			return 0f;
		}
		return Mathf.Clamp01((10f - Vector3.Distance(this.curTrans.position, this.curPlayer.mTransform.position) + this.curRaduio) / 10f);
	}

	public void SetRaduio(float raduio)
	{
		this.curRaduio = raduio;
		this.curTrans.localScale = Vector3.one * this.curRaduio;
	}
}
