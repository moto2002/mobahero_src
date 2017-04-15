using System;
using System.Collections.Generic;

public interface IPlay
{
	List<EventDelegate> OnEnd
	{
		get;
	}

	void Begin();
}
