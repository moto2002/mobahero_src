using System;

public class AnimationRelation
{
	public string tag;

	public AnimationEventType type;

	public int action_index;

	public AnimationRelation(string tag, AnimationEventType type, int action_index)
	{
		this.tag = tag;
		this.type = type;
		this.action_index = action_index;
	}
}
