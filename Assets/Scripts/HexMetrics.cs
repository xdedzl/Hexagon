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

    public static Vector3[] corners = {
    new Vector3(0f, 0f, outerRadius),
    new Vector3(innerRadius, 0f, 0.5f * outerRadius),
    new Vector3(innerRadius, 0f, -0.5f * outerRadius),
    new Vector3(0f, 0f, -outerRadius),
    new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
    new Vector3(-innerRadius, 0f, 0.5f * outerRadius)
    };  
}
