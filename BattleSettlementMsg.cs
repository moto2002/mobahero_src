using System;

public class BattleSettlementMsg : GameMessage
{
	private object settlementState;

	public BattleSettlementMsg(object _settlementState)
	{
		this.settlementState = _settlementState;
		MessageManager.dispatch(this);
	}

	public object GetSettlementState()
	{
		return this.settlementState;
	}
}
