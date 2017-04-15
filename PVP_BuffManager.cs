using System;
using System.Collections.Generic;

public class PVP_BuffManager : BuffManager
{
	public override void OnUpdate(float deltaTime)
	{
		if (this.buffList.Count == 0)
		{
			return;
		}
		Dictionary<string, BuffVo>.Enumerator enumerator = this.buffList.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<string, BuffVo> current = enumerator.Current;
			if (current.Value != null)
			{
				KeyValuePair<string, BuffVo> current2 = enumerator.Current;
				current2.Value.OnUpdate(deltaTime);
			}
		}
	}
}
