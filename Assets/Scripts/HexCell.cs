using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 六边形单元
/// </summary>
public class HexCell : MonoBehaviour {
    public HexCoordinates coordinates;
    public Color color;

    [SerializeField]
    private HexCell[] neighbors;

    /// <summary>
    /// 获取对应方向的邻居
    /// </summary>
    /// <param name="direction">方向</param>
    public HexCell GetNeighbor(HexDirection direction)
    {
        return neighbors[(int)direction];
    }

    /// <summary>
    /// 设置对应方向的邻居
    /// </summary>
    public void SetNeighbor(HexDirection direction, HexCell cell)
    {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this; // 设置相对方向
    }
}
