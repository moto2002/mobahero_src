using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace MobaHeros.Spawners
{
	public class TitanSpawner : GameSpawner
	{
		[DebuggerHidden]
		protected override IEnumerator Preload_Coroutine(MonoBehaviour mono)
		{
			TitanSpawner.<Preload_Coroutine>c__Iterator1CF <Preload_Coroutine>c__Iterator1CF = new TitanSpawner.<Preload_Coroutine>c__Iterator1CF();
			<Preload_Coroutine>c__Iterator1CF.mono = mono;
			<Preload_Coroutine>c__Iterator1CF.<$>mono = mono;
			<Preload_Coroutine>c__Iterator1CF.<>f__this = this;
			return <Preload_Coroutine>c__Iterator1CF;
		}
	}
}
