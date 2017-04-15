using System;
using System.Collections.Generic;

public abstract class UtilCounter
{
	protected UtilType _type;

	private Dictionary<int, CounterValue> _lookingTable;

	public bool IfInit;

	public UtilCounter(UtilType type)
	{
		this._type = type;
		this._lookingTable = new Dictionary<int, CounterValue>();
	}

	public abstract void InitCounter();

	public virtual void AddValue(int uid, CounterValue t)
	{
		if (this._lookingTable.ContainsKey(uid))
		{
			this._lookingTable[uid] = t;
		}
		else
		{
			this._lookingTable.Add(uid, t);
		}
	}

	public virtual CounterValue GetValue(int uid)
	{
		CounterValue result = null;
		if (!this._lookingTable.TryGetValue(uid, out result))
		{
			result = null;
		}
		return result;
	}

	public virtual void Update()
	{
		Dictionary<int, CounterValue>.Enumerator enumerator = this._lookingTable.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<int, CounterValue> current = enumerator.Current;
			current.Value.Update();
		}
	}

	public virtual Dictionary<int, CounterValue> GetDic()
	{
		return this._lookingTable;
	}

	public virtual void Clear()
	{
		this._lookingTable.Clear();
	}
}
