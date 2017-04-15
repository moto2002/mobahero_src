using System;
using UnityEngine;

public class Timed : MonoBehaviour
{
	public float m_fDestruktionSpeed = 0.5f;

	public Material m_Mat;

	public float m_fTime;

	private void Start()
	{
		this.m_Mat = base.renderer.material;
	}

	private void Update()
	{
		this.m_fTime += Time.deltaTime * this.m_fDestruktionSpeed;
		if (this.m_fTime >= 2f)
		{
			this.m_fTime = 2f;
		}
		this.m_Mat.SetFloat("_Amount", this.m_fTime);
	}

	public void ResetEffect(Material mat)
	{
		this.m_fTime = 0f;
		this.m_Mat = mat;
		this.m_Mat.SetFloat("_Amount", this.m_fTime);
	}
}
