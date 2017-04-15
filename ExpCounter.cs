using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class ExpCounter : UtilCounter
{
	public ExpCounter(UtilType type) : base(type)
	{
	}

	public override void InitCounter()
	{
		List<Units> mapUnits = MapManager.Instance.GetMapUnits(TargetTag.Hero);
		if (mapUnits != null)
		{
			foreach (Units current in mapUnits)
			{
				this.AddValue(current.unique_id, new ExpValue(current.unique_id, 0f));
			}
		}
		this.IfInit = true;
	}

	[DebuggerHidden]
	private IEnumerator DelayInit()
	{
		return new ExpCounter.<DelayInit>c__Iterator1B7();
	}
}
