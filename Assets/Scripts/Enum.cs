using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HexDirection
{
    NE,  // 东北
    E,   // 东
    SE,  // 东南
    SW,  // 西南
    W,   // 西
    NW,  // 西北
}

public enum HexEdgeType
{
    Flat,   // 平面
    Slope,  // 斜坡
    Cliff   // 陡坡
}