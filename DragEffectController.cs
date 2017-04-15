using System;
using UnityEngine;

[RequireComponent(typeof(UIWidget))]
public class DragEffectController : MonoBehaviour
{
	[Tooltip("涉及到坐标计算的相关控件")]
	public Transform[] associatingParents;

	[Range(1f, 2000f), SerializeField, Tooltip("最大响应边界（越界坐标视作最小值点）")]
	public float maxAttrEdge = 500f;

	[Tooltip("零刻点（最大值点）")]
	public float zeroLinePosX;

	[Tooltip("最小缩放比例")]
	public float minScale;

	[Tooltip("最大缩放比例")]
	public float maxScale = 1f;

	[Range(0f, 1f), SerializeField, Tooltip("最小透明度")]
	public float minAlpha;

	[Range(0f, 1f), SerializeField, Tooltip("最大透明度")]
	public float maxAlpha = 1f;

	private UIWidget widget;

	private void Awake()
	{
		this.widget = base.transform.GetComponent<UIWidget>();
	}

	private void OnDestroy()
	{
	}

	private void Update()
	{
		float num = 0f;
		for (int i = 0; i < this.associatingParents.Length; i++)
		{
			num += ((!(this.associatingParents[i] == null)) ? this.associatingParents[i].localPosition.x : 0f);
		}
		float num2 = Mathf.Abs(base.transform.localPosition.x + num - this.zeroLinePosX);
		float num3 = Mathf.Min(num2 / this.maxAttrEdge, 1f);
		float d = this.maxScale - (this.maxScale - this.minScale) * num3;
		float alpha = this.maxAlpha - (this.maxAlpha - this.minAlpha) * num3;
		base.transform.localScale = Vector3.one * d;
		this.widget.alpha = alpha;
	}
}
