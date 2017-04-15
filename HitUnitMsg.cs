using System;

public class HitUnitMsg : GameMessage
{
	private Units _atker;

	public Units atker
	{
		get
		{
			return this._atker;
		}
	}

	public HitUnitMsg(Units atker)
	{
		this._atker = atker;
		MessageManager.dispatch(this);
	}
}
