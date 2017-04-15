using System;
using UnityEngine;

namespace Pathfinding
{
	public class FleePath : RandomPath
	{
		[Obsolete("Please use the Construct method instead")]
		public FleePath(Vector3 start, Vector3 avoid, int length, OnPathDelegate callbackDelegate = null) : base(start, length, callbackDelegate)
		{
			throw new Exception("Please use the Construct method instead");
		}

		public FleePath()
		{
		}

		public static FleePath Construct(Vector3 start, Vector3 avoid, int searchLength, OnPathDelegate callback = null)
		{
			FleePath path = PathPool<FleePath>.GetPath();
			path.Setup(start, avoid, searchLength, callback);
			return path;
		}

		protected void Setup(Vector3 start, Vector3 avoid, int searchLength, OnPathDelegate callback)
		{
			base.Setup(start, searchLength, callback);
			this.aim = avoid - start;
			this.aim *= 10f;
			this.aim = start - this.aim;
		}

		protected override void Recycle()
		{
			PathPool<FleePath>.Recycle(this);
		}
	}
}
