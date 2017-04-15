using Com.Game.Utils;
using MobaHeros;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace MobaFrame.SkillAction
{
	public abstract class BaseAction
	{
		private class Stat
		{
			public int ctorCnt;

			public int createNodeCnt;

			public int destroyCnt;

			public int destroyNodeCnt;

			public override string ToString()
			{
				return string.Format("{0}/{1} {2}/{3}", new object[]
				{
					this.ctorCnt,
					this.destroyCnt,
					this.createNodeCnt,
					this.destroyNodeCnt
				});
			}
		}

		private const int ID_SKILLACTION_START = 1000;

		private static readonly BaseAction.Stat s_stat = new BaseAction.Stat();

		private static List<GameObject> nodePool = new List<GameObject>();

		private static GameObject skillNodePool;

		private static GameObject childSkillNodePool;

		public int actionId;

		private ResourceHandle _nodeGoHandle;

		private GameObject _nodeGo;

		private Units actionUnit;

		private Transform actionTransform;

		private Vector3? actionPosition;

		private string actionName;

		private bool isRecordAction = true;

		private bool is_c2p = true;

		public Callback<BaseAction> OnActionStartCallback;

		public Callback<BaseAction> OnActionEndCallback;

		public Callback<BaseAction> OnActionStopCallback;

		public bool isStopped;

		public bool isDestroyed;

		public bool isPlaying;

		protected bool isActive;

		protected CoroutineManager m_coroutinue;

		private static int skill_action_id = 1000;

		private static readonly Dictionary<NodeType, Transform> _dictionary = new Dictionary<NodeType, Transform>(default(EnumEqualityComparer<NodeType>));

		public GameObject gameObject
		{
			get
			{
				return this._nodeGo;
			}
		}

		public Transform transform
		{
			get
			{
				if (this.gameObject != null)
				{
					return this.gameObject.transform;
				}
				return null;
			}
		}

		public bool IsAutoDestroy
		{
			get;
			set;
		}

		public Units unit
		{
			get
			{
				return this.actionUnit;
			}
			set
			{
				this.actionUnit = value;
			}
		}

		public Transform mActionTransform
		{
			get
			{
				if (this.actionUnit != null)
				{
					this.actionTransform = this.unit.mTransform;
				}
				else if (this.actionTransform == null)
				{
					this.actionTransform = this.transform;
				}
				return this.actionTransform;
			}
			set
			{
				this.actionTransform = value;
			}
		}

		public Vector3? mActionPosition
		{
			get
			{
				if (this.mActionTransform != null)
				{
					this.actionPosition = new Vector3?(this.mActionTransform.position);
				}
				return this.actionPosition;
			}
			set
			{
				this.actionPosition = value;
			}
		}

		public string mActionName
		{
			get
			{
				if (this.actionName == null)
				{
					this.actionName = string.Concat(new object[]
					{
						"Action_",
						base.GetType().Name,
						"_",
						this.actionId
					});
				}
				return this.actionName;
			}
			set
			{
				this.actionName = value;
			}
		}

		public bool IsRecordAction
		{
			get
			{
				return this.isRecordAction;
			}
			set
			{
				this.isRecordAction = value;
			}
		}

		protected bool IsRecord
		{
			get
			{
				return false;
			}
		}

		protected virtual bool IsMaster
		{
			get
			{
				return this.unit != null && this.unit.IsMaster;
			}
		}

		protected virtual bool IsFree
		{
			get
			{
				return this.unit != null && this.unit.IsFree;
			}
		}

		public bool IsC2P
		{
			get
			{
				return this.is_c2p;
			}
			set
			{
				this.is_c2p = value;
			}
		}

		public bool IsMasterAction
		{
			get
			{
				return this.IsMaster && this.IsC2P;
			}
		}

		public bool IsDestroyed
		{
			get
			{
				return this.isDestroyed;
			}
		}

		protected virtual bool useCollider
		{
			get
			{
				return this.IsMaster && this.IsC2P;
			}
		}

		protected CoroutineManager mCoroutineManager
		{
			get
			{
				if (this.m_coroutinue == null)
				{
					this.m_coroutinue = new CoroutineManager();
				}
				return this.m_coroutinue;
			}
		}

		protected BaseAction()
		{
			this.actionId = this.assign_id();
			BaseAction.s_stat.ctorCnt++;
		}

		public static void AllocNodePool()
		{
			if (BaseAction.skillNodePool == null)
			{
				BaseAction.skillNodePool = new GameObject("SkillNodePool");
			}
			if (BaseAction.childSkillNodePool == null)
			{
				BaseAction.childSkillNodePool = new GameObject("ChildSkillNodePool");
			}
			for (int i = 0; i < BaseAction.nodePool.Count; i++)
			{
				if (BaseAction.nodePool[i] != null)
				{
					UnityEngine.Object.Destroy(BaseAction.nodePool[i]);
				}
			}
			BaseAction.nodePool.Clear();
			for (int j = 0; j < 1; j++)
			{
				BaseAction.ReleaseNode(BaseAction.AllocNode(NodeType.None, true));
			}
		}

		private static GameObject AllocNode(NodeType type, bool forceNew = false)
		{
			string text = null;
			if (BaseAction.nodePool.Count != 0 && !forceNew)
			{
				GameObject gameObject = BaseAction.nodePool[0];
				BaseAction.nodePool.RemoveAt(0);
				gameObject.SetActive(true);
				gameObject.transform.position = Vector3.zero;
				return gameObject;
			}
			switch (type)
			{
			case NodeType.None:
				text = "Prefab/Skill/SkillNode";
				break;
			case NodeType.SkillNode:
				text = "Prefab/Skill/SkillNode";
				break;
			case NodeType.EffectNode:
				text = "Prefab/Skill/EffectNode";
				break;
			case NodeType.DamageNode:
				text = "Prefab/Skill/DamageNode";
				break;
			}
			if (text != null)
			{
				UnityEngine.Object original = Resources.Load(text);
				GameObject gameObject2 = UnityEngine.Object.Instantiate(original) as GameObject;
				gameObject2.tag = "ActionNode";
				return gameObject2;
			}
			return null;
		}

		private static void ReleaseNode(GameObject go)
		{
			UnityEngine.Object.Destroy(go);
		}

		public static void Dump()
		{
			ClientLogger.Error(BaseAction.s_stat.ToString());
		}

		public virtual bool IsForceDisplay()
		{
			return false;
		}

		public void Play()
		{
			this.OnInit();
			this.OnPlay();
			this.isPlaying = true;
			this.isStopped = false;
			this.isDestroyed = false;
			this.OnActionPlay();
		}

		public void Stop()
		{
			if (!this.isStopped && this.isPlaying)
			{
				this.OnStop();
				this.isStopped = true;
				this.isPlaying = false;
				this.isActive = false;
				this.OnActionStop();
			}
		}

		public virtual void Destroy()
		{
			if (!this.isDestroyed)
			{
				if (!this.isStopped)
				{
					this.Stop();
				}
				this.OnDestroy();
				this.isStopped = true;
				this.isDestroyed = true;
				this.isPlaying = false;
				this.OnActionEnd();
				this.OnActionEndCallback = null;
				this.OnActionStartCallback = null;
				this.OnActionStopCallback = null;
			}
		}

		public void RecordStart()
		{
		}

		public void RecordEnd()
		{
		}

		public void SendStart()
		{
			if (this.IsMaster && !this.IsFree && this.IsC2P)
			{
				this.OnSendStart();
			}
		}

		public void SendEnd()
		{
			if (this.IsMaster && !this.IsFree && this.IsC2P)
			{
				this.OnSendEnd();
			}
		}

		protected virtual void OnInit()
		{
		}

		protected virtual void OnPlay()
		{
			if (this.doAction())
			{
				this.RecordStart();
				this.SendStart();
			}
			else
			{
				this.Destroy();
			}
		}

		protected abstract bool doAction();

		protected virtual void OnStop()
		{
			if (this.m_coroutinue != null)
			{
				this.m_coroutinue.StopAllCoroutine();
			}
		}

		protected virtual void OnUpdate()
		{
		}

		protected virtual void OnDestroy()
		{
			BaseAction.s_stat.destroyCnt++;
			if (this.gameObject)
			{
			}
			if (this._nodeGoHandle != null)
			{
				BaseAction.s_stat.destroyNodeCnt++;
				ResourceHandle.SafeRelease(ref this._nodeGoHandle);
				this._nodeGo = null;
			}
			if (this._nodeGo != null)
			{
				BaseAction.ReleaseNode(this._nodeGo);
				this._nodeGo = null;
			}
		}

		public void DestroyNode()
		{
		}

		protected virtual void OnRecordStart()
		{
		}

		protected virtual void OnRecordEnd()
		{
		}

		protected virtual void OnSendStart()
		{
		}

		protected virtual void OnSendEnd()
		{
		}

		protected virtual void OnActionPlay()
		{
			if (this.OnActionStartCallback != null)
			{
				this.OnActionStartCallback(this);
			}
		}

		protected virtual void OnActionStop()
		{
			if (this.OnActionStopCallback != null)
			{
				this.OnActionStopCallback(this);
			}
		}

		protected virtual void OnActionEnd()
		{
			if (this.OnActionEndCallback != null)
			{
				this.OnActionEndCallback(this);
			}
		}

		private int assign_id()
		{
			BaseAction.skill_action_id++;
			return BaseAction.skill_action_id;
		}

		public static T CreateAction<T>() where T : BaseAction, new()
		{
			return Activator.CreateInstance<T>();
		}

		private static Transform GetNodeParent(NodeType type)
		{
			Transform transform;
			if (BaseAction._dictionary.TryGetValue(type, out transform) && transform)
			{
				return transform;
			}
			GameObject gameObject = new GameObject(type.ToString());
			BaseAction._dictionary[type] = gameObject.transform;
			return gameObject.transform;
		}

		public static void ClearResources()
		{
			BaseAction._dictionary.Clear();
		}

		protected void CreateNode(NodeType type, string nodeName = null)
		{
			if (this._nodeGoHandle != null)
			{
				return;
			}
			this._nodeGo = null;
			switch (type)
			{
			case NodeType.SkillNode:
				this._nodeGo = BaseAction.AllocNode(type, false);
				this._nodeGo.name = "SkillNode";
				break;
			case NodeType.EffectNode:
				this._nodeGo = BaseAction.AllocNode(type, false);
				this._nodeGo.name = "EffectNode";
				break;
			case NodeType.DamageNode:
				this._nodeGo = BaseAction.AllocNode(type, false);
				this._nodeGo.name = "DamageNode";
				break;
			}
			this._nodeGo.transform.parent = BaseAction.GetNodeParent(type);
			BaseAction.s_stat.createNodeCnt++;
		}

		public virtual void ReplaceSomeThing(int type, object something)
		{
		}

		public static bool IsAnchorFollowEffect(PerformData data)
		{
			return data.config.effect_pos_type == 2 || data.config.effect_pos_type == 5;
		}

		public static bool IsDamageColliderFollow(PerformData data)
		{
			return data.isDamageColliderFollow;
		}

		public static bool IsDamageColliderFollowUnit(PerformData data)
		{
			return data.isDamageColliderFollowUnit;
		}

		public static bool IsUseSelfCollider(PerformData data)
		{
			return data.colliderRangeType == ColliderRangeType.SelfCollider;
		}

		public static float GetPerformEffectDelay(Units actionUnit, PerformData performData)
		{
			if (performData == null)
			{
				return 0f;
			}
			if (actionUnit != null && actionUnit.IsInAttack)
			{
				return performData.config.effect_delay / actionUnit.animController.GetMecanim().animator.speed;
			}
			return performData.config.effect_delay;
		}

		protected virtual void RemoveActionFromSkill(BaseAction action)
		{
		}

		protected virtual void AddActionToSkill(SkillCastPhase phase, BaseAction action)
		{
		}

		public virtual void DoSpecialProcess()
		{
		}

		public virtual bool GetActionById(int inActionId, out BaseAction outActionInst)
		{
			outActionInst = null;
			if (this.actionId == inActionId)
			{
				outActionInst = this;
				return true;
			}
			return false;
		}
	}
}
