using System;
using System.Runtime.InteropServices;

namespace Pathfinding.Ionic.Zip
{
	[Guid("ebc25cf6-9120-4283-b972-0e5520d00008")]
	public class SfxGenerationException : ZipException
	{
		public SfxGenerationException()
		{
		}

		public SfxGenerationException(string message) : base(message)
		{
		}
	}
}
