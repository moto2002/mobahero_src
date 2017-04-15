using MobaTools.Prefab;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EditorLinkPerform : EditorPerform
{
	private LineRenderer[] lineRenders;

	protected LinkType mLinkType;

	protected BoneAnchorType mBoneType;

	public override void Start(string performID, EditorUnit unit, List<EditorUnit> targets, Vector3? pos, EditorSkill skill)
	{
		base.Start(performID, unit, targets, pos, skill);
		this.mLinkType = (LinkType)this.data.effectParam1;
		this.mBoneType = (BoneAnchorType)this.data.effectParam2;
	}

	protected override void OnPlayEffect()
	{
		this.lineRenders = this.trans.GetComponentsInChildren<LineRenderer>();
	}

	protected override void Update()
	{
		if (this.targets == null)
		{
			return;
		}
		if (this.trans == null)
		{
			return;
		}
		if (this.targets.Count == 0)
		{
			return;
		}
		if (this.lineRenders == null)
		{
			return;
		}
		for (int i = 0; i < this.lineRenders.Length; i++)
		{
			this.SetLinkTargets(this.lineRenders[i], this.targets);
		}
	}

	public virtual void SetLinkTargets(LineRenderer lineRenderer, List<EditorUnit> targets)
	{
		lineRenderer.SetVertexCount(targets.Count + 1);
		if (this.mLinkType == LinkType.LinkWithTargetAndSelf)
		{
			lineRenderer.SetPosition(0, this.unit.Unit.GetBonePos(this.mBoneType));
		}
		else
		{
			lineRenderer.SetPosition(0, targets[0].Unit.GetCenter());
		}
		for (int i = 0; i < targets.Count; i++)
		{
			if (targets[i] != null)
			{
				lineRenderer.SetPosition(i + 1, targets[i].Unit.GetCenter());
			}
		}
	}
}
