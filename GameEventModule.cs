using Com.Game.Module;
using MobaHeros.Pvp;
using System;
using System.Collections;
using System.Diagnostics;

public class GameEventModule : BaseGameModule
{
	private readonly CoroutineManager _coroutineManager = new CoroutineManager();

	private bool threeFigherModel;

	[DebuggerHidden]
	private IEnumerator Playing_Coroutine()
	{
		GameEventModule.<Playing_Coroutine>c__Iterator1A7 <Playing_Coroutine>c__Iterator1A = new GameEventModule.<Playing_Coroutine>c__Iterator1A7();
		<Playing_Coroutine>c__Iterator1A.<>f__this = this;
		return <Playing_Coroutine>c__Iterator1A;
	}

	public override void OnGameStateChange(GameState old, GameState newState)
	{
		if (newState == GameState.Game_Over)
		{
			PostEffectX.FinishGray();
			if (AudioMgr.Instance.isUsingWWise())
			{
			}
			Singleton<TriggerManager>.Instance.SendGameStateEvent(GameEvent.GameOver);
			Singleton<BattleSettlementMgr>.Instance.OnGameOver();
			new GameOverMsg();
		}
		if (newState == GameState.Game_Exit)
		{
			PostEffectX.FinishGray();
			new ExitBattleMsg();
		}
		if (newState == GameState.Game_Playing)
		{
			this._coroutineManager.StartCoroutine(this.Playing_Coroutine(), true);
			this._coroutineManager.StartCoroutine(this.Update_Coroutine(), true);
		}
	}

	public override void Uninit()
	{
		this._coroutineManager.StopAllCoroutine();
		PostEffectX.FinishGray();
	}

	private void PlayBirthEffect()
	{
		StrategyManager.Instance.GameStartState();
		if (!Singleton<PvpManager>.Instance.IsGlobalObserver)
		{
			Units player = MapManager.Instance.GetPlayer();
			if (null != player)
			{
			}
			this._coroutineManager.StartCoroutine(this.PlayBirthEffect_Coroutine(TeamType.LM), true);
			this._coroutineManager.StartCoroutine(this.PlayBirthEffect_Coroutine(TeamType.BL), true);
		}
		else
		{
			this._coroutineManager.StartCoroutine(this.PlayBirthEffect_Coroutine(TeamType.LM), true);
			this._coroutineManager.StartCoroutine(this.PlayBirthEffect_Coroutine(TeamType.BL), true);
		}
	}

	[DebuggerHidden]
	private IEnumerator PlayBirthEffect_Coroutine(TeamType type)
	{
		GameEventModule.<PlayBirthEffect_Coroutine>c__Iterator1A8 <PlayBirthEffect_Coroutine>c__Iterator1A = new GameEventModule.<PlayBirthEffect_Coroutine>c__Iterator1A8();
		<PlayBirthEffect_Coroutine>c__Iterator1A.type = type;
		<PlayBirthEffect_Coroutine>c__Iterator1A.<$>type = type;
		return <PlayBirthEffect_Coroutine>c__Iterator1A;
	}

	[DebuggerHidden]
	private IEnumerator Update_Coroutine()
	{
		return new GameEventModule.<Update_Coroutine>c__Iterator1A9();
	}
}
