using MobaHeros;
using System;
using UnityEngine;

public class SkillPointerManager
{
	private static ResourceHandleWrapper<CirclePointer> _circlePointer;

	private static ResourceHandleWrapper<LinePointer> _linePointer;

	public static ResourceHandleWrapper<SkillPointer> CreateSkillPointer(Units self, string skill_id, float distance)
	{
		ResourceHandle resourceHandle = null;
		Skill skillById = self.getSkillById(skill_id);
		int num = skillById.castingType;
		if (skillById.data.IsUseSkillPointer)
		{
			num = (int)skillById.data.SkillPointerType;
		}
		switch (num)
		{
		case 1:
			resourceHandle = MapManager.Instance.SpawnResourceHandle("LinePointer", null, 0);
			break;
		case 2:
			resourceHandle = MapManager.Instance.SpawnResourceHandle("CirclePointer", null, 0);
			break;
		case 3:
		{
			float num2 = 45f;
			if (skillById.data.IsUseSkillPointer)
			{
				num2 = skillById.data.SectorPointerAngle;
			}
			if (Math.Abs(num2 - 20f) < 1f)
			{
				resourceHandle = MapManager.Instance.SpawnResourceHandle("SectorPointer20", null, 0);
			}
			else if (Math.Abs(num2 - 30f) < 1f)
			{
				resourceHandle = MapManager.Instance.SpawnResourceHandle("SectorPointer30", null, 0);
			}
			else if (Math.Abs(num2 - 45f) < 1f)
			{
				resourceHandle = MapManager.Instance.SpawnResourceHandle("SectorPointer45", null, 0);
			}
			else if (Math.Abs(num2 - 60f) < 1f)
			{
				resourceHandle = MapManager.Instance.SpawnResourceHandle("SectorPointer60", null, 0);
			}
			break;
		}
		default:
			resourceHandle = MapManager.Instance.SpawnResourceHandle("CirclePointer", null, 0);
			break;
		}
		if (resourceHandle == null)
		{
			return null;
		}
		Transform raw = resourceHandle.Raw;
		raw.gameObject.layer = LayerMask.NameToLayer("SkillPointer");
		EffectFollowBehavior effectFollowBehavior = raw.gameObject.GetComponent<EffectFollowBehavior>();
		if (effectFollowBehavior == null)
		{
			effectFollowBehavior = raw.gameObject.AddComponent<EffectFollowBehavior>();
		}
		effectFollowBehavior.SetFollowObj(self.transform, Vector3.up / 5f);
		raw.position = self.transform.position + Vector3.up / 5f;
		raw.localRotation = Quaternion.Euler(Vector3.zero);
		SkillPointer component = raw.GetComponent<SkillPointer>();
		component.theurgist = self;
		component.CreateSkillPointer(skillById.skillMainId, skillById.distance, skillById.effect_range1, skillById.effect_range2);
		ResourceHandleWrapper<SkillPointer> resourceHandleWrapper = new ResourceHandleWrapper<SkillPointer>(resourceHandle);
		self.mSkillPointer = resourceHandleWrapper;
		return resourceHandleWrapper;
	}

	private static ResourceHandleWrapper<LinePointer> CreateLinePointer(Units self)
	{
		ResourceHandle resourceHandle = MapManager.Instance.SpawnResourceHandle("LinePointer", null, 0);
		Transform raw = resourceHandle.Raw;
		raw.gameObject.SetActive(false);
		raw.gameObject.layer = LayerMask.NameToLayer("SkillPointer");
		EffectFollowBehavior effectFollowBehavior = raw.gameObject.GetComponent<EffectFollowBehavior>();
		if (effectFollowBehavior == null)
		{
			effectFollowBehavior = raw.gameObject.AddComponent<EffectFollowBehavior>();
		}
		effectFollowBehavior.SetFollowObj(self.transform, Vector3.up / 5f);
		raw.position = Vector3.zero;
		raw.localRotation = Quaternion.Euler(Vector3.up);
		LinePointer component = raw.GetComponent<LinePointer>();
		component.theurgist = self;
		component.followBehavior = effectFollowBehavior;
		return new ResourceHandleWrapper<LinePointer>(resourceHandle);
	}

	private static ResourceHandleWrapper<CirclePointer> CreateCirclePointer(Units self)
	{
		ResourceHandle resourceHandle = MapManager.Instance.SpawnResourceHandle("CirclePointer", null, 0);
		Transform raw = resourceHandle.Raw;
		raw.gameObject.SetActive(false);
		raw.gameObject.layer = LayerMask.NameToLayer("SkillPointer");
		EffectFollowBehavior effectFollowBehavior = raw.gameObject.GetComponent<EffectFollowBehavior>();
		if (effectFollowBehavior == null)
		{
			effectFollowBehavior = raw.gameObject.AddComponent<EffectFollowBehavior>();
		}
		effectFollowBehavior.SetFollowObj(self.transform, Vector3.up / 5f);
		raw.position = Vector3.zero;
		raw.localRotation = Quaternion.Euler(Vector3.up);
		CirclePointer component = raw.GetComponent<CirclePointer>();
		component.theurgist = self;
		component.followBehavior = effectFollowBehavior;
		return new ResourceHandleWrapper<CirclePointer>(resourceHandle);
	}

	public static void ShowLinePointer(Units self, Vector3 direct, float width = 0f, float length = 0f)
	{
		if (SkillPointerManager._linePointer == null)
		{
			SkillPointerManager._linePointer = SkillPointerManager.CreateLinePointer(self);
		}
		LinePointer component = SkillPointerManager._linePointer.Component;
		if (!component.isShow)
		{
			component.Show();
		}
		component.followBehavior.isActive = true;
		if (width != 0f && length != 0f)
		{
			component.SetEffectRange(width, length);
		}
		component.SetDirect(direct);
	}

	public static void SetLinePointerDirect(Vector3 direct)
	{
		if (!SkillPointerManager._linePointer.IsValid<LinePointer>())
		{
			return;
		}
		LinePointer component = SkillPointerManager._linePointer.Component;
		if (!component.isShow)
		{
			return;
		}
		component.SetDirect(direct);
	}

	public static void HideLinePointer()
	{
		if (SkillPointerManager._linePointer.IsValid<LinePointer>())
		{
			SkillPointerManager._linePointer.Component.Hide();
		}
	}

	public static void ShowCirclePointer(Units self, float inAttackRange, float inEffectRange)
	{
		if (SkillPointerManager._circlePointer == null)
		{
			SkillPointerManager._circlePointer = SkillPointerManager.CreateCirclePointer(self);
		}
		CirclePointer component = SkillPointerManager._circlePointer.Component;
		if (!component.isShow)
		{
			component.Show();
		}
		component.followBehavior.isActive = true;
		component.SetEffectRange(inEffectRange);
		component.SetAttackRange(inAttackRange);
	}

	public static void HideCirclePointer()
	{
		if (SkillPointerManager._circlePointer.IsValid<CirclePointer>())
		{
			SkillPointerManager._circlePointer.Component.Hide();
		}
	}
}
