using Com.Game.Data;
using Com.Game.Manager;
using System;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class PerformData
	{
		public string performId;

		public SysSkillPerformVo config;

		public AnimationType action_type;

		public int action_index;

		public PerformType effect_type;

		public float effectParam1;

		public float effectParam2;

		public float effectParam3;

		public string effectStrParam1;

		public float[] effect_pos_offset;

		public float offset_x;

		public float offset_y;

		public float offset_z;

		public float[] effect_rotation_offset;

		public float offset_rx;

		public float offset_ry;

		public float offset_rz;

		public bool body_dispear;

		public bool body_dissolve;

		public bool body_destroy;

		public bool isBeInterruptThenDestroy = true;

		public bool isDeadThenDestroy = true;

		public bool isUseCasterRot = true;

		public bool isUsePool = true;

		public bool isForceDisplay;

		public bool useCollider;

		public ColliderRangeType colliderRangeType;

		public float colliderParam1;

		public float colliderParam2;

		public bool isCloneDmage;

		public float particleClose_time;

		public bool isEndAction = true;

		public bool isLoopSound;

		public bool isDamageColliderFollow = true;

		public bool isDamageColliderFollowUnit;

		public float eyeRange;

		public TargetTag performTagType = TargetTag.All;

		public bool isAffectWeapon;

		public int weaponPosType;

		public ColliderAnchorType colliderAnchorType;

		public string endPerformId;

		public PerformData(string perform_id)
		{
			this.performId = perform_id;
			this.config = BaseDataMgr.instance.GetDataById<SysSkillPerformVo>(perform_id);
			if (this.config == null)
			{
				Debug.LogError("Error performID=" + this.performId);
				return;
			}
			this.Parse(this.config);
		}

		public PerformData(string perform_id, SysSkillPerformVo perform_vo)
		{
			this.performId = perform_id;
			this.config = perform_vo;
			this.Parse(this.config);
		}

		private void Parse(SysSkillPerformVo perform_vo)
		{
			if (perform_vo.action_id != "[]")
			{
				string[] stringValue = StringUtils.GetStringValue(perform_vo.action_id, '_');
				if (stringValue != null)
				{
					this.action_type = this.GetActionType(stringValue[0]);
					if (stringValue.Length > 1)
					{
						this.action_index = int.Parse(stringValue[1]);
					}
				}
			}
			if (perform_vo.effect_type != "[]")
			{
				string[] array = StringUtils.SplitVoString(perform_vo.effect_type, "|");
				this.effect_type = (PerformType)int.Parse(array[0]);
				this.GetPerformParam(array);
			}
			this.body_dispear = (perform_vo.body_dispear == 1);
			this.body_dissolve = (perform_vo.body_dissolve == 1);
			this.body_destroy = (perform_vo.body_destroy == 1);
			if (StringUtils.CheckValid(perform_vo.use_collider))
			{
				string[] array2 = StringUtils.SplitVoString(perform_vo.use_collider, "|");
				this.useCollider = (int.Parse(array2[0]) == 1);
				this.colliderRangeType = (ColliderRangeType)((array2.Length <= 1) ? 0 : int.Parse(array2[1]));
				this.colliderParam1 = ((array2.Length <= 2) ? 0f : float.Parse(array2[2]));
				this.colliderParam2 = ((array2.Length <= 3) ? 0f : float.Parse(array2[3]));
			}
			if (perform_vo.effect_pos_offset != "[]")
			{
				this.effect_pos_offset = StringUtils.GetStringToFloat(perform_vo.effect_pos_offset, '|');
				this.offset_x = ((this.effect_pos_offset == null) ? 0f : this.effect_pos_offset[0]);
				this.offset_y = ((this.effect_pos_offset == null || this.effect_pos_offset.Length <= 1) ? 0f : this.effect_pos_offset[1]);
				this.offset_z = ((this.effect_pos_offset == null || this.effect_pos_offset.Length <= 2) ? 0f : this.effect_pos_offset[2]);
			}
			if (perform_vo.effect_rotation_offset != "[]")
			{
				this.effect_rotation_offset = StringUtils.GetStringToFloat(perform_vo.effect_rotation_offset, '|');
				this.offset_rx = ((this.effect_rotation_offset == null) ? 0f : this.effect_rotation_offset[0]);
				this.offset_ry = ((this.effect_rotation_offset == null || this.effect_rotation_offset.Length <= 1) ? 0f : this.effect_rotation_offset[1]);
				this.offset_rz = ((this.effect_rotation_offset == null || this.effect_rotation_offset.Length <= 2) ? 0f : this.effect_rotation_offset[2]);
			}
			this.endPerformId = "[]";
			this.isBeInterruptThenDestroy = true;
			this.isDeadThenDestroy = true;
			this.isUseCasterRot = true;
			this.isUsePool = true;
			this.particleClose_time = 0f;
			this.isCloneDmage = false;
			this.isEndAction = true;
			this.isLoopSound = false;
			this.isDamageColliderFollow = true;
			this.isDamageColliderFollowUnit = false;
			this.performTagType = TargetTag.All;
			this.isAffectWeapon = false;
			this.weaponPosType = 0;
			this.isForceDisplay = false;
			if (StringUtils.CheckValid(perform_vo.extra_param))
			{
				string[] stringValue2 = StringUtils.GetStringValue(perform_vo.extra_param, ',');
				for (int i = 0; i < stringValue2.Length; i++)
				{
					if (StringUtils.CheckValid(stringValue2[i]))
					{
						string[] array3 = StringUtils.SplitVoString(stringValue2[i], "|");
						if (array3[0] == "1")
						{
							this.endPerformId = array3[1];
						}
						else if (array3[0] == "2")
						{
							if (array3[1] == "1")
							{
								this.isBeInterruptThenDestroy = true;
							}
							else if (array3[1] == "0")
							{
								this.isBeInterruptThenDestroy = false;
							}
						}
						else if (array3[0] == "3")
						{
							if (array3[1] == "1")
							{
								this.isUseCasterRot = true;
							}
							else if (array3[1] == "0")
							{
								this.isUseCasterRot = false;
							}
						}
						else if (array3[0] == "4")
						{
							if (array3[1] == "1")
							{
								this.isDeadThenDestroy = true;
							}
							else if (array3[1] == "0")
							{
								this.isDeadThenDestroy = false;
							}
						}
						else if (array3[0] == "5")
						{
							if (array3[1] == "1")
							{
								this.isUsePool = true;
							}
							else if (array3[1] == "0")
							{
								this.isUsePool = false;
							}
						}
						else if (array3[0] == "6")
						{
							this.particleClose_time = float.Parse(array3[1]);
						}
						else if (!(array3[0] == "7"))
						{
							if (array3[0] == "8")
							{
								if (array3[1] == "1")
								{
									this.isCloneDmage = true;
								}
								else if (array3[1] == "0")
								{
									this.isCloneDmage = false;
								}
							}
							else if (array3[0] == "9")
							{
								if (array3[1] == "1")
								{
									this.isEndAction = true;
								}
								else if (array3[1] == "0")
								{
									this.isEndAction = false;
								}
							}
							else if (array3[0] == "10")
							{
								if (array3[1] == "1")
								{
									this.isLoopSound = true;
								}
								else if (array3[1] == "0")
								{
									this.isLoopSound = false;
								}
							}
							else if (array3[0] == "11")
							{
								if (array3[1] == "1")
								{
									this.isDamageColliderFollow = true;
								}
								else if (array3[1] == "0")
								{
									this.isDamageColliderFollow = false;
								}
							}
							else if (array3[0] == "12")
							{
								if (array3[1] == "1")
								{
									this.isDamageColliderFollowUnit = true;
								}
								else if (array3[1] == "0")
								{
									this.isDamageColliderFollowUnit = false;
								}
							}
							else if (array3[0] == "13")
							{
								this.performTagType = (TargetTag)int.Parse(array3[1]);
							}
							else if (array3[0] == "14")
							{
								this.isAffectWeapon = true;
								if (array3.Length > 1)
								{
									int num = 0;
									if (int.TryParse(array3[1], out num))
									{
										this.weaponPosType = num;
									}
								}
							}
							else if (array3[0] == "15")
							{
								this.colliderAnchorType = ColliderAnchorType.centor;
								if (array3.Length > 1)
								{
									int num2 = 0;
									if (int.TryParse(array3[1], out num2) && num2 == 1)
									{
										this.colliderAnchorType = ColliderAnchorType.bottom;
									}
								}
							}
							else if (array3[0] == "16")
							{
								this.eyeRange = float.Parse(array3[1]);
							}
							else if (array3[0] == "22")
							{
								if (array3[1] == "0")
								{
									this.isForceDisplay = false;
								}
								else if (array3[1] == "1")
								{
									this.isForceDisplay = true;
								}
							}
						}
					}
				}
			}
		}

		private AnimationType GetActionType(string action)
		{
			switch (action)
			{
			case "attack":
				return AnimationType.ComboAttack;
			case "conjure":
				return AnimationType.Conjure;
			case "breath":
				return AnimationType.Breath;
			}
			return AnimationType.None;
		}

		public bool IsEmpty()
		{
			return this.config.effect_id == "Null" || this.config.effect_id == "[]";
		}

		public bool IsUnuseable()
		{
			return this.config.effect_id == "[]";
		}

		public bool IsDestroyByTime()
		{
			return true;
		}

		public void GetPerformParam(string[] types)
		{
			this.effectParam1 = 0f;
			this.effectParam2 = 0f;
			this.effectParam3 = 0f;
			switch (int.Parse(types[0]))
			{
			case 2:
				this.effectParam1 = ((types.Length <= 1) ? 1f : float.Parse(types[1]));
				this.effectParam2 = ((types.Length <= 2) ? 0f : float.Parse(types[2]));
				this.effectParam3 = ((types.Length <= 3) ? 0f : float.Parse(types[3]));
				break;
			case 3:
				this.effectParam1 = ((types.Length <= 1) ? 1f : float.Parse(types[1]));
				this.effectParam2 = ((types.Length <= 2) ? 1f : float.Parse(types[2]));
				break;
			case 4:
			case 16:
				this.effectParam1 = float.Parse(types[1]);
				this.effectParam2 = ((types.Length <= 2) ? 0f : float.Parse(types[2]));
				this.effectParam3 = ((types.Length <= 3) ? 0f : float.Parse(types[3]));
				break;
			case 5:
				this.effectParam1 = float.Parse(types[1]);
				break;
			case 8:
				this.effectParam1 = ((types.Length <= 1) ? 60f : float.Parse(types[1]));
				this.effectParam2 = ((types.Length <= 2) ? 0f : float.Parse(types[2]));
				break;
			case 9:
				this.effectParam1 = float.Parse(types[1]);
				this.effectParam2 = ((types.Length <= 2) ? 60f : float.Parse(types[2]));
				this.effectParam3 = ((types.Length <= 3) ? 2f : float.Parse(types[3]));
				break;
			case 10:
				this.effectParam1 = ((types.Length <= 1) ? 0f : float.Parse(types[1]));
				this.effectParam2 = ((types.Length <= 2) ? 0f : float.Parse(types[2]));
				break;
			case 11:
				this.effectParam1 = ((types.Length <= 1) ? 0f : float.Parse(types[1]));
				this.effectParam2 = ((types.Length <= 2) ? 0f : float.Parse(types[2]));
				break;
			case 13:
				this.effectParam1 = ((types.Length <= 1) ? 0f : float.Parse(types[1]));
				this.effectParam2 = ((types.Length <= 2) ? 0f : float.Parse(types[2]));
				break;
			case 14:
				this.effectParam1 = ((types.Length <= 1) ? 2f : float.Parse(types[1]));
				break;
			case 17:
				this.effectParam1 = ((types.Length <= 1) ? 1f : float.Parse(types[1]));
				this.effectParam2 = ((types.Length <= 2) ? 5f : float.Parse(types[2]));
				break;
			case 18:
				this.effectParam1 = ((types.Length <= 1) ? 60f : float.Parse(types[1]));
				this.effectStrParam1 = ((types.Length <= 2) ? string.Empty : types[2]);
				this.effectParam2 = ((types.Length <= 3) ? 1f : float.Parse(types[3]));
				break;
			case 19:
				this.effectParam1 = ((types.Length <= 1) ? 60f : float.Parse(types[1]));
				this.effectParam2 = ((types.Length <= 2) ? 0.1f : float.Parse(types[2]));
				this.effectParam3 = ((types.Length <= 3) ? 2.14748365E+09f : float.Parse(types[3]));
				break;
			}
		}
	}
}
