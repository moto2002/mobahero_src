using System;
using System.Collections.Generic;
using UnityEngine;

public class TestMist : MonoBehaviour
{
	public Camera cam;

	public static Camera cx;

	public Transform[] lmx;

	public Transform[] blx;

	public Mist3Adapter adp;

	private static List<Units> lm;

	private static List<Units> bl;

	public Renderer rd;

	public Material mat;

	public static List<Units> GetUnits(TeamType team)
	{
		if (team == TeamType.LM)
		{
			return TestMist.lm;
		}
		return TestMist.bl;
	}

	private void Start()
	{
		this.Init();
		TestMist.cx = this.cam;
		this.mat = this.rd.sharedMaterial;
	}

	private void Update()
	{
		this.mat.SetTexture("_MainTex", PostMist.Instance.footMask);
	}

	private void Init()
	{
		TestMist.lm = new List<Units>();
		TestMist.bl = new List<Units>();
		int layer = LayerMask.NameToLayer("Unit");
		Transform[] array = this.lmx;
		for (int i = 0; i < array.Length; i++)
		{
			Transform transform = array[i];
			UMist uMist = transform.gameObject.AddComponent<UMist>();
			uMist.teamType = 0;
			uMist.tag = TargetTag.Hero.ToString();
			transform.gameObject.layer = layer;
			TestMist.lm.Add(uMist);
		}
		Transform[] array2 = this.blx;
		for (int j = 0; j < array2.Length; j++)
		{
			Transform transform2 = array2[j];
			UMist uMist2 = transform2.gameObject.AddComponent<UMist>();
			uMist2.teamType = 1;
			uMist2.tag = TargetTag.Hero.ToString();
			transform2.gameObject.layer = layer;
			TestMist.bl.Add(uMist2);
		}
		this.adp.CheckMistSettings();
	}
}
