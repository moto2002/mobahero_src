using System;
using System.Collections.Generic;
using UnityEngine;

public class GoodsInfo
{
	private static GoodsInfo instance;

	private static object obj_lock = new object();

	private float cost_diamond;

	private float cost_money;

	private float cost_bottle;

	private int containerType;

	private int costType;

	private int costType_another;

	private float discount = 1f;

	public float CostDiamond
	{
		get
		{
			return this.cost_diamond;
		}
	}

	public float CostMoney
	{
		get
		{
			return this.cost_money;
		}
	}

	public float CostBottle
	{
		get
		{
			return this.cost_bottle;
		}
	}

	public int Container
	{
		get
		{
			return this.containerType;
		}
	}

	public int CostType
	{
		get
		{
			return this.costType;
		}
	}

	public int CostTypeanother
	{
		get
		{
			return this.costType_another;
		}
	}

	public float Discount
	{
		get
		{
			return this.discount;
		}
	}

	private GoodsInfo()
	{
	}

	public static GoodsInfo GetInstance()
	{
		if (GoodsInfo.instance == null)
		{
			object obj = GoodsInfo.obj_lock;
			lock (obj)
			{
				if (GoodsInfo.instance == null)
				{
					GoodsInfo.instance = new GoodsInfo();
				}
			}
		}
		return GoodsInfo.instance;
	}

	public void SetInfo(List<float> info)
	{
		if (info != null)
		{
			if (info[0] == 1f)
			{
				this.containerType = 3;
				this.discount = info[1];
				this.costType = Convert.ToInt32(info[2]);
				int num = this.costType;
				if (num != 1)
				{
					if (num != 2)
					{
						if (num == 9)
						{
							this.cost_bottle = info[3];
						}
					}
					else
					{
						this.cost_diamond = info[3];
					}
				}
				else
				{
					this.cost_money = info[3];
				}
			}
			else
			{
				this.containerType = Convert.ToInt32(info[info.Count - 1]);
				this.discount = 0f;
				int num = this.containerType;
				if (num != 1)
				{
					if (num == 2)
					{
						this.costType = Convert.ToInt32(info[1]);
						this.costType_another = Convert.ToInt32(info[2]);
					}
				}
				else
				{
					this.costType = Convert.ToInt32(info[1]);
					this.costType_another = 0;
				}
				if (this.costType_another == 0)
				{
					num = this.costType;
					if (num != 1)
					{
						if (num != 2)
						{
							if (num == 9)
							{
								this.cost_bottle = info[2];
							}
						}
						else
						{
							this.cost_diamond = info[2];
						}
					}
					else
					{
						this.cost_money = info[2];
					}
				}
				else
				{
					num = this.costType;
					if (num != 1)
					{
						if (num != 2)
						{
							if (num == 9)
							{
								this.cost_bottle = info[3];
							}
						}
						else
						{
							this.cost_diamond = info[3];
						}
					}
					else
					{
						this.cost_money = info[3];
					}
					num = this.costType_another;
					if (num != 1)
					{
						if (num != 2)
						{
							if (num == 9)
							{
								this.cost_bottle = info[4];
							}
						}
						else
						{
							this.cost_diamond = info[4];
						}
					}
					else
					{
						this.cost_money = info[4];
					}
				}
			}
		}
	}

	public void DisplayInfo(Transform SinglePrice, Transform BothPrice, Transform DiscountPrice)
	{
		if (null == SinglePrice || null == BothPrice || null == DiscountPrice)
		{
			return;
		}
		switch (this.Container)
		{
		case 1:
		{
			SinglePrice.gameObject.SetActive(true);
			BothPrice.gameObject.SetActive(false);
			DiscountPrice.gameObject.SetActive(false);
			int costTypeanother = this.CostType;
			if (costTypeanother != 1)
			{
				if (costTypeanother != 2)
				{
					if (costTypeanother != 9)
					{
					}
				}
				else
				{
					SinglePrice.GetChild(0).GetComponent<UISprite>().spriteName = "icon_zuanshi";
					SinglePrice.GetChild(1).GetComponent<UILabel>().text = this.CostDiamond.ToString();
				}
			}
			else
			{
				SinglePrice.GetChild(0).GetComponent<UISprite>().spriteName = "icon_gold";
				SinglePrice.GetChild(1).GetComponent<UILabel>().text = this.CostMoney.ToString();
			}
			SinglePrice.GetComponent<UICenterHelper>().Reposition();
			break;
		}
		case 2:
		{
			SinglePrice.gameObject.SetActive(false);
			BothPrice.gameObject.SetActive(true);
			DiscountPrice.gameObject.SetActive(false);
			int costTypeanother = this.CostType;
			if (costTypeanother != 1)
			{
				if (costTypeanother != 2)
				{
					if (costTypeanother != 9)
					{
					}
				}
				else
				{
					BothPrice.GetChild(1).GetComponent<UILabel>().text = this.CostDiamond.ToString();
				}
			}
			else
			{
				BothPrice.GetChild(3).GetComponent<UILabel>().text = this.CostMoney.ToString();
			}
			costTypeanother = this.CostTypeanother;
			if (costTypeanother != 1)
			{
				if (costTypeanother != 2)
				{
					if (costTypeanother != 9)
					{
					}
				}
				else
				{
					BothPrice.GetChild(1).GetComponent<UILabel>().text = this.CostDiamond.ToString();
				}
			}
			else
			{
				BothPrice.GetChild(3).GetComponent<UILabel>().text = this.CostMoney.ToString();
			}
			BothPrice.GetComponent<UICenterHelper>().Reposition();
			break;
		}
		case 3:
		{
			SinglePrice.gameObject.SetActive(false);
			BothPrice.gameObject.SetActive(false);
			DiscountPrice.gameObject.SetActive(true);
			int costTypeanother = this.CostType;
			if (costTypeanother != 1)
			{
				if (costTypeanother != 2)
				{
					if (costTypeanother != 9)
					{
					}
				}
				else
				{
					DiscountPrice.GetChild(0).GetComponent<UISprite>().spriteName = "icon_zuanshi";
					DiscountPrice.GetChild(3).GetComponent<UILabel>().text = Mathf.CeilToInt(this.CostDiamond * this.Discount).ToString();
					DiscountPrice.GetChild(1).GetComponent<UILabel>().text = "原价" + this.CostDiamond;
				}
			}
			else
			{
				DiscountPrice.GetChild(0).GetComponent<UISprite>().spriteName = "icon_gold";
				DiscountPrice.GetChild(3).GetComponent<UILabel>().text = Mathf.CeilToInt(this.CostMoney * this.Discount).ToString();
				DiscountPrice.GetChild(1).GetComponent<UILabel>().text = "原价" + this.CostMoney;
			}
			DiscountPrice.GetComponent<UICenterHelper>().Reposition();
			break;
		}
		}
	}
}
