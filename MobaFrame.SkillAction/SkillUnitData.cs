using Com.Game.Data;
using Com.Game.Manager;
using System;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class SkillUnitData
	{
		public string dataId;

		public SysSkillUnitVo config;

		public bool destroy_flag;

		public bool IsBloodBall
		{
			get
			{
				return this.config.item_type == 2 || this.config.item_type == 3 || this.config.item_type == 4;
			}
		}

		public SkillUnitData(string data_id)
		{
			this.dataId = data_id;
			this.config = BaseDataMgr.instance.GetDataById<SysSkillUnitVo>(data_id);
			if (this.config == null)
			{
				Debug.LogError("Error dataId =" + this.dataId);
				return;
			}
			this.Parse(this.config);
		}

		public SkillUnitData(string data_id, SysSkillUnitVo config)
		{
			this.dataId = data_id;
			this.config = config;
			if (config == null)
			{
				Debug.LogError("Error dataId=" + this.dataId);
				return;
			}
			this.Parse(config);
		}

		public void Parse(SysSkillUnitVo dataConfig)
		{
			this.destroy_flag = (this.config.destroy_flag == 1);
		}
	}
}
