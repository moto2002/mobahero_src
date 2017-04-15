using System;
using UnityEngine;

public class AnimEventArgs
{
	public AnimatorStateInfo stateInfo;

	public ActionState state;

	public int action_index;

	public AnimEventArgs(AnimatorStateInfo stateInfo, int action_index, ActionState state)
	{
		this.stateInfo = stateInfo;
		this.state = state;
		this.action_index = action_index;
	}
}
