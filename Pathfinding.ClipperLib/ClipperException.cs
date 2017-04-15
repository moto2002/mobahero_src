using System;

namespace Pathfinding.ClipperLib
{
	internal class ClipperException : Exception
	{
		public ClipperException(string description) : base(description)
		{
		}
	}
}
