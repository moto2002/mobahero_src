using System;

namespace Pathfinding
{
	public class PathNNConstraint : NNConstraint
	{
		public new static PathNNConstraint Default
		{
			get
			{
				return new PathNNConstraint
				{
					constrainArea = true
				};
			}
		}

		public virtual void SetStart(GraphNode node)
		{
			if (node != null)
			{
				this.area = (int)node.Area;
			}
			else
			{
				this.constrainArea = false;
			}
		}

		public override bool Suitable(GraphNode node)
		{
			return (!this.constrainWalkability || node.Walkable == this.walkable) && (!this.constrainArea || this.area < 0 || (ulong)node.Area == (ulong)((long)this.area)) && (!this.constrainTags || (this.tags >> (int)node.Tag & 1) != 0);
		}
	}
}
