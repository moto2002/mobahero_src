using System;

public class AiConfigParam
{
	public enum ActionContext
	{
		At_Home_Add_Hp = 1,
		Eat_Buff,
		Near_Enemy_Tower,
		Back_Home_Use_Skill,
		Back_Home_On_Foot
	}

	public class CheckHealthStateParam
	{
		private float _hpMinVal;

		private float _hpMaxVal;

		private float _hpMinPercent;

		private float _hpMaxPercent;

		public float HpMinVal
		{
			get
			{
				return this._hpMinVal;
			}
		}

		public float HpMaxVal
		{
			get
			{
				return this._hpMaxVal;
			}
		}

		public float HpMinPercent
		{
			get
			{
				return this._hpMinPercent;
			}
		}

		public float HpMaxPercent
		{
			get
			{
				return this._hpMaxPercent;
			}
		}

		public CheckHealthStateParam(float minVal, float minPercent, float maxVal, float maxPercent)
		{
			this._hpMinVal = minVal;
			this._hpMinPercent = minPercent;
			this._hpMaxVal = maxVal;
			this._hpMaxPercent = maxPercent;
		}
	}

	public class LeaveTowerParam
	{
		private float _hpPercent;

		public float HpPercent
		{
			get
			{
				return this._hpPercent;
			}
		}

		public LeaveTowerParam(float hpPercent)
		{
			this._hpPercent = hpPercent;
		}
	}

	public static AiConfigParam.CheckHealthStateParam GetCheckHealthParam(AiConfigParam.ActionContext context)
	{
		AiConfigParam.CheckHealthStateParam result = null;
		switch (context)
		{
		case AiConfigParam.ActionContext.At_Home_Add_Hp:
			result = new AiConfigParam.CheckHealthStateParam(-1f, 0.95f, -1f, -1f);
			break;
		case AiConfigParam.ActionContext.Eat_Buff:
			result = new AiConfigParam.CheckHealthStateParam(-1f, 0.7f, -1f, -1f);
			break;
		case AiConfigParam.ActionContext.Near_Enemy_Tower:
			result = new AiConfigParam.CheckHealthStateParam(-1f, 0.5f, -1f, -1f);
			break;
		case AiConfigParam.ActionContext.Back_Home_Use_Skill:
			result = new AiConfigParam.CheckHealthStateParam(-1f, 0.4f, -1f, -1f);
			break;
		case AiConfigParam.ActionContext.Back_Home_On_Foot:
			result = new AiConfigParam.CheckHealthStateParam(-1f, 0.3f, -1f, -1f);
			break;
		}
		return result;
	}

	public static AiConfigParam.LeaveTowerParam GetLeaveTowerParam(AiConfigParam.ActionContext context)
	{
		return new AiConfigParam.LeaveTowerParam(0.5f);
	}
}
