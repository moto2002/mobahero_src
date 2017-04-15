using System;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public abstract class BasePlayer : MonoBehaviour, IPlayer
	{
		public Callback<int> OnPlayCallback;

		public Callback<int> OnStopCallback;

		public Callback<int> OnDestroyCallback;

		private void Awake()
		{
		}

		private void Start()
		{
		}

		private void OnDestroy()
		{
			this.OnPlayCallback = null;
			this.OnStopCallback = null;
			this.OnDestroyCallback = null;
		}

		public abstract void Play();

		public abstract void Stop();
	}
}
