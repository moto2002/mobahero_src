using System;

namespace MobaHeros
{
	public interface IDumpable : IDisposable
	{
		void Dump();
	}
}
