using System;
using UnityEngine;

namespace MobaFrame.SkillAction
{
    /// <summary>
    /// 播放器基类
    /// </summary>
	public abstract class BasePlayer : MonoBehaviour, IPlayer
	{
        /// <summary>
        /// 开始播放回调
        /// </summary>
		public Callback<int> OnPlayCallback;
        /// <summary>
        /// 停止播放回调
        /// </summary>
		public Callback<int> OnStopCallback;
        /// <summary>
        /// 销毁播放回调
        /// </summary>
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
        /// <summary>
        /// 开始播放接口
        /// </summary>
		public abstract void Play();
        /// <summary>
        /// 停止播放接口
        /// </summary>
		public abstract void Stop();
	}
}
