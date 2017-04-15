using Com.Game.Module;
using Com.Game.Utils;
using MobaFrame.SkillAction;
using MobaHeros;
using MobaMessageData;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : UnitComponent
{
	public enum DataPoolType
	{
		BasicData,
		AddData,
		MulData,
		AddDataBefore,
		MulDataBefore,
		CurrentData
	}

	public int level = 1;

	protected AttrType MainAttrType;

	private float[][] _dataPool = new float[][]
	{
		new float[99],
		new float[99],
		new float[99],
		new float[99],
		new float[99],
		new float[99]
	};

	private object[] _dataObjects = new object[52];

	private PVE_DataRevertAction _pveDataRevertAction;

	public DataManager()
	{
	}

	public DataManager(Units self) : base(self)
	{
	}

	protected float[] GetDataPool(DataManager.DataPoolType type)
	{
		return this._dataPool[(int)type];
	}

	public void CopyAllData(Units target)
	{
		for (AttrType attrType = AttrType.None; attrType < AttrType.End; attrType++)
		{
			this.SetAttr(DataManager.DataPoolType.BasicData, attrType, target.data.GetAttr(DataManager.DataPoolType.BasicData, attrType));
			this.SetAttr(DataManager.DataPoolType.AddData, attrType, target.data.GetAttr(DataManager.DataPoolType.AddData, attrType));
			this.SetAttr(DataManager.DataPoolType.MulData, attrType, target.data.GetAttr(DataManager.DataPoolType.MulData, attrType));
			this.SetAttr(DataManager.DataPoolType.AddDataBefore, attrType, target.data.GetAttr(DataManager.DataPoolType.AddDataBefore, attrType));
			this.SetAttr(DataManager.DataPoolType.MulDataBefore, attrType, target.data.GetAttr(DataManager.DataPoolType.MulDataBefore, attrType));
			this.SetAttr(DataManager.DataPoolType.CurrentData, attrType, target.data.GetAttr(DataManager.DataPoolType.CurrentData, attrType));
		}
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
			return value;
		case AttrType.Agile:
			this.SetAttr(DataManager.DataPoolType.CurrentData, attrType, value);
			return value;
		case AttrType.Intelligence:
			this.SetAttr(DataManager.DataPoolType.CurrentData, attrType, value);
			return value;
		case AttrType.HpMax:
		{
			float attr = this.GetAttr(AttrType.HpMax);
			float attr2 = this.GetAttr(AttrType.Hp);
			if (attr == 0f)
			{
				this.SetAttr(DataManager.DataPoolType.CurrentData, AttrType.HpMax, value);
				this.SetAttr(DataManager.DataPoolType.CurrentData, AttrType.Hp, value);
			}
			else
			{
				this.SetAttr(DataManager.DataPoolType.CurrentData, AttrType.HpMax, value);
				this.SetAttr(DataManager.DataPoolType.CurrentData, AttrType.Hp, attr2 * (value / attr));
			}
			return value;
		}
		case AttrType.MpMax:
		{
			float attr3 = this.GetAttr(AttrType.MpMax);
			float attr4 = this.GetAttr(AttrType.Mp);
			if (attr3 == 0f)
			{
				this.SetAttr(DataManager.DataPoolType.CurrentData, AttrType.MpMax, value);
				this.SetAttr(DataManager.DataPoolType.CurrentData, AttrType.Mp, value);
			}
			else
			{
				this.SetAttr(DataManager.DataPoolType.CurrentData, AttrType.MpMax, value);
			}
			return value;
		}
		}
		return this.SetAttr(DataManager.DataPoolType.CurrentData, attrType, value);
	}

	protected float SetAttr(DataManager.DataPoolType dataPoolType, AttrType attrType, float value)
	{
		if (CharacterDataMgr.instance != null && dataPoolType == DataManager.DataPoolType.CurrentData)
		{
			AttrNumberData attrNumberData = CharacterDataMgr.instance.GetAttrNumberData(attrType);
			if (attrNumberData != null)
			{
				if (value > attrNumberData.m_fMaxValue)
				{
					value = attrNumberData.m_fMaxValue;
				}
				else if (value < attrNumberData.m_fMinValue)
				{
					value = attrNumberData.m_fMinValue;
				}
			}
		}
		this._dataPool[(int)dataPoolType][(int)attrType] = value;
		if (dataPoolType == DataManager.DataPoolType.CurrentData && this.self.isPlayer)
		{
			switch (attrType)
			{
			case AttrType.Attack:
			case AttrType.Armor:
			case AttrType.MoveSpeed:
			case AttrType.AttackSpeed:
				goto IL_AA;
			case AttrType.DodgeProp:
				IL_90:
				switch (attrType)
				{
				case AttrType.MagicResist:
				case AttrType.MagicPower:
					goto IL_AA;
				}
				return value;
			}
			goto IL_90;
			IL_AA:
			MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)25057, new MsgData_AttrChangeData
			{
				nType = (int)attrType,
				fValue = value
			}, 0f);
			MobaMessageManager.ExecuteMsg(message);
		}
		return value;
	}

	public float GetAttr(DataManager.DataPoolType DataPoolType, AttrType type)
	{
		return this._dataPool[(int)DataPoolType][(int)type];
	}

	protected float CalAttrByType(AttrType type)
	{
		float attr = this.GetAttr(DataManager.DataPoolType.BasicData, type);
		float growthByAttr = this.GetGrowthByAttr(type);
		float num = this.CalL2ByL1(type);
		float attr2 = this.GetAttr(DataManager.DataPoolType.AddData, type);
		float attr3 = this.GetAttr(DataManager.DataPoolType.MulData, type);
		float attr4 = this.GetAttr(DataManager.DataPoolType.AddDataBefore, type);
		float attr5 = this.GetAttr(DataManager.DataPoolType.MulDataBefore, type);
		float value;
		if (type == AttrType.Shield)
		{
			value = attr + growthByAttr + num + attr2 + this.GetAttr(DataManager.DataPoolType.CurrentData, AttrType.HpMax) * (attr3 + attr5) + attr4;
			value = Mathf.Clamp(value, 0f, 3.40282347E+38f);
		}
		else if (type == AttrType.AttackSpeed)
		{
			value = (attr + growthByAttr + attr2) * (1f + attr3 + num + attr5) + attr4;
		}
		else
		{
			value = (attr + growthByAttr + num + attr2) * (1f + attr3 + attr5) + attr4;
		}
		return this.SetAttr(type, value);
	}

	private float GetGrowthByAttr(AttrType type)
	{
		if (this.self == null)
		{
			return 0f;
		}
		if (type == AttrType.Agile)
		{
			return this.GetAttr(AttrType.AgileGroth) * (float)(this.self.level - 1);
		}
		if (type == AttrType.Power)
		{
			return this.GetAttr(AttrType.PowerGroth) * (float)(this.self.level - 1);
		}
		if (type == AttrType.Intelligence)
		{
			return this.GetAttr(AttrType.IntelligenceGroth) * (float)(this.self.level - 1);
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
		case AttrType.DodgeProp:
		case AttrType.MoveSpeed:
		case (AttrType)11:
			IL_2E:
			if (type != AttrType.MagicResist)
			{
				return 0f;
			}
			return this.GetData<float>(DataType.ResistanceParam) * (float)(this.GetData<int>(DataType.Level) - 1);
		case AttrType.AttackSpeed:
			return this.GetAttr(DataManager.DataPoolType.CurrentData, AttrType.Agile) * 0.01f;
		case AttrType.HpMax:
			return this.GetAttr(DataManager.DataPoolType.CurrentData, AttrType.Power) * 20f;
		case AttrType.MpMax:
			return this.GetAttr(DataManager.DataPoolType.CurrentData, AttrType.Intelligence) * 10f;
		case AttrType.HpRestore:
			return this.GetAttr(DataManager.DataPoolType.CurrentData, AttrType.Power) * 0.05f;
		}
		goto IL_2E;
	}

	protected void ClearDataPool()
	{
		Array.Clear(this.GetDataPool(DataManager.DataPoolType.AddData), 0, 99);
		Array.Clear(this.GetDataPool(DataManager.DataPoolType.MulData), 0, 99);
		Array.Clear(this.GetDataPool(DataManager.DataPoolType.BasicData), 0, 99);
		Array.Clear(this.GetDataPool(DataManager.DataPoolType.AddDataBefore), 0, 99);
		Array.Clear(this.GetDataPool(DataManager.DataPoolType.MulDataBefore), 0, 99);
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

	protected void MoveCurrentToBaseAndClearAddMul()
	{
	}

	public float GetAttr(AttrType type)
	{
		if (type == AttrType.ExtraAttack)
		{
			float basicAttrInBattle = this.GetBasicAttrInBattle(AttrType.Attack);
			return this.GetAttr(AttrType.Attack) - basicAttrInBattle;
		}
		return this.GetAttr(DataManager.DataPoolType.CurrentData, type);
	}

	public float GetBasicAttrInBattle(AttrType type)
	{
		float attr = this.GetAttr(DataManager.DataPoolType.BasicData, type);
		float growthByAttr = this.GetGrowthByAttr(type);
		float num = this.CalL2ByL1(type);
		float result;
		if (type == AttrType.Shield)
		{
			result = attr + growthByAttr + num + this.GetAttr(DataManager.DataPoolType.CurrentData, AttrType.HpMax);
		}
		else if (type == AttrType.AttackSpeed)
		{
			result = (attr + growthByAttr) * (1f + num);
		}
		else
		{
			result = attr + growthByAttr + num;
		}
		return result;
	}

	public float ChangeAttr(AttrType attrType, OpType opType, float value)
	{
		if (attrType == AttrType.Hp || attrType == AttrType.Mp)
		{
			if (opType == OpType.Add)
			{
				this.SetAttr(attrType, this.GetAttr(attrType) + value);
			}
			else if (opType == OpType.Mul)
			{
				if (attrType == AttrType.Hp)
				{
					this.SetAttr(attrType, this.GetAttr(AttrType.Hp) + this.GetAttr(AttrType.HpMax) * value);
				}
				else if (attrType == AttrType.Mp)
				{
					this.SetAttr(attrType, this.GetAttr(AttrType.Mp) + this.GetAttr(AttrType.MpMax) * value);
				}
			}
			return this.GetAttr(attrType);
		}
		float[] dataPool;
		if (opType == OpType.Add)
		{
			dataPool = this.GetDataPool(DataManager.DataPoolType.AddData);
		}
		else if (opType == OpType.Mul)
		{
			dataPool = this.GetDataPool(DataManager.DataPoolType.MulData);
		}
		else if (opType == OpType.AddBefore)
		{
			dataPool = this.GetDataPool(DataManager.DataPoolType.AddDataBefore);
		}
		else
		{
			dataPool = this.GetDataPool(DataManager.DataPoolType.MulDataBefore);
		}
		dataPool[(int)attrType] += value;
		if (attrType == AttrType.Shield)
		{
			dataPool[(int)attrType] = Mathf.Clamp(dataPool[(int)attrType], 0f, 3.40282347E+38f);
		}
		return this.CalAttrByType(attrType);
	}

	public void SetHp(float value)
	{
		this.self.RefreshSyncState();
		this.SetAttr(AttrType.Hp, value);
	}

	public void SetMp(float value)
	{
		this.SetAttr(AttrType.Mp, value);
	}

	public void SetMaxHp(float value)
	{
		this.SetAttr(AttrType.HpMax, value);
	}

	public void SetMaxMp(float value)
	{
		this.SetAttr(AttrType.MpMax, value);
	}

	public void SetDamageMultiple(float value)
	{
		this.SetAttr(AttrType.DamageMultiple, value);
	}

	public void SetAttrVal(DataUpdateInfo data)
	{
		if (data.dataKeys == null)
		{
			ClientLogger.Error("data.dataKeys == null");
			return;
		}
		for (int i = 0; i < data.dataKeys.Count; i++)
		{
			this.SetAttr((AttrType)data.dataKeys[i], data.dataValues[i]);
		}
	}

	public void SetAttrVal(List<short> keys, List<float> values)
	{
		if (keys == null || values == null)
		{
			return;
		}
		for (int i = 0; i < keys.Count; i++)
		{
			this.SetAttr((AttrType)keys[i], values[i]);
			if (keys[i] == 2 && this.self.isPlayer)
			{
				Singleton<SkillView>.Instance.CheckIconToGrayByCanUse(null, -1);
			}
		}
	}

	public void SetData(DataType dataType, object value)
	{
		this._dataObjects[dataType - DataType.DataType_Begin] = value;
		if (dataType == DataType.Level)
		{
			this.OnLevelUpTo((int)value);
		}
	}

	public int GetDataInt(DataType exp)
	{
		object obj = this._dataObjects[exp - DataType.DataType_Begin];
		if (obj == null)
		{
			return 0;
		}
		return (int)obj;
	}

	public T GetData<T>(DataType type)
	{
		object obj = this._dataObjects[type - DataType.DataType_Begin];
		if (obj == null)
		{
			return default(T);
		}
		return (T)((object)obj);
	}

	public virtual void CollectGameItemAttr()
	{
	}

	public virtual void OnLevelUpTo(int _level)
	{
		this.level = _level;
		this.CollectGameItemAttr();
	}

	public void RestoreData(float delta = 0f)
	{
		string name = this.self.name;
		if (name == "Sumo+BL_TEAM")
		{
			return;
		}
		if (this._pveDataRevertAction == null)
		{
			this._pveDataRevertAction = ActionManager.DataRevert(this.self);
		}
		else
		{
			this._pveDataRevertAction.Play();
		}
	}

	protected void GetKeyData(ref List<int> dataKeys, ref List<float> dataValues)
	{
		dataKeys.Clear();
		dataValues.Clear();
		float item = this.self.hp;
		float item2 = this.self.mp;
		if (!this.self.isLive)
		{
			item = 0f;
			item2 = 0f;
		}
		dataKeys.Add(1);
		dataValues.Add(item);
		dataKeys.Add(2);
		dataValues.Add(item2);
	}

	public override void OnDeath(Units attacker)
	{
	}
}
