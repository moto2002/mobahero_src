using System;

public abstract class UtilData
{
	protected string _id;

	public UtilData(string id)
	{
		this._id = id;
		this.InitConfig();
	}

	protected abstract void InitConfig();
}
