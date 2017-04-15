using System;
using System.Collections.Generic;
using UnityEngine;

public class SkinInfo
{
	private int _skinId;

	private Texture _tex;

	private float _cost;

	private float cost_money;

	private float cost_bottle;

	private int containerType;

	private int costType;

	private int costType_another;

	private float discount;

	private bool iswear;

	public int skinId
	{
		get
		{
			return this._skinId;
		}
	}

	public Texture tex
	{
		get
		{
			return this._tex;
		}
	}

	public float Cost
	{
		get
		{
			return this._cost;
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

	public bool IsWear
	{
		get
		{
			return this.iswear;
		}
	}

	public SkinInfo(int skinId, int cost)
	{
		this._skinId = skinId;
		this._tex = CachedRes.getSkinTex(skinId);
		this._cost = (float)cost;
	}

	public SkinInfo(Texture skinTex, int cost)
	{
		this._tex = skinTex;
		this._cost = (float)cost;
	}

	public SkinInfo(int skinid, List<float> info, bool is_wear = false)
	{
		this.iswear = is_wear;
		this._skinId = skinid;
		this._tex = CachedRes.getSkinTex(this.skinId);
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
						this._cost = info[3];
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
							this._cost = info[2];
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
							this._cost = info[3];
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
							this._cost = info[4];
						}
					}
					else
					{
						this.cost_money = info[4];
					}
				}
			}
			return;
		}
	}
}
