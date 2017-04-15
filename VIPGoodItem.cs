using System;
using UnityEngine;

public class VIPGoodItem : MonoBehaviour
{
	public UISprite GoodTexture;

	public Transform Recommend;

	public UILabel Name;

	public UILabel Introduction;

	public UILabel NeedMoney;

	private void Awake()
	{
		this.GoodTexture = base.transform.Find("GoodTexture").GetComponent<UISprite>();
		this.Recommend = base.transform.Find("Recommend");
		this.Name = base.transform.Find("GoodNameBg/Name").GetComponent<UILabel>();
		this.Introduction = base.transform.Find("Introduction").GetComponent<UILabel>();
		this.NeedMoney = base.transform.Find("NeedMoney").GetComponent<UILabel>();
		UIEventListener expr_8D = UIEventListener.Get(base.gameObject);
		expr_8D.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_8D.onClick, new UIEventListener.VoidDelegate(this.OnClik_Event));
	}

	private void OnClik_Event(GameObject Object_1 = null)
	{
	}
}
