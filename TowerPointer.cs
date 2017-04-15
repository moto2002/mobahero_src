using System;
using UnityEngine;

public class TowerPointer : MonoBehaviour
{
	public Transform attackRange;

	public TweenScale attackTweenScale;

	public Material materialRed;

	public Material materialYellow;

	public Material materialGreen;

	public MeshRenderer spriteRenderer;

	public void ShowPointer(float radius, TowerAttackIndicator.WarnLevel level, bool animateShow, float alpha = 1f)
	{
		base.transform.name = "TowerPointer";
		this.attackRange.localScale = new Vector3(radius, 1f, radius);
		this.attackRange.localPosition = Vector3.zero;
		if (level == TowerAttackIndicator.WarnLevel.Green)
		{
			this.spriteRenderer.material = this.materialGreen;
		}
		else if (level == TowerAttackIndicator.WarnLevel.Red)
		{
			this.spriteRenderer.material = this.materialRed;
		}
		else if (level == TowerAttackIndicator.WarnLevel.Yellow)
		{
			this.spriteRenderer.material = this.materialYellow;
		}
		Color color = this.spriteRenderer.sharedMaterial.GetColor("_Color");
		color.a = alpha;
		this.spriteRenderer.sharedMaterial.SetColor("_Color", color);
		if (animateShow)
		{
			this.Show();
		}
	}

	private void Show()
	{
		this.attackRange.gameObject.SetActive(true);
		this.attackTweenScale.from = Vector3.zero;
		this.attackTweenScale.to = this.attackRange.localScale;
		this.attackTweenScale.Begin();
	}
}
