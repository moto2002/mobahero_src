using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class CardsAlpha : MonoBehaviour
{
	private List<SpriteRenderer> cards = new List<SpriteRenderer>();

	public float curRatio;

	public int curShowNum;

	public int totalCards;

	public float duration = 1f;

	private void Start()
	{
		if (this.cards != null && this.cards.Count <= 0)
		{
			SpriteRenderer[] componentsInChildren = base.GetComponentsInChildren<SpriteRenderer>();
			SpriteRenderer[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				SpriteRenderer item = array[i];
				this.cards.Add(item);
			}
			this.totalCards = this.cards.Count;
		}
		this.ResetColor();
	}

	public void ResetColor()
	{
		int num = 0;
		foreach (SpriteRenderer current in this.cards)
		{
			num++;
			if (num != 1)
			{
				Color color = current.color;
				current.color = new Color(color.r, color.g, color.b, 0f);
			}
		}
	}

	public void StartShowingCards()
	{
		base.StartCoroutine(this.ShowCards());
	}

	[DebuggerHidden]
	private IEnumerator ShowCards()
	{
		CardsAlpha.<ShowCards>c__Iterator1D6 <ShowCards>c__Iterator1D = new CardsAlpha.<ShowCards>c__Iterator1D6();
		<ShowCards>c__Iterator1D.<>f__this = this;
		return <ShowCards>c__Iterator1D;
	}

	private void UpdateCards(int showNum)
	{
		if (showNum > this.totalCards)
		{
			showNum = this.totalCards;
		}
		float num = 1f / (float)showNum;
		float num2 = 1f;
		for (int i = 0; i < this.totalCards; i++)
		{
			SpriteRenderer spriteRenderer = this.cards[i];
			if (i + 1 <= showNum)
			{
				num2 -= num;
				spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, num2);
			}
			else
			{
				spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0f);
			}
		}
	}
}
