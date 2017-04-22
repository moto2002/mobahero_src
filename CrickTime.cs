using Com.Game.Module;
using MobaHeros.Pvp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
/// <summary>
/// 冷却时间组件
/// </summary>
public class CrickTime : StaticUnitComponent
{
    /// <summary>
    /// 运动动画冷却计时
    /// </summary>
	public float moveAnimCool;
    /// <summary>
    /// 动作冷却计时
    /// </summary>
	public float actionCool;
    /// <summary>
    /// 技能冷却计时
    /// </summary>
	public float skillCool;
    /// <summary>
    /// 攻击冷却计时
    /// </summary>
	public float attackCool;
    /// <summary>
    /// 等待冷却计时
    /// </summary>
	public float waitCool;
    /// <summary>
    /// ai冷却计时
    /// </summary>
	public float aiCool;
    /// <summary>
    /// 攻击时间长度冷却计时
    /// </summary>
	public float attackTimeLenghCool;
    /// <summary>
    /// 技能id列表
    /// </summary>
	public List<string> skillID = new List<string>();
    /// <summary>
    /// 技能CD列表
    /// </summary>
	public Dictionary<string, float> skillCD = new Dictionary<string, float>();

	public Dictionary<string, float> publicCD = new Dictionary<string, float>();

	public Dictionary<string, float> chargeCD = new Dictionary<string, float>();

	public List<string> buffID = new List<string>();
    /// <summary>
    /// buff时间列表
    /// </summary>
	public Dictionary<string, float> buffTime = new Dictionary<string, float>();
    /// <summary>
    /// 是否初始化
    /// </summary>
	private bool _hasInit;

	private Task _updateTask;
    /// <summary>
    /// 是否可以播放动画移动????
    /// </summary>
	public bool canMoveAnim
	{
		get
		{
			return this.moveAnimCool <= 0f;
		}
	}
    /// <summary>
    /// 是否执行Action
    /// </summary>
	public bool canAction
	{
		get
		{
			return this.actionCool <= 0f;
		}
	}
    /// <summary>
    /// 是否可以移动
    /// </summary>
	public bool canMove
	{
		get
		{
			return this.waitCool <= 0f;
		}
	}
    /// <summary>
    /// 是否可以旋转
    /// </summary>
	public bool canRotate
	{
		get
		{
			return this.waitCool <= 0f;
		}
	}
    /// <summary>
    /// 是否可以执行AI
    /// </summary>
	public bool canAI
	{
		get
		{
			return this.aiCool <= 0f;
		}
	}
    /// <summary>
    /// 是否可以释放技能
    /// </summary>
	public bool canSkill
	{
		get
		{
			return this.skillCool <= 0f;
		}
	}
    /// <summary>
    /// 是否可以进行攻击
    /// </summary>
	public bool canAttack
	{
		get
		{
			return this.attackCool <= 0f;
		}
	}
    /// <summary>
    /// 是否处于攻击时间内
    /// </summary>
	public bool isInAttackTimeLengh
	{
		get
		{
			return this.attackTimeLenghCool <= 0f;
		}
	}
    /// <summary>
    /// 清理所有状态
    /// </summary>
	private void Clear()
	{
		this.skillCD.Clear();
		this.publicCD.Clear();
		this.buffTime.Clear();
		this.actionCool = 0f;
		this.skillCool = 0f;
		this.attackCool = 0f;
		this.waitCool = 0f;
		this.aiCool = 0f;
		this.attackTimeLenghCool = 0f;
		this.moveAnimCool = 0f;
		this._hasInit = false;
		this._updateTask = null;
		if (this.m_CoroutineManager != null)
		{
			this.m_CoroutineManager.StopAllCoroutine();
		}
	}

	[DebuggerHidden]
	private IEnumerator Update_Coroutine()
	{
		CrickTime.<Update_Coroutine>c__Iterator1D2 <Update_Coroutine>c__Iterator1D = new CrickTime.<Update_Coroutine>c__Iterator1D2();
		<Update_Coroutine>c__Iterator1D.<>f__this = this;
		return <Update_Coroutine>c__Iterator1D;
	}
    /// <summary>
    /// 更新各种冷却时间及buff时间状态
    /// </summary>
    /// <param name="deltaTime"></param>
	private void DoUpdate(float deltaTime)
	{
		if (GameManager.IsPausing())
		{
			return;
		}
		if (this.actionCool > 0f)
		{
			this.actionCool -= deltaTime;
		}
		if (this.skillCool > 0f)
		{
			this.skillCool -= deltaTime;
		}
		if (this.attackCool > 0f)
		{
			this.attackCool -= deltaTime;
		}
		if (this.waitCool > 0f)
		{
			this.waitCool -= deltaTime;
		}
		if (this.aiCool > 0f)
		{
			this.aiCool -= deltaTime;
		}
		if (this.attackTimeLenghCool > 0f)
		{
			this.attackTimeLenghCool -= deltaTime;
		}
		if (this.moveAnimCool > 0f)
		{
			this.moveAnimCool -= deltaTime;
		}
        //不是观察者也不是当前玩家角色
		if (!Singleton<PvpManager>.Instance.IsObserver && !this.self.isPlayer)
		{
			return;
		}
        //主控角色才进入以下逻辑,刷新各种技能的CD状态,buff时间状态等
		for (int i = 0; i < this.skillID.Count; i++)
		{
			string text = this.skillID[i];
			float num;
			if (this.skillCD.TryGetValue(text, out num) && num > 0f)
			{
				if (num <= deltaTime)
				{
					this.skillCD[text] = 0f;
					MessageEventArgs messageEventArgs = new MessageEventArgs();
					messageEventArgs.AddMessage("type", 256.ToString());
					messageEventArgs.AddMessage("id", text);
					Singleton<SkillView>.Instance.GetMessages(messageEventArgs);
				}
				else
				{
					this.skillCD[text] = num - deltaTime;
				}
			}
			float num2 = 0f;
			if (this.publicCD.TryGetValue(text, out num2) && num2 > 0f)
			{
				if (num2 <= deltaTime)
				{
					this.publicCD[text] = 0f;
					MessageEventArgs messageEventArgs2 = new MessageEventArgs();
					messageEventArgs2.AddMessage("type", 256.ToString());
					messageEventArgs2.AddMessage("id", text);
					Singleton<SkillView>.Instance.GetMessages(messageEventArgs2);
				}
				else
				{
					this.publicCD[text] = num2 - deltaTime;
				}
			}
			float num3;
			if (this.chargeCD.TryGetValue(text, out num3) && num3 > 0f)
			{
				if (num3 <= deltaTime)
				{
					this.chargeCD[text] = 0f;
				}
				else
				{
					this.chargeCD[text] = num3 - deltaTime;
				}
			}
		}
		if (this.buffTime != null && this.buffTime.Count > 0)
		{
			for (int j = 0; j < this.buffID.Count; j++)
			{
				string text2 = this.buffID[j];
				if (this.buffTime.ContainsKey(text2) && this.buffTime[text2] > 0f && this.buffTime[text2] >= 0f)
				{
					Dictionary<string, float> dictionary;
					Dictionary<string, float> expr_2FA = dictionary = this.buffTime;
					string key;
					string expr_2FF = key = text2;
					float num4 = dictionary[key];
					if ((expr_2FA[expr_2FF] = num4 - deltaTime) <= 0f)
					{
						this.buffTime[text2] = 0f;
						MessageEventArgs messageEventArgs3 = new MessageEventArgs();
						messageEventArgs3.AddMessage("type", 257.ToString());
						messageEventArgs3.AddMessage("id", text2);
						Singleton<SkillView>.Instance.GetMessages(messageEventArgs3);
					}
				}
				float num5;
				if (this.buffTime.TryGetValue(text2, out num5) && num5 > 0f)
				{
					if (num5 <= deltaTime)
					{
						this.buffTime[text2] = 0f;
						MessageEventArgs messageEventArgs4 = new MessageEventArgs();
						messageEventArgs4.AddMessage("type", 257.ToString());
						messageEventArgs4.AddMessage("id", text2);
						Singleton<SkillView>.Instance.GetMessages(messageEventArgs4);
					}
					else
					{
						this.buffTime[text2] = num5 - deltaTime;
					}
				}
			}
		}
	}

	public override void OnInit()
	{
		if (!this._hasInit)
		{
			if (this._updateTask == null)
			{
				this._updateTask = this.m_CoroutineManager.StartCoroutine(this.Update_Coroutine(), true);
			}
			this._hasInit = true;
		}
	}

	public override void OnStop()
	{
	}

	public override void OnExit()
	{
		this.Clear();
	}
    /// <summary>
    /// 设置技能cd时间
    /// </summary>
    /// <param name="skill_id"></param>
    /// <param name="time"></param>
	public void SetSkillCDTime(string skill_id, float time)
	{
		if (!this.skillID.Contains(skill_id))
		{
			this.skillID.Add(skill_id);
			this.skillCD.Add(skill_id, time);
		}
		else
		{
			this.skillCD[skill_id] = time;
		}
	}
    /// <summary>
    /// 获取技能cd时间
    /// </summary>
    /// <param name="skill_id"></param>
    /// <returns></returns>
	public float GetSkillCDtime(string skill_id)
	{
		if (this.skillCD.ContainsKey(skill_id))
		{
			return this.skillCD[skill_id];
		}
		return 0f;
	}
    /// <summary>
    /// 重置技能冷却
    /// </summary>
	public void ResertAllSkillCool()
	{
		this.skillCD.Clear();
	}
    /// <summary>
    /// 重置指定id的技能冷却
    /// </summary>
    /// <param name="skill_id"></param>
	public void ResertSkillCool(string skill_id)
	{
		if (this.skillCD.ContainsKey(skill_id))
		{
			this.skillCD[skill_id] = 0f;
		}
	}
    /// <summary>
    /// 设置buff的时间
    /// </summary>
    /// <param name="buff_id"></param>
    /// <param name="time"></param>
	public void SetBuffTime(string buff_id, float time)
	{
		if (!this.buffID.Contains(buff_id))
		{
			this.buffID.Add(buff_id);
			this.buffTime.Add(buff_id, time);
		}
		else
		{
			this.buffTime[buff_id] = time;
		}
	}
    /// <summary>
    /// 获取buff的时间
    /// </summary>
    /// <param name="buff_id"></param>
    /// <returns></returns>
	public float GetBuffTime(string buff_id)
	{
		if (this.buffTime.ContainsKey(buff_id))
		{
			return this.buffTime[buff_id];
		}
		return 0f;
	}
    /// <summary>
    /// 设置技能主管时间
    /// </summary>
    /// <param name="skill_id"></param>
    /// <param name="time"></param>
	public void SetChargeTime(string skill_id, float time)
	{
		if (!this.skillID.Contains(skill_id))
		{
			this.skillID.Add(skill_id);
			this.chargeCD.Add(skill_id, time);
		}
		else
		{
			this.chargeCD[skill_id] = time;
		}
	}
    /// <summary>
    /// 获取技能主管时间
    /// </summary>
    /// <param name="skill_id"></param>
    /// <returns></returns>
	public float GetChargeTime(string skill_id)
	{
		if (this.chargeCD.ContainsKey(skill_id))
		{
			return this.chargeCD[skill_id];
		}
		return 0f;
	}
}
