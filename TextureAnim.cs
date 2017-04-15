using System;
using UnityEngine;

public class TextureAnim : MonoBehaviour
{
	public SpriteRenderer TextureUI;

	public string TexturePath;

	public float fireRate = 0.04f;

	public int count = 124;

	public int frame;

	public bool stopAtEnd = true;

	private bool isEnd;

	public bool forceRun;

	private float nextFire;

	private bool ActivateWait;

	public bool forceExit;

	public void StartAnim()
	{
		if (base.audio != null)
		{
			base.audio.Play();
		}
		this.ActivateWait = true;
	}

	private void Update()
	{
		if (this.ActivateWait || this.forceRun)
		{
			this.TextureUI.enabled = true;
			if (this.frame < this.count && !this.forceExit)
			{
				if (Time.time > this.nextFire)
				{
					this.nextFire = Time.time + this.fireRate;
					this.ShowTexture(this.frame);
					this.frame++;
				}
			}
			else if (this.frame == this.count && this.stopAtEnd)
			{
				if (!this.isEnd)
				{
					this.nextFire = Time.time + this.fireRate;
					this.ShowTexture(this.frame);
					this.isEnd = true;
				}
			}
			else if (!this.isEnd)
			{
				this.isEnd = true;
			}
		}
		else
		{
			this.frame = 0;
		}
	}

	private void ShowTexture(int curframe)
	{
		if (curframe < 10)
		{
			this.TextureUI.sprite = (Sprite)Resources.Load(this.TexturePath + "00" + curframe.ToString(), typeof(Sprite));
		}
		else if (curframe >= 10 && curframe < 100)
		{
			if (curframe % 30 == 0)
			{
				Resources.UnloadUnusedAssets();
			}
			this.TextureUI.sprite = (Sprite)Resources.Load(this.TexturePath + "0" + curframe.ToString(), typeof(Sprite));
		}
		else
		{
			if (curframe % 30 == 0)
			{
				Resources.UnloadUnusedAssets();
			}
			this.TextureUI.sprite = (Sprite)Resources.Load(this.TexturePath + curframe.ToString(), typeof(Sprite));
		}
	}

	public void Destory()
	{
		this.frame = 0;
		this.TextureUI.enabled = false;
		this.ActivateWait = false;
		UnityEngine.Object.Destroy(base.transform.parent.gameObject);
	}
}
