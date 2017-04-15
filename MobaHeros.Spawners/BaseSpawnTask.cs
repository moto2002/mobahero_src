using Com.Game.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace MobaHeros.Spawners
{
	public abstract class BaseSpawnTask : ISpawnTask
	{
		public delegate void OnSpawnUnit(Units units, EntityVo entity);

		protected SpawnUtility SpawnUtility;

		protected SysBattleSceneVo MyScene;

		protected CoroutineManager MyCoroutineManager = new CoroutineManager();

		public event Action<BaseSpawnTask, bool> OnFinished
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.OnFinished = (Action<BaseSpawnTask, bool>)Delegate.Combine(this.OnFinished, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.OnFinished = (Action<BaseSpawnTask, bool>)Delegate.Remove(this.OnFinished, value);
			}
		}

		public string TaskName
		{
			get;
			set;
		}

		protected BaseSpawnTask(SysBattleSceneVo scene)
		{
			this.MyScene = scene;
			this.SpawnUtility = new SpawnUtility(this.MyScene);
		}

		public virtual void Start()
		{
		}

		public virtual void Stop()
		{
			this.MyCoroutineManager.StopAllCoroutine();
		}

		[DebuggerHidden]
		public static IEnumerator ConcatEnum(params IEnumerator[] enums)
		{
			BaseSpawnTask.<ConcatEnum>c__Iterator1C8 <ConcatEnum>c__Iterator1C = new BaseSpawnTask.<ConcatEnum>c__Iterator1C8();
			<ConcatEnum>c__Iterator1C.enums = enums;
			<ConcatEnum>c__Iterator1C.<$>enums = enums;
			return <ConcatEnum>c__Iterator1C;
		}

		[DebuggerHidden]
		protected IEnumerator TriggerFinishEvents(bool res)
		{
			BaseSpawnTask.<TriggerFinishEvents>c__Iterator1C9 <TriggerFinishEvents>c__Iterator1C = new BaseSpawnTask.<TriggerFinishEvents>c__Iterator1C9();
			<TriggerFinishEvents>c__Iterator1C.res = res;
			<TriggerFinishEvents>c__Iterator1C.<$>res = res;
			<TriggerFinishEvents>c__Iterator1C.<>f__this = this;
			return <TriggerFinishEvents>c__Iterator1C;
		}

		[DebuggerHidden]
		protected IEnumerator DoSpawnInstance_Coroutinue(EntityVo npcinfo, string tag, TeamType teamType, int spawnPos, float spawnInterval = 0f, float spawnDelay = 0f, string respawnInterval = "[]", Callback<int> onSpawnEnd = null, Callback<Units> onSpawnOne = null)
		{
			BaseSpawnTask.<DoSpawnInstance_Coroutinue>c__Iterator1CA <DoSpawnInstance_Coroutinue>c__Iterator1CA = new BaseSpawnTask.<DoSpawnInstance_Coroutinue>c__Iterator1CA();
			<DoSpawnInstance_Coroutinue>c__Iterator1CA.npcinfo = npcinfo;
			<DoSpawnInstance_Coroutinue>c__Iterator1CA.spawnInterval = spawnInterval;
			<DoSpawnInstance_Coroutinue>c__Iterator1CA.tag = tag;
			<DoSpawnInstance_Coroutinue>c__Iterator1CA.teamType = teamType;
			<DoSpawnInstance_Coroutinue>c__Iterator1CA.spawnPos = spawnPos;
			<DoSpawnInstance_Coroutinue>c__Iterator1CA.respawnInterval = respawnInterval;
			<DoSpawnInstance_Coroutinue>c__Iterator1CA.onSpawnOne = onSpawnOne;
			<DoSpawnInstance_Coroutinue>c__Iterator1CA.spawnDelay = spawnDelay;
			<DoSpawnInstance_Coroutinue>c__Iterator1CA.onSpawnEnd = onSpawnEnd;
			<DoSpawnInstance_Coroutinue>c__Iterator1CA.<$>npcinfo = npcinfo;
			<DoSpawnInstance_Coroutinue>c__Iterator1CA.<$>spawnInterval = spawnInterval;
			<DoSpawnInstance_Coroutinue>c__Iterator1CA.<$>tag = tag;
			<DoSpawnInstance_Coroutinue>c__Iterator1CA.<$>teamType = teamType;
			<DoSpawnInstance_Coroutinue>c__Iterator1CA.<$>spawnPos = spawnPos;
			<DoSpawnInstance_Coroutinue>c__Iterator1CA.<$>respawnInterval = respawnInterval;
			<DoSpawnInstance_Coroutinue>c__Iterator1CA.<$>onSpawnOne = onSpawnOne;
			<DoSpawnInstance_Coroutinue>c__Iterator1CA.<$>spawnDelay = spawnDelay;
			<DoSpawnInstance_Coroutinue>c__Iterator1CA.<$>onSpawnEnd = onSpawnEnd;
			<DoSpawnInstance_Coroutinue>c__Iterator1CA.<>f__this = this;
			return <DoSpawnInstance_Coroutinue>c__Iterator1CA;
		}

		[DebuggerHidden]
		protected IEnumerator DoSpawnPlayers_Coroutinue(List<EntityVo> friendInfo, int[] playerPos, float spawnInterval = 0f, float spawnDelay = 0f, string respawnInterval = "[]", Callback<int> onSpawnEnd = null, BaseSpawnTask.OnSpawnUnit onSpawnOne = null)
		{
			BaseSpawnTask.<DoSpawnPlayers_Coroutinue>c__Iterator1CB <DoSpawnPlayers_Coroutinue>c__Iterator1CB = new BaseSpawnTask.<DoSpawnPlayers_Coroutinue>c__Iterator1CB();
			<DoSpawnPlayers_Coroutinue>c__Iterator1CB.friendInfo = friendInfo;
			<DoSpawnPlayers_Coroutinue>c__Iterator1CB.spawnInterval = spawnInterval;
			<DoSpawnPlayers_Coroutinue>c__Iterator1CB.playerPos = playerPos;
			<DoSpawnPlayers_Coroutinue>c__Iterator1CB.respawnInterval = respawnInterval;
			<DoSpawnPlayers_Coroutinue>c__Iterator1CB.onSpawnOne = onSpawnOne;
			<DoSpawnPlayers_Coroutinue>c__Iterator1CB.spawnDelay = spawnDelay;
			<DoSpawnPlayers_Coroutinue>c__Iterator1CB.onSpawnEnd = onSpawnEnd;
			<DoSpawnPlayers_Coroutinue>c__Iterator1CB.<$>friendInfo = friendInfo;
			<DoSpawnPlayers_Coroutinue>c__Iterator1CB.<$>spawnInterval = spawnInterval;
			<DoSpawnPlayers_Coroutinue>c__Iterator1CB.<$>playerPos = playerPos;
			<DoSpawnPlayers_Coroutinue>c__Iterator1CB.<$>respawnInterval = respawnInterval;
			<DoSpawnPlayers_Coroutinue>c__Iterator1CB.<$>onSpawnOne = onSpawnOne;
			<DoSpawnPlayers_Coroutinue>c__Iterator1CB.<$>spawnDelay = spawnDelay;
			<DoSpawnPlayers_Coroutinue>c__Iterator1CB.<$>onSpawnEnd = onSpawnEnd;
			<DoSpawnPlayers_Coroutinue>c__Iterator1CB.<>f__this = this;
			return <DoSpawnPlayers_Coroutinue>c__Iterator1CB;
		}

		[DebuggerHidden]
		protected IEnumerator SpawnInstances_Coroutinue(List<EntityVo> npcinfo, string tag, TeamType teamType, string spawnPos, float spawnTime = 0f, float spawnDelay = 0f, string respawnInterval = "[]", Callback<int> onSpawnEnd = null, BaseSpawnTask.OnSpawnUnit onSpawnOne = null)
		{
			BaseSpawnTask.<SpawnInstances_Coroutinue>c__Iterator1CC <SpawnInstances_Coroutinue>c__Iterator1CC = new BaseSpawnTask.<SpawnInstances_Coroutinue>c__Iterator1CC();
			<SpawnInstances_Coroutinue>c__Iterator1CC.npcinfo = npcinfo;
			<SpawnInstances_Coroutinue>c__Iterator1CC.spawnTime = spawnTime;
			<SpawnInstances_Coroutinue>c__Iterator1CC.spawnPos = spawnPos;
			<SpawnInstances_Coroutinue>c__Iterator1CC.tag = tag;
			<SpawnInstances_Coroutinue>c__Iterator1CC.teamType = teamType;
			<SpawnInstances_Coroutinue>c__Iterator1CC.respawnInterval = respawnInterval;
			<SpawnInstances_Coroutinue>c__Iterator1CC.onSpawnOne = onSpawnOne;
			<SpawnInstances_Coroutinue>c__Iterator1CC.spawnDelay = spawnDelay;
			<SpawnInstances_Coroutinue>c__Iterator1CC.onSpawnEnd = onSpawnEnd;
			<SpawnInstances_Coroutinue>c__Iterator1CC.<$>npcinfo = npcinfo;
			<SpawnInstances_Coroutinue>c__Iterator1CC.<$>spawnTime = spawnTime;
			<SpawnInstances_Coroutinue>c__Iterator1CC.<$>spawnPos = spawnPos;
			<SpawnInstances_Coroutinue>c__Iterator1CC.<$>tag = tag;
			<SpawnInstances_Coroutinue>c__Iterator1CC.<$>teamType = teamType;
			<SpawnInstances_Coroutinue>c__Iterator1CC.<$>respawnInterval = respawnInterval;
			<SpawnInstances_Coroutinue>c__Iterator1CC.<$>onSpawnOne = onSpawnOne;
			<SpawnInstances_Coroutinue>c__Iterator1CC.<$>spawnDelay = spawnDelay;
			<SpawnInstances_Coroutinue>c__Iterator1CC.<$>onSpawnEnd = onSpawnEnd;
			<SpawnInstances_Coroutinue>c__Iterator1CC.<>f__this = this;
			return <SpawnInstances_Coroutinue>c__Iterator1CC;
		}

		[DebuggerHidden]
		protected IEnumerator DoSpawnPvpHeroes_Coroutinue(List<EntityVo> npcinfo, string tag, TeamType teamType, string spawnPos, float spawnTime = 0f, float spawnDelay = 0f, Callback<int> onSpawnEnd = null, BaseSpawnTask.OnSpawnUnit onSpawnOne = null)
		{
			BaseSpawnTask.<DoSpawnPvpHeroes_Coroutinue>c__Iterator1CD <DoSpawnPvpHeroes_Coroutinue>c__Iterator1CD = new BaseSpawnTask.<DoSpawnPvpHeroes_Coroutinue>c__Iterator1CD();
			<DoSpawnPvpHeroes_Coroutinue>c__Iterator1CD.npcinfo = npcinfo;
			<DoSpawnPvpHeroes_Coroutinue>c__Iterator1CD.spawnTime = spawnTime;
			<DoSpawnPvpHeroes_Coroutinue>c__Iterator1CD.spawnPos = spawnPos;
			<DoSpawnPvpHeroes_Coroutinue>c__Iterator1CD.tag = tag;
			<DoSpawnPvpHeroes_Coroutinue>c__Iterator1CD.teamType = teamType;
			<DoSpawnPvpHeroes_Coroutinue>c__Iterator1CD.onSpawnOne = onSpawnOne;
			<DoSpawnPvpHeroes_Coroutinue>c__Iterator1CD.spawnDelay = spawnDelay;
			<DoSpawnPvpHeroes_Coroutinue>c__Iterator1CD.onSpawnEnd = onSpawnEnd;
			<DoSpawnPvpHeroes_Coroutinue>c__Iterator1CD.<$>npcinfo = npcinfo;
			<DoSpawnPvpHeroes_Coroutinue>c__Iterator1CD.<$>spawnTime = spawnTime;
			<DoSpawnPvpHeroes_Coroutinue>c__Iterator1CD.<$>spawnPos = spawnPos;
			<DoSpawnPvpHeroes_Coroutinue>c__Iterator1CD.<$>tag = tag;
			<DoSpawnPvpHeroes_Coroutinue>c__Iterator1CD.<$>teamType = teamType;
			<DoSpawnPvpHeroes_Coroutinue>c__Iterator1CD.<$>onSpawnOne = onSpawnOne;
			<DoSpawnPvpHeroes_Coroutinue>c__Iterator1CD.<$>spawnDelay = spawnDelay;
			<DoSpawnPvpHeroes_Coroutinue>c__Iterator1CD.<$>onSpawnEnd = onSpawnEnd;
			<DoSpawnPvpHeroes_Coroutinue>c__Iterator1CD.<>f__this = this;
			return <DoSpawnPvpHeroes_Coroutinue>c__Iterator1CD;
		}
	}
}
