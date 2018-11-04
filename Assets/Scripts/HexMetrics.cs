using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 六边形度量值   
/// </summary>
public static class HexMetrics {

    /// <summary>
    /// 六边形外径
    /// </summary>
    public const float outerRadius = 10.0f;
    /// <summary>
    /// 六边形内径  内径 = 3^0.5 * 0.5 * 外径
    /// </summary>
    public const float innerRadius = outerRadius * 0.866025404f;
    /// <summary>
    /// 纯色区域占比
    /// </summary>
    public const float solidFactor = 0.75f;
    /// <summary>
    /// 混合区域占比
    /// </summary>
    public const float blendFactor = 1 - solidFactor;
    /// <summary>
    /// 每一阶梯的高度
    /// </summary>
    public const float elevationStep = 5f;
    /// <summary>
    /// 每个斜坡的阶梯数量
    /// </summary>
    public const int terracesPerSlope = 2;
    /// <summary>
    /// 如果有两个阶梯，就会有四个中间点外加一个最高点
    /// </summary>
    public const int terraceSteps = terracesPerSlope * 2 + 1;
    /// <summary>
    /// 每个阶梯在一个斜坡所占的百分比（用于调整水平x，z）
    /// </summary>
    public const float horizontalTerraceStepSize = 1f / terraceSteps;
    /// <summary>
    /// 每个阶梯在一个斜坡所占的百分比（用于调整竖直y）
    /// </summary>
    public const float verticalTerraceStepSize = 1f / (terracesPerSlope + 1);

    /// <summary>
    /// 六边形六个顶点相对中心点的位置
    /// </summary>
    private static Vector3[] corners = {
    new Vector3(0f, 0f, outerRadius),
    new Vector3(innerRadius, 0f, 0.5f * outerRadius),
    new Vector3(innerRadius, 0f, -0.5f * outerRadius),
    new Vector3(0f, 0f, -outerRadius),
    new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
    new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
    new Vector3(0f, 0f, outerRadius),
    };  

    /// <summary>
    /// 获取对应方向网格顶点的相对坐标
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public static Vector3 GetFirstCorner(HexDirection direction)
    {
        return corners[(int)direction];
    }

    /// <summary>
    /// 获取对应方向下一个网格顶点的相对坐标
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public static Vector3 GetSecondCorner(HexDirection direction)
    {
        return corners[(int)direction + 1];
    }

    /// <summary>
    /// 获取纯色内六边形相对方向顶点相对坐标
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public static Vector3 GetFirstSolidCorner(HexDirection direction)
    {
        return corners[(int)direction] * solidFactor;
    }

    /// <summary>
    /// 获取纯色内六边形相对方向下一个顶点相对坐标
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public static Vector3 GetSecondSolidCorner(HexDirection direction)
    {
        return corners[(int)direction + 1] * solidFactor;
    }

    /// <summary>
    /// 获取边缘桥的宽度（这个应该可以用静态变量代替）
    /// </summary>
    public static Vector3 GetBridge(HexDirection direction)
    {
        return (corners[(int)(direction)] + corners[(int)direction + 1])* blendFactor;
    }

    /// <summary>
    /// 用于的到阶梯位置的特殊插值方式
    /// Y坐标必须只在奇数阶梯中改变，不能在偶数阶梯中改变，否则我们不会得到平直的阶地。
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="step"></param>
    /// <returns></returns>
    public static Vector3 TerraceLerp(Vector3 a, Vector3 b, int step)
    {
        float h = step * HexMetrics.horizontalTerraceStepSize;
        a.x += (b.x - a.x) * h;
        a.z += (b.x - a.z) * h;
        float v = ((step + 1) / 2) * HexMetrics.verticalTerraceStepSize;
        a.y += (b.y - a.y) * v;
        return a;
    }

    /// <summary>
    /// 颜色插值
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="step"></param>
    /// <returns></returns>
    public static Color TerraceLerp(Color a, Color b, int step)
    {
        float h = step * HexMetrics.horizontalTerraceStepSize;
        return Color.Lerp(a, b, h);
    }
}
