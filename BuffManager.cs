using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using MobaFrame.SkillAction;
using MobaHeros;
using System;
using System.Collections.Generic;
using System.Linq;

public class BuffManager : UnitComponent
{
	protected Dictionary<string, BuffVo> buffList = new Dictionary<string, BuffVo>();

	protected Dictionary<string, StartBuffAction> buffActionList = new Dictionary<string, StartBuffAction>();

	private Dictionary<string, List<List<DataInfo>>> buffDataInfo = new Dictionary<string, List<List<DataInfo>>>();

	private Dictionary<int, List<string>> m_BuffGroups = new Dictionary<int, List<string>>();

	public BuffManager()
	{
	}

	public BuffManager(Units self) : base(self)
	{
	}

	public override void OnInit()
	{
		base.OnInit();
		if (!this.self.isHero)
		{
			this.donotUpdateByMonster = true;
		}
	}

	public override void OnUpdate(float deltaTime)
	{
		this.doUpdateBuff_Master(deltaTime);
	}

	public override void OnStop()
	{
		this.Clear();
	}

	public override void OnDeath(Units attacker)
	{
		this.Clear();
	}

	public override void OnExit()
	{
		this.Clear();
	}

	private void Clear()
	{
		this.buffDataInfo.Clear();
		this.m_CoroutineManager.StopAllCoroutine();
		this.DestroyBuffs();
	}

	public void AddBuff(string buff_id, Units casterUnit)
	{
		if (!StringUtils.CheckValid(buff_id))
		{
			return;
		}
		BuffData vo = Singleton<BuffDataManager>.Instance.GetVo(buff_id);
		if (vo == null)
		{
			return;
		}
		if (this.m_BuffGroups.ContainsKey(vo.m_nBuffGroup))
		{
			List<string> list = this.m_BuffGroups[vo.m_nBuffGroup];
			if (list != null && list.Count > 0)
			{
				BuffVo buffVo = null;
				BuffVo buffVo2 = null;
				int num = 0;
				for (int i = list.Count - 1; i >= 0; i--)
				{
					if (this.buffList.ContainsKey(list[i]))
					{
						if (buffVo == null)
						{
							buffVo = this.buffList[list[i]];
						}
						else if (buffVo.data.m_OverlapPriority > this.buffList[list[i]].data.m_OverlapPriority)
						{
							buffVo = this.buffList[list[i]];
						}
						if (buffVo2 == null)
						{
							buffVo2 = this.buffList[list[i]];
						}
						else if (buffVo2.data.m_OverlapPriority <= this.buffList[list[i]].data.m_OverlapPriority)
						{
							buffVo2 = this.buffList[list[i]];
						}
						num += this.buffList[list[i]].layer;
					}
					else
					{
						list.RemoveAt(i);
					}
				}
				if (buffVo2 != null && buffVo != null)
				{
					int max_layers = buffVo2.data.config.max_layers;
					if (vo.m_OverlapPriority >= buffVo2.data.m_OverlapPriority)
					{
						max_layers = vo.config.max_layers;
					}
					if (num >= max_layers)
					{
						if (vo.m_OverlapPriority < buffVo.data.m_OverlapPriority)
						{
							return;
						}
						if (buffVo.layer > 1)
						{
							this.RemoveBuff(buffVo.buff_id, 1);
						}
						else
						{
							this.RemoveBuff(buffVo.buff_id, -1);
						}
					}
				}
			}
		}
		if (this.buffList.ContainsKey(buff_id))
		{
			BuffVo buffVo3 = this.buffList[buff_id];
			if (buffVo3 != null)
			{
				buffVo3.casterUnit = casterUnit;
				if (buffVo3.data.config.max_layers == -1)
				{
					return;
				}
				if (!buffVo3.data.isNotClearResetBuffAndDeletAtMax)
				{
					buffVo3.Reset(0);
					if (buffVo3.data.config.max_layers != 0 && buffVo3.layer >= buffVo3.data.config.max_layers)
					{
						return;
					}
					buffVo3.layer++;
				}
				else if (buffVo3.layer == buffVo3.data.config.max_layers - 1)
				{
					buffVo3.ForceOver();
				}
				else
				{
					if (buffVo3.data.config.max_layers != 0 && buffVo3.layer >= buffVo3.data.config.max_layers)
					{
						return;
					}
					buffVo3.layer++;
				}
			}
		}
		else
		{
			BuffVo buffVo3 = BuffVo.Create(buff_id, casterUnit, this.self, 1);
			buffVo3.layer = 1;
			this.buffList.Add(buffVo3.buff_id, buffVo3);
			if (!this.m_BuffGroups.ContainsKey(buffVo3.data.m_nBuffGroup))
			{
				List<string> value = new List<string>();
				this.m_BuffGroups.Add(buffVo3.data.m_nBuffGroup, value);
			}
			this.m_BuffGroups[buffVo3.data.m_nBuffGroup].Add(buff_id);
			if (buffVo3 != null && buffVo3.data.attachState != null)
			{
				int[] attachState = buffVo3.data.attachState;
				for (int j = 0; j < attachState.Length; j++)
				{
					int state = attachState[j];
					this.self.AddState((StateType)state);
				}
			}
		}
		this.doBuff_Master(buff_id);
		Singleton<TriggerManager>.Instance.SendUnitStateEvent(UnitEvent.UnitAddBuff, this.self, null, null);
	}

	public void RemoveBuffGroup(int nGroup, int nCount)
	{
		if (nCount < -2)
		{
			return;
		}
		if (this.m_BuffGroups.ContainsKey(nGroup))
		{
			List<string> list = this.m_BuffGroups[nGroup];
			if (list != null && list.Count > 0)
			{
				int num = nCount;
				if (nCount == -2)
				{
					num = list.Count / 2;
				}
				else if (nCount == -1)
				{
					num = list.Count;
				}
				if (num > list.Count)
				{
					num = list.Count;
				}
				for (int i = 0; i < num; i++)
				{
					this.RemoveBuff(list[i], -1);
				}
				if (num > 0)
				{
					list.RemoveRange(0, num);
				}
			}
		}
	}

	public void RemoveBuff(string buff_id, int reduce_layers = -1)
	{
		if (this.buffList.ContainsKey(buff_id))
		{
			BuffVo buffVo = this.self.buffManager.buffList[buff_id];
			if (buffVo != null && buffVo.layer > 0)
			{
				if (reduce_layers == -1)
				{
					reduce_layers = buffVo.layer;
				}
				else if (reduce_layers == -2)
				{
					reduce_layers = buffVo.layer / 2;
				}
				else if (reduce_layers > 0 && reduce_layers > buffVo.layer)
				{
					reduce_layers = buffVo.layer;
				}
				if (this.buffList.ContainsKey(buff_id))
				{
					if (reduce_layers > 0)
					{
						buffVo.layer -= reduce_layers;
					}
					this.doRevertBuff_Master(buffVo, reduce_layers);
					if (buffVo.layer <= 0)
					{
						if (buffVo != null && buffVo.data != null && this.m_BuffGroups.ContainsKey(buffVo.data.m_nBuffGroup))
						{
							for (int i = 0; i < this.m_BuffGroups[buffVo.data.m_nBuffGroup].Count; i++)
							{
								if (string.Compare(this.m_BuffGroups[buffVo.data.m_nBuffGroup][i], buff_id) == 0)
								{
									this.m_BuffGroups[buffVo.data.m_nBuffGroup].RemoveAt(i);
									break;
								}
							}
						}
						if (StringUtils.CheckValid(buffVo.data.higheff_ids))
						{
							for (int j = 0; j < buffVo.data.higheff_ids.Length; j++)
							{
								this.self.highEffManager.RemoveHighEffect(buffVo.data.higheff_ids[j]);
							}
						}
						if (StringUtils.CheckValid(buffVo.data.buff_ids))
						{
							for (int k = 0; k < buffVo.data.buff_ids.Length; k++)
							{
								this.RemoveBuff(buffVo.data.buff_ids[k], -1);
							}
						}
						this.DestroyAction(buff_id);
						this.buffList.Remove(buff_id);
						Singleton<TriggerManager>.Instance.SendUnitStateEvent(UnitEvent.UnitRemoveBuff, this.self, null, null);
						if (buffVo != null && buffVo.data.attachState != null)
						{
							int[] attachState = buffVo.data.attachState;
							for (int l = 0; l < attachState.Length; l++)
							{
								int state = attachState[l];
								this.self.RemoveState((StateType)state);
							}
						}
						if (StringUtils.CheckValid(buffVo.data.end_attach_higheff_ids))
						{
							for (int m = 0; m < buffVo.data.end_attach_higheff_ids.Length; m++)
							{
								if (!SkillUtility.IsImmunityHighEff(this.self, buffVo.data.end_attach_higheff_ids[m]))
								{
									ActionManager.AddHighEffect(buffVo.data.end_attach_higheff_ids[m], string.Empty, this.self, buffVo.casterUnit, null, true);
								}
							}
						}
						if (StringUtils.CheckValid(buffVo.data.end_attach_buff_ids))
						{
							for (int n = 0; n < buffVo.data.end_attach_buff_ids.Length; n++)
							{
								if (!SkillUtility.IsImmunityBuff(this.self, buffVo.data.end_attach_buff_ids[n]))
								{
									ActionManager.AddBuff(buffVo.data.end_attach_buff_ids[n], this.self, buffVo.casterUnit, true, string.Empty);
								}
							}
						}
					}
					else
					{
						buffVo.Reset(0);
					}
				}
			}
			Singleton<TriggerManager>.Instance.SendUnitStateEvent(UnitEvent.UnitRemoveBuff, this.self, null, null);
		}
	}

	public void SetBuffCDTime(string buffID, float cd)
	{
		BuffVo buff = this.GetBuff(buffID);
		if (buff == null)
		{
			return;
		}
		buff.totalTime = cd;
	}

	public Dictionary<string, BuffVo> GetAllBuffs()
	{
		return this.buffList;
	}

	public void RemoveAllDebuff()
	{
		List<string> list = new List<string>();
		foreach (BuffVo current in this.buffList.Values)
		{
			if (current.data.DataType.GainType == EffectGainType.negative)
			{
				list.Add(current.buff_id);
			}
		}
		foreach (string current2 in list)
		{
			this.RemoveBuff(current2, -1);
		}
	}

	public int GetBuffLayer(string buff_id)
	{
		BuffVo buff = this.GetBuff(buff_id);
		if (buff == null)
		{
			return 0;
		}
		return buff.layer;
	}

	public int GetBuffLayerByGroupId(int groupId)
	{
		if (this.m_BuffGroups == null)
		{
			return 0;
		}
		int num = 0;
		if (this.m_BuffGroups.ContainsKey(groupId) && this.m_BuffGroups[groupId] != null && this.m_BuffGroups[groupId].Count > 0)
		{
			List<string> list = this.m_BuffGroups[groupId];
			for (int i = 0; i < list.Count; i++)
			{
				num += this.GetBuffLayer(list[i]);
			}
		}
		return num;
	}

	public void DoBuff(string buff_id, StartBuffAction action)
	{
		if (!StringUtils.CheckValid(buff_id))
		{
			return;
		}
		if (action == null)
		{
			return;
		}
		BuffVo buffVo = null;
		if (this.buffList.ContainsKey(buff_id))
		{
			buffVo = this.buffList[buff_id];
		}
		if (buffVo != null)
		{
			if ((!this.buffActionList.ContainsKey(buff_id) || buffVo.data.config.isReaptPerform == 1f) && buffVo.data.perform_ids != null)
			{
				for (int i = 0; i < buffVo.data.perform_ids.Length; i++)
				{
					if (StringUtils.CheckValid(buffVo.data.perform_ids[i]) && !buffVo.IsHpOverflow(this.self) && !buffVo.IsMpOverflow(this.self))
					{
						action.AddAction(ActionManager.PlayEffect(buffVo.data.perform_ids[i], this.self, null, null, true, string.Empty, buffVo.casterUnit));
					}
				}
			}
			this.AddAction(buff_id, action);
		}
	}

	public void DestroyBuffs()
	{
		List<string> list = this.buffList.Keys.ToList<string>();
		for (int i = 0; i < list.Count; i++)
		{
			BuffData vo = Singleton<BuffDataManager>.Instance.GetVo(list[i]);
			if (vo != null && vo.isClearWhenDeath)
			{
				this.OnBuffEnd(list[i]);
				if (this.m_BuffGroups.ContainsKey(vo.m_nBuffGroup))
				{
					this.m_BuffGroups.Remove(vo.m_nBuffGroup);
				}
			}
		}
	}

	public void ClearAllBuffs()
	{
		this.DestroyBuffs();
		this.m_BuffGroups.Clear();
		this.buffList.Clear();
		this.DestroyActions();
	}

	private void doBuff_Master(string buff_id)
	{
		if (!StringUtils.CheckValid(buff_id))
		{
			return;
		}
		BuffVo buffVo = null;
		if (this.buffList.ContainsKey(buff_id))
		{
			buffVo = this.buffList[buff_id];
		}
		if (buffVo != null)
		{
			if (!this.buffActionList.ContainsKey(buff_id))
			{
				ActionManager.StartBuff(buff_id, this.self, false);
			}
			else
			{
				this.buffActionList[buff_id].Play();
			}
			if (this.IsPropertyBuff(buff_id))
			{
				this.self.dataChange.doBuffWoundAction(buff_id, buffVo.casterUnit, false);
			}
			else
			{
				this.self.dataChange.doSkillWoundAction(buffVo.data.damage_ids, buffVo.casterUnit, true, new float[0]);
			}
			if (StringUtils.CheckValid(buffVo.data.higheff_ids))
			{
				for (int i = 0; i < buffVo.data.higheff_ids.Length; i++)
				{
					if (!SkillUtility.IsImmunityHighEff(this.self, buffVo.data.higheff_ids[i]))
					{
						ActionManager.AddHighEffect(buffVo.data.higheff_ids[i], string.Empty, this.self, buffVo.casterUnit, null, true);
					}
				}
			}
			if (StringUtils.CheckValid(buffVo.data.buff_ids))
			{
				for (int j = 0; j < buffVo.data.buff_ids.Length; j++)
				{
					if (!SkillUtility.IsImmunityBuff(this.self, buffVo.data.buff_ids[j]))
					{
						ActionManager.AddBuff(buffVo.data.buff_ids[j], this.self, buffVo.casterUnit, true, string.Empty);
					}
				}
			}
		}
	}

	private void doRevertBuff_Master(BuffVo buff, int reduce_layers)
	{
		if (!this.self.IsMaster)
		{
			return;
		}
		if (buff != null && buff.data.revert && reduce_layers > 0 && !buff.IsEffective())
		{
			for (int i = 0; i < reduce_layers; i++)
			{
				this.self.dataChange.doBuffWoundAction(buff.buff_id, buff.casterUnit, true);
			}
		}
	}

	protected virtual void doUpdateBuff_Master(float deltaTime)
	{
		if (!this.self.IsMaster)
		{
			return;
		}
		if (this.buffActionList == null || this.buffActionList.Count == 0)
		{
			return;
		}
		List<string> list = new List<string>();
		List<string> list2 = new List<string>(this.buffActionList.Keys);
		foreach (string current in list2)
		{
			if (this.buffList.ContainsKey(current))
			{
				BuffVo buffVo = this.buffList[current];
				if (buffVo != null)
				{
					buffVo.OnUpdate(deltaTime);
					if (buffVo.IsEffective())
					{
						if (buffVo.IsOver())
						{
							list.Add(buffVo.buff_id);
						}
						else if (buffVo.IsTimeUp())
						{
							buffVo.Reset(1);
							this.doBuff_Master(buffVo.buff_id);
						}
					}
					else if (buffVo.IsOver())
					{
						list.Add(buffVo.buff_id);
					}
					else if (buffVo.IsShieldOver(this.self))
					{
						list.Add(buffVo.buff_id);
					}
				}
			}
		}
		for (int i = 0; i < list.Count; i++)
		{
			this.OnBuffEnd(list[i]);
		}
		if (list.Count > 0)
		{
			list.Clear();
		}
	}

	public bool IsHaveBuff(string buffId)
	{
		return this.buffList.ContainsKey(buffId);
	}

	public bool IsHaveBuffGroup(int groupId)
	{
		return this.m_BuffGroups != null && (this.m_BuffGroups.ContainsKey(groupId) && this.m_BuffGroups[groupId] != null && this.m_BuffGroups[groupId].Count > 0);
	}

	public bool IsPropertyBuff(string buffId)
	{
		BuffData vo = Singleton<BuffDataManager>.Instance.GetVo(buffId);
		return vo != null && vo.isProperty;
	}

	private void OnBuffEnd(string buff_id)
	{
		ActionManager.RemoveBuff(buff_id, this.self, this.self, -1, true);
	}

	private void AddAction(string buff_id, StartBuffAction action)
	{
		if (!this.buffActionList.ContainsKey(buff_id) && action != null && !action.isDestroyed)
		{
			this.buffActionList.Add(buff_id, action);
		}
	}

	private void RemoveAction(string buff_id)
	{
		if (this.buffActionList.ContainsKey(buff_id))
		{
			this.buffActionList.Remove(buff_id);
		}
	}

	private void DestroyAction(string buff_id)
	{
		if (this.buffActionList.ContainsKey(buff_id))
		{
			StartBuffAction startBuffAction = this.buffActionList[buff_id];
			if (startBuffAction != null)
			{
				startBuffAction.Destroy();
			}
			this.RemoveAction(buff_id);
		}
	}

	private void DestroyActions()
	{
		List<string> list = this.buffActionList.Keys.ToList<string>();
		for (int i = 0; i < list.Count; i++)
		{
			StartBuffAction startBuffAction = this.buffActionList[list[i]];
			if (startBuffAction != null)
			{
				startBuffAction.Destroy();
			}
		}
		this.buffActionList.Clear();
	}

	public List<string> GetShowBuffs()
	{
		List<string> list = new List<string>();
		Dictionary<string, BuffVo>.Enumerator enumerator = this.buffList.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<string, BuffVo> current = enumerator.Current;
			string key = current.Key;
			if (this.IsShowBuff(key))
			{
				if (!list.Contains(key))
				{
					list.Add(key);
				}
			}
		}
		return list;
	}

	public Dictionary<string, int> GetShowBuffGroups()
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		foreach (KeyValuePair<int, List<string>> current in this.m_BuffGroups)
		{
			if (current.Value != null && current.Value.Count > 0)
			{
				int num = 0;
				BuffVo buffVo = null;
				for (int i = 0; i < current.Value.Count; i++)
				{
					BuffVo buff = this.GetBuff(current.Value[i]);
					if (buff != null)
					{
						num += buff.layer;
						if (i == current.Value.Count - 1)
						{
							buffVo = buff;
						}
					}
				}
				if (buffVo != null && !dictionary.ContainsKey(buffVo.buff_id))
				{
					if (this.IsShowBuff(buffVo.buff_id))
					{
						dictionary.Add(buffVo.buff_id, num);
					}
				}
			}
		}
		return dictionary;
	}

	public bool IsShowBuff(string buff_Id)
	{
		SysSkillBuffVo dataById = BaseDataMgr.instance.GetDataById<SysSkillBuffVo>(buff_Id);
		return dataById.show_icon == 1;
	}

	public BuffVo GetBuff(string buff_id)
	{
		if (this.buffList.ContainsKey(buff_id))
		{
			return this.buffList[buff_id];
		}
		return null;
	}

	public void AddBuffDataInfo(string buffId, List<DataInfo> infos)
	{
		if (!this.buffDataInfo.ContainsKey(buffId))
		{
			List<List<DataInfo>> list = new List<List<DataInfo>>();
			list.Add(infos);
			this.buffDataInfo.Add(buffId, list);
		}
		else
		{
			this.buffDataInfo[buffId].Add(infos);
		}
	}

	public void RemBuffDataInfo(string buffId)
	{
		if (this.buffDataInfo.ContainsKey(buffId) && this.buffDataInfo[buffId].Count > 0)
		{
			this.buffDataInfo[buffId].RemoveAt(0);
		}
	}

	public List<DataInfo> GetBuffDataInfo(string buffId)
	{
		if (this.buffDataInfo.ContainsKey(buffId) && this.buffDataInfo[buffId].Count > 0)
		{
			return this.buffDataInfo[buffId][0];
		}
		return null;
	}

	public bool HaveBuffDataInfo(string buffId)
	{
		return this.buffDataInfo.ContainsKey(buffId) && this.buffDataInfo[buffId].Count > 0;
	}
}
