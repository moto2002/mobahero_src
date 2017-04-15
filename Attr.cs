using MobaHeros;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attr
{
	protected AttrType MainAttrType;

	protected int level = 1;

	private Dictionary<DataManager.DataPoolType, Dictionary<AttrType, float>> _dataPool = new Dictionary<DataManager.DataPoolType, Dictionary<AttrType, float>>();

	private Dictionary<DataType, object> _dataTypeDictionary = new Dictionary<DataType, object>();

	protected Dictionary<AttrType, float> GetDataPool(DataManager.DataPoolType type)
	{
		if (!this._dataPool.ContainsKey(type))
		{
			this._dataPool.Add(type, new Dictionary<AttrType, float>());
		}
		return this._dataPool[type];
	}

	protected float SetAttr(AttrType attrType, float value)
	{
		switch (attrType)
		{
		case AttrType.Hp:
			value = Mathf.Clamp(value, 0f, this.GetAttr(AttrType.HpMax));
			return this.SetAttr(DataManager.DataPoolType.CurrentData, attrType, value);
		case AttrType.Mp:
			value = Mathf.Clamp(value, 0f, this.GetAttr(AttrType.MpMax));
			return this.SetAttr(DataManager.DataPoolType.CurrentData, attrType, value);
		case AttrType.Power:
			this.SetAttr(DataManager.DataPoolType.CurrentData, attrType, value);
			this.CalAttrByType(AttrType.HpMax);
			this.CalAttrByType(AttrType.HpRestore);
			this.CalAttrByType(AttrType.Attack);
			return value;
		case AttrType.Agile:
			this.SetAttr(DataManager.DataPoolType.CurrentData, attrType, value);
			this.CalAttrByType(AttrType.Armor);
			this.CalAttrByType(AttrType.AttackSpeed);
			this.CalAttrByType(AttrType.Attack);
			return value;
		case AttrType.Intelligence:
			this.SetAttr(DataManager.DataPoolType.CurrentData, attrType, value);
			this.CalAttrByType(AttrType.MpMax);
			this.CalAttrByType(AttrType.MagicResist);
			this.CalAttrByType(AttrType.Attack);
			return value;
		case AttrType.HpMax:
		{
			float attr = this.GetAttr(AttrType.HpMax);
			float attr2 = this.GetAttr(AttrType.Hp);
			this.SetAttr(DataManager.DataPoolType.CurrentData, AttrType.HpMax, value);
			this.SetAttr(DataManager.DataPoolType.CurrentData, AttrType.Hp, attr2 * (value / attr));
			return value;
		}
		case AttrType.MpMax:
		{
			float attr3 = this.GetAttr(AttrType.MpMax);
			float attr4 = this.GetAttr(AttrType.Mp);
			this.SetAttr(DataManager.DataPoolType.CurrentData, AttrType.MpMax, value);
			this.SetAttr(DataManager.DataPoolType.CurrentData, AttrType.Mp, attr4 * (value / attr3));
			return value;
		}
		}
		return this.SetAttr(DataManager.DataPoolType.CurrentData, attrType, value);
	}

	protected float SetAttr(DataManager.DataPoolType dataPoolType, AttrType attrType, float value)
	{
		Dictionary<AttrType, float> dataPool = this.GetDataPool(dataPoolType);
		if (!dataPool.ContainsKey(attrType))
		{
			dataPool.Add(attrType, value);
		}
		else
		{
			dataPool[attrType] = value;
		}
		return value;
	}

	public float GetAttr(DataManager.DataPoolType DataPoolType, AttrType type)
	{
		Dictionary<AttrType, float> dataPool = this.GetDataPool(DataPoolType);
		if (!dataPool.ContainsKey(type))
		{
			dataPool.Add(type, 0f);
		}
		return dataPool[type];
	}

	protected float CalAttrByType(AttrType type)
	{
		float attr = this.GetAttr(DataManager.DataPoolType.BasicData, type);
		float growthByAttr = this.GetGrowthByAttr(type);
		float num = this.CalL2ByL1(type);
		float attr2 = this.GetAttr(DataManager.DataPoolType.AddData, type);
		float attr3 = this.GetAttr(DataManager.DataPoolType.MulData, type);
		float value;
		if (type == AttrType.Shield)
		{
			value = attr + growthByAttr + num + attr2 + this.GetAttr(DataManager.DataPoolType.CurrentData, AttrType.HpMax) * attr3;
		}
		else if (type == AttrType.AttackSpeed)
		{
			value = (attr + growthByAttr + attr2) * (1f + attr3 + num);
		}
		else
		{
			value = (attr + growthByAttr + num + attr2) * (1f + attr3);
		}
		return this.SetAttr(type, value);
	}

	public float GetBasicAttr(AttrType type)
	{
		return this.GetAttr(DataManager.DataPoolType.BasicData, type) + this.CalL2ByL1(type);
	}

	private float GetGrowthByAttr(AttrType type)
	{
		if (type == AttrType.Agile)
		{
			return this.GetAttr(AttrType.AgileGroth) * (float)(this.level - 1);
		}
		if (type == AttrType.Power)
		{
			return this.GetAttr(AttrType.PowerGroth) * (float)(this.level - 1);
		}
		if (type == AttrType.Intelligence)
		{
			return this.GetAttr(AttrType.IntelligenceGroth) * (float)(this.level - 1);
		}
		return 0f;
	}

	private float CalL2ByL1(AttrType type)
	{
		switch (type)
		{
		case AttrType.Attack:
			if (this.MainAttrType == AttrType.Power || this.MainAttrType == AttrType.Agile || this.MainAttrType == AttrType.Intelligence)
			{
				return this.GetAttr(DataManager.DataPoolType.CurrentData, this.MainAttrType) * 1f;
			}
			return 0f;
		case AttrType.Armor:
			return this.GetAttr(DataManager.DataPoolType.CurrentData, AttrType.Agile);
		case AttrType.AttackSpeed:
			return this.GetAttr(DataManager.DataPoolType.CurrentData, AttrType.Agile) * 0.01f;
		case AttrType.HpMax:
			return this.GetAttr(DataManager.DataPoolType.CurrentData, AttrType.Power) * 20f;
		case AttrType.MpMax:
			return this.GetAttr(DataManager.DataPoolType.CurrentData, AttrType.Intelligence) * 10f;
		case AttrType.HpRestore:
			return this.GetAttr(DataManager.DataPoolType.CurrentData, AttrType.Power) * this.GetData<float>(DataType.HpRestoreParam);
		case AttrType.MpRestore:
			return this.GetAttr(DataManager.DataPoolType.CurrentData, AttrType.Intelligence) * this.GetData<float>(DataType.MpRestoreParam);
		}
		return 0f;
	}

	protected void ClearDataPool()
	{
		this.GetDataPool(DataManager.DataPoolType.AddData).Clear();
		this.GetDataPool(DataManager.DataPoolType.MulData).Clear();
		this.GetDataPool(DataManager.DataPoolType.BasicData).Clear();
	}

	protected void InitProperty(AttrType attrType, float value)
	{
		this.SetAttr(DataManager.DataPoolType.CurrentData, attrType, value);
		this.SetAttr(DataManager.DataPoolType.BasicData, attrType, value);
	}

	protected void CalcAllAttrs()
	{
		this.CalAttrByType(AttrType.Power);
		this.CalAttrByType(AttrType.Agile);
		this.CalAttrByType(AttrType.Intelligence);
		IEnumerator enumerator = Enum.GetValues(typeof(AttrType)).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				AttrType attrType = (AttrType)((int)enumerator.Current);
				if (attrType != AttrType.None && attrType != AttrType.End && attrType != AttrType.Hp && attrType != AttrType.Mp)
				{
					this.CalAttrByType(attrType);
				}
			}
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}
	}

	public float GetAttr(AttrType type)
	{
		return this.GetAttr(DataManager.DataPoolType.CurrentData, type);
	}

	public float ChangeAttr(AttrType attrType, OpType opType, float value)
	{
		if (attrType == AttrType.Hp || attrType == AttrType.Mp)
		{
			if (opType == OpType.Add)
			{
				this.SetAttr(attrType, this.GetAttr(attrType) + value);
			}
			else if (attrType == AttrType.Hp)
			{
				this.SetAttr(attrType, this.GetAttr(AttrType.Hp) + this.GetAttr(AttrType.HpMax) * value);
			}
			else if (attrType == AttrType.Mp)
			{
				this.SetAttr(attrType, this.GetAttr(AttrType.Mp) + this.GetAttr(AttrType.MpMax) * value);
			}
			return this.GetAttr(attrType);
		}
		Dictionary<AttrType, float> dataPool;
		if (opType == OpType.Add)
		{
			dataPool = this.GetDataPool(DataManager.DataPoolType.AddData);
		}
		else
		{
			dataPool = this.GetDataPool(DataManager.DataPoolType.MulData);
		}
		if (!dataPool.ContainsKey(attrType))
		{
			dataPool.Add(attrType, 0f);
		}
		Dictionary<AttrType, float> dictionary;
		Dictionary<AttrType, float> expr_AD = dictionary = dataPool;
		float num = dictionary[attrType];
		expr_AD[attrType] = num + value;
		return this.CalAttrByType(attrType);
	}

	public void SetHp(float value)
	{
		this.SetAttr(AttrType.Hp, value);
	}

	public void SetMp(float value)
	{
		this.SetAttr(AttrType.Mp, value);
	}

	public void SetData(DataType dataType, object value)
	{
		if (!this._dataTypeDictionary.ContainsKey(dataType))
		{
			this._dataTypeDictionary.Add(dataType, value);
		}
		else
		{
			this._dataTypeDictionary[dataType] = value;
		}
	}

	public int GetDataInt(DataType exp)
	{
		object obj = this._dataTypeDictionary[exp];
		return (int)obj;
	}

	public T GetData<T>(DataType type)
	{
		object obj;
		if (this._dataTypeDictionary.TryGetValue(type, out obj))
		{
			return (T)((object)obj);
		}
		return default(T);
	}

	public void OnLevelUpTo(int _level)
	{
		this.level = _level;
		this.CalcAllAttrs();
	}
}
