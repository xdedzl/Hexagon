using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExternFun {

    /// <summary>
    /// 获取反方向
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
	public static HexDirection Opposite(this HexDirection direction)
    {
        return (int)direction < 3 ? (direction + 3) : direction - 3;
    }

    /// <summary>
    /// 获取前一个方向
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public static HexDirection Previous(this HexDirection direction)
    {
        return direction == HexDirection.NE ? HexDirection.NW : (direction - 1);
    }

    /// <summary>
    /// 获取下一个方向
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public static HexDirection Next(this HexDirection direction)
    {
        return direction == HexDirection.NW ? HexDirection.NE : (direction + 1);
    }
}
