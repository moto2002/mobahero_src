using Assets.Scripts.Model;
using MobaHeros;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Character.Control
{
	public class ManualControlTarget
	{
		private ManualController controller;

		private Units self;

		private Vector3 touchPoint;

		private Vector3 groundPoint;

		private Units targetUnit;

		private GameObject targetObj;

		private ResourceHandle skillFlag;

		private ResourceHandle targetIcon;

		private ResourceHandle targetIconBackup;

		private bool validGroundPoint;

		private bool validTargetGo;

		private bool validTargetUnit;

		private IList<TeamType> enemyType;

		private float dpi;

		private bool gotDpi;

		public bool ValidGroudPoint
		{
			get
			{
				return this.validGroundPoint;
			}
		}

		public bool ValidTargetGo
		{
			get
			{
				return this.validTargetGo;
			}
		}

		public bool ValidTargetUnit
		{
			get
			{
				return this.validTargetUnit;
			}
		}

		public Vector3 GroundPoint
		{
			get
			{
				return this.groundPoint;
			}
		}

		public GameObject Target
		{
			get
			{
				return this.targetObj;
			}
		}

		public Units TargetUnit
		{
			get
			{
				return this.targetUnit;
			}
		}

		public ManualControlTarget(ManualController c)
		{
			this.controller = c;
			this.self = c.ControlUnit;
			this.enemyType = TeamManager.GetEnemyTeams(this.self.TeamType);
		}

		public void clearState()
		{
		}

		public void ClearTarget()
		{
			this.targetUnit = null;
		}

		public bool UpdateTargetAll(Vector3 point, SkillTargetCamp skillTargetCamp)
		{
			this.touchPoint = point;
			this.validGroundPoint = false;
			this.validTargetGo = false;
			this.validTargetUnit = false;
			if (!this.GetBattleShopPoint())
			{
				if (this.GetGroundPoint())
				{
					float radius = this.GetDPI() * 0.5f;
					if (this.GetBesetTargetAll(radius, skillTargetCamp))
					{
						this.RightTargetAll();
					}
				}
			}
			return this.ValidGroudPoint || this.ValidTargetUnit;
		}

		public bool UpdateTarget(Vector3 point)
		{
			this.touchPoint = point;
			this.validGroundPoint = false;
			this.validTargetGo = false;
			this.validTargetUnit = false;
			if (!this.GetBattleShopPoint())
			{
				if (this.GetGroundPoint())
				{
					float radius = this.GetDPI() * 0.5f;
					if (!this.GetBesetTarget(radius) || this.RightTarget())
					{
					}
				}
			}
			return this.ValidGroudPoint || this.ValidTargetUnit;
		}

		private bool RightTargetAll()
		{
			this.targetUnit = null;
			if (this.targetObj != null && !PlayerControlMgr.Instance.GetPlayer().IsForceClickGroud)
			{
				this.targetUnit = this.targetObj.GetComponentInParent<Units>();
				if (this.targetUnit != null && this.targetUnit.isLive && this.targetUnit.CanBeSelected)
				{
					this.validTargetUnit = true;
					return true;
				}
			}
			return false;
		}

		private bool RightTarget()
		{
			this.targetUnit = null;
			if (this.targetObj != null && !PlayerControlMgr.Instance.GetPlayer().IsForceClickGroud)
			{
				this.targetUnit = this.targetObj.GetComponentInParent<Units>();
				if (this.targetUnit != null && this.targetUnit.isLive && TeamManager.CanAttack(this.self, this.targetUnit) && this.targetUnit.CanBeSelected)
				{
					this.validTargetUnit = true;
					return true;
				}
			}
			return false;
		}

		private bool GetBattleShopPoint()
		{
			Ray ray = Camera.main.ScreenPointToRay(this.touchPoint);
			int mask = LayerMask.GetMask(new string[]
			{
				"Shop"
			});
			RaycastHit raycastHit;
			bool flag = Physics.Raycast(ray, out raycastHit, 200f, mask);
			if (flag)
			{
				BattleEquipmentShopModel component = raycastHit.transform.GetComponent<BattleEquipmentShopModel>();
				if (!(component != null))
				{
					return false;
				}
				MobaMessageManagerTools.BattleShop_openBattleShop((EBattleShopType)component.ShopModelType, EBattleShopOpenType.eFromModel);
			}
			return flag;
		}

		public static Units GetTouchUnit(Vector3 worldpoint)
		{
			Vector3 position = Camera.main.WorldToScreenPoint(worldpoint);
			Ray ray = Camera.main.ScreenPointToRay(position);
			int mask = LayerMask.GetMask(new string[]
			{
				"Unit"
			});
			RaycastHit raycastHit;
			bool flag = Physics.Raycast(ray, out raycastHit, 200f, mask);
			if (flag)
			{
				return raycastHit.transform.GetComponent<Units>();
			}
			return null;
		}

		private bool GetGroundPoint()
		{
			Ray ray = Camera.main.ScreenPointToRay(this.touchPoint);
			int groundMask = Layer.GroundMask;
			RaycastHit raycastHit;
			bool flag = Physics.Raycast(ray, out raycastHit, 200f, groundMask);
			if (flag)
			{
				this.groundPoint = raycastHit.point;
				this.validGroundPoint = true;
				return true;
			}
			this.groundPoint = Vector3.zero;
			return false;
		}

		private bool IsUnitsRightTargetCamp(Units target, SkillTargetCamp skillTargetCamp)
		{
			return TeamManager.CheckTeam(this.self.gameObject, target.gameObject, skillTargetCamp, null);
		}

		private bool GetBesetTargetAll(float radius, SkillTargetCamp skillTargetCamp = SkillTargetCamp.All)
		{
			float num = 0.2f;
			float num2 = 999f;
			this.targetObj = null;
			Dictionary<int, Units> allMapUnits = MapManager.Instance.GetAllMapUnits();
			float num3 = 0.6f;
			if (allMapUnits != null && allMapUnits.Count > 0)
			{
				foreach (KeyValuePair<int, Units> current in allMapUnits)
				{
					Units value = current.Value;
					if (!(null == value) && value.isVisibleInCamera && !value.isItem && !value.isBuffItem && value.m_SelectRadius > 0f && this.IsUnitsRightTargetCamp(value, skillTargetCamp))
					{
						Vector3 b = Camera.main.WorldToScreenPoint(value.ColliderCenter);
						b.z = 0f;
						Vector3 a = this.touchPoint;
						a.z = 0f;
						Vector3 position = value.transform.position;
						float num4 = Vector3.Distance(position, this.groundPoint);
						float num5 = Vector3.Distance(a, b);
						float num6 = this.GetDPI() * num;
						if (TagManager.CheckTag(value, global::TargetTag.Tower))
						{
							num6 *= 1.5f;
							num3 = 1.8f;
						}
						else if (TagManager.CheckTag(value, global::TargetTag.Home))
						{
							num6 *= 2.4f;
							num3 = 3f;
						}
						else if (TagManager.CheckTag(value, global::TargetTag.Labisi))
						{
							num6 *= 2.4f;
							num3 = 3f;
						}
						if ((num5 < num6 + this.GetDPI() * num * 0.2f || num4 <= num3) && num5 < num2)
						{
							num2 = num5;
							this.targetObj = value.gameObject;
						}
					}
				}
			}
			if (this.targetObj != null)
			{
				this.validTargetGo = true;
			}
			return this.validTargetGo;
		}

		private bool GetBesetTarget(float radius)
		{
			float num = 0.3f;
			float num2 = 999f;
			this.targetObj = null;
			Dictionary<int, Units> allMapUnits = MapManager.Instance.GetAllMapUnits();
			float num3 = 0.6f;
			foreach (Units current in allMapUnits.Values)
			{
				if (!(null == current) && current.isVisible && current.isLive && current.isVisibleInCamera && this.self.TeamType != current.TeamType && !current.isItem && !current.isBuffItem && current.m_SelectRadius > 0f && current.UnitType != UnitType.Pet && current.UnitType != UnitType.LabisiUnit)
				{
					float num4 = Vector3.Distance(current.transform.position, this.groundPoint);
					if (num4 <= current.m_SelectRadius + 2.5f)
					{
						Vector3 b = Camera.main.WorldToScreenPoint(current.ColliderCenter);
						b.z = 0f;
						Vector3 a = this.touchPoint;
						a.z = 0f;
						float num5 = Vector3.Distance(a, b);
						float num6 = this.GetDPI() * num;
						if (TagManager.CheckTag(current, global::TargetTag.Tower))
						{
							num6 *= 1.5f;
							num3 = 1.8f;
						}
						else if (TagManager.CheckTag(current, global::TargetTag.Home))
						{
							num6 = num6 * 2.4f * current.m_SelectRadius / 2.5f;
							num3 = 3f * current.m_SelectRadius / 2.5f;
						}
						if ((num5 < num6 + this.GetDPI() * num * 0.2f || num4 <= num3) && num5 < num2)
						{
							num2 = num5;
							this.targetObj = current.gameObject;
						}
					}
				}
			}
			if (this.targetObj != null)
			{
				this.validTargetGo = true;
			}
			return this.validTargetGo;
		}

		public void CreateSkillFlag(Vector3 pos, Quaternion rotate)
		{
			this.skillFlag = MapManager.Instance.SpawnResourceHandle("SkillPosFlag", pos, new Quaternion(0f, 0f, 0f, 0f), 0);
		}

		public void ClearSkillFlag()
		{
			if (this.skillFlag != null)
			{
				this.skillFlag.Release();
				this.skillFlag = null;
			}
		}

		public void DrawMoveFlag()
		{
			if (this.targetIcon != null)
			{
				this.targetIcon.Raw.position = this.GroundPoint + new Vector3(0f, 0.1f, 0f);
			}
			else if (this.targetIconBackup != null)
			{
				this.targetIcon = this.targetIconBackup;
				this.targetIcon.Raw.position = this.groundPoint + new Vector3(0f, 0.1f, 0f);
				this.targetIconBackup = null;
			}
			else
			{
				this.targetIcon = MapManager.Instance.SpawnResourceHandle("MouseClick", this.groundPoint + new Vector3(0f, 0.1f, 0f), Quaternion.LookRotation(Vector3.forward), 0);
			}
			if (this.targetIcon != null)
			{
				ClickIcon.Get(this.targetIcon.Raw.gameObject).Init(ClickIconType.MouseClick);
			}
		}

		public void ClearMoveFlag()
		{
			if (this.targetIcon != null)
			{
				this.targetIcon.Raw.position = new Vector3(this.targetIcon.Raw.position.x + 9999f, this.targetIcon.Raw.position.y + 9999f, this.targetIcon.Raw.position.z + 9999f);
				if (this.targetIconBackup != null)
				{
					this.targetIconBackup.Release();
				}
				this.targetIconBackup = this.targetIcon;
			}
			this.targetIcon = null;
		}

		private float GetDPI()
		{
			if (!this.gotDpi)
			{
				this.dpi = ((Screen.dpi <= 0f) ? 100f : Screen.dpi);
				this.gotDpi = true;
			}
			if (GlobalSettings.Instance.QualitySetting.OldLevel == 3 || GlobalSettings.Instance.QualitySetting.OldLevel == 4)
			{
				return this.dpi;
			}
			return this.dpi * 2f / 3f;
		}
	}
}
