using System;
using UnityEngine;

public class ChengjiuControl : MonoBehaviour
{
	public bool ActivateWait = true;

	public bool once = true;

	public float fireRate = 0.02f;

	public string SpriteName = "qizi_";

	public int firstnum = 1;

	public int maxlength = 15;

	public int namelength = 2;

	private float nextFire;

	private int count;

	private void Start()
	{
		this.count = this.firstnum;
	}

	private void Update()
	{
		if (this.ActivateWait)
		{
			if (this.count < this.maxlength)
			{
				if (Time.time > this.nextFire)
				{
					this.nextFire = Time.time + this.fireRate;
					base.GetComponent<UISprite>().spriteName = this.SpriteName + this.count.ToString().PadLeft(this.namelength, '0');
					this.count++;
				}
			}
			else if (this.once)
			{
				this.ActivateWait = false;
				this.count = this.maxlength;
			}
			else
			{
				this.count = this.firstnum;
			}
		}
	}

	private void OnDisable()
	{
		base.GetComponent<UISprite>().spriteName = this.SpriteName + "0" + 1;
		this.count = 0;
		this.ActivateWait = true;
	}
}
