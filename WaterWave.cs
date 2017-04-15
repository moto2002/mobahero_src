using System;
using UnityEngine;

public class WaterWave : MonoBehaviour
{
	[SerializeField]
	private Renderer m_rendererWater;

	private Material m_materialWater;

	private AnimationCurve m_secondTexCurve;

	private AnimationCurve m_thirdTexCurve;

	private float m_timeInterval_SecondTex;

	private float m_timeInterval_ThirdTex;

	private float m_endTime_SecondTex;

	private float m_endTime_ThirdTex;

	private void Awake()
	{
		this.m_materialWater = this.m_rendererWater.material;
		this.m_timeInterval_SecondTex = 0f;
		this.m_timeInterval_ThirdTex = 0f;
		this.m_endTime_SecondTex = 20f;
		this.m_endTime_ThirdTex = 10f;
		this.m_secondTexCurve = AnimationCurve.Linear(0f, 1f, this.m_endTime_SecondTex, 0f);
		this.m_thirdTexCurve = AnimationCurve.Linear(0f, 1f, this.m_endTime_ThirdTex, 0f);
	}

	private void Update()
	{
		if (this.m_timeInterval_SecondTex >= this.m_endTime_SecondTex)
		{
			this.m_timeInterval_SecondTex %= this.m_endTime_SecondTex;
		}
		if (this.m_timeInterval_ThirdTex >= this.m_endTime_ThirdTex)
		{
			this.m_timeInterval_ThirdTex %= this.m_endTime_ThirdTex;
		}
		this.m_materialWater.SetTextureOffset("_SecondTex", new Vector2(this.m_secondTexCurve.Evaluate(this.m_timeInterval_SecondTex), 0f));
		this.m_materialWater.SetTextureOffset("_ThirdTex", new Vector2(this.m_thirdTexCurve.Evaluate(this.m_timeInterval_ThirdTex), 0f));
		this.m_timeInterval_SecondTex += Time.deltaTime;
		this.m_timeInterval_ThirdTex += Time.deltaTime;
	}

	public void SetAlphaTexOffset(float offsetValue)
	{
		this.m_materialWater.SetTextureOffset("_AlphaTex", new Vector2(0.005f * offsetValue, 0f));
	}
}
