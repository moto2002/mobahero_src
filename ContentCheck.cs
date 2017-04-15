using System;
using UnityEngine;

public class ContentCheck : MonoBehaviour
{
	[SerializeField]
	private UIInput content;

	private UILabel label;

	private void Awake()
	{
		this.label = base.GetComponent<UILabel>();
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (this.content.value.Length > 28)
		{
			if (null != this.label)
			{
				this.label.alpha = 1f;
				int num = 28 - this.content.value.Length;
				this.label.text = num.ToString();
			}
		}
		else
		{
			this.label.alpha = 0f;
		}
	}
}
