using System;
using UnityEngine;

public class MinimapHeroMark : MonoBehaviour
{
	public UISprite m_texture;

	public Transform m_bg;

	public Transform m_deathMark;

	private bool isDeathState;

	public void ShowDeath()
	{
		this.m_texture.enabled = false;
		this.m_bg.gameObject.SetActive(false);
		this.m_deathMark.gameObject.SetActive(true);
		this.isDeathState = true;
	}

	public void ShowNomal()
	{
		this.m_texture.enabled = true;
		this.m_bg.gameObject.SetActive(true);
		this.m_deathMark.gameObject.SetActive(false);
		this.isDeathState = false;
	}

	public bool IsDeathState()
	{
		return this.isDeathState;
	}
}
