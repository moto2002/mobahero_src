using System;
/// <summary>
/// 静态unit组件
/// </summary>
public class StaticUnitComponent : UnitComponent
{
    /// <summary>
    /// 是否需要更新---不需要
    /// </summary>
    /// <returns></returns>
	public override bool needUpdate()
	{
		return false;
	}
}
