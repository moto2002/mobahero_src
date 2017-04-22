using System;

namespace Assets.Scripts.Character.Control
{
    /// <summary>
    /// 控制事件类型
    /// </summary>
	public enum EControlType
	{
		eNull,
        /// <summary>
        /// 按下
        /// </summary>
		eDown,
        /// <summary>
        /// 按住
        /// </summary>
		ePress,
        /// <summary>
        /// 按下后放开
        /// </summary>
		eUp,
        /// <summary>
        /// 移动结束
        /// </summary>
		eMoveEnd
	}
}
