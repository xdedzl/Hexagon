using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 六边形单元
/// </summary>
public class HexCell : MonoBehaviour {

    /// <summary>
    /// 六边形坐标系
    /// </summary>
    public HexCoordinates coordinates;
    /// <summary>
    /// 单元颜色
    /// </summary>
    public Color Color
    {
        get { return color; }
        set { if (color == value) return; color = value; Refresh(); }
    }
    private Color color;
    /// <summary>
    /// 邻居
    /// </summary>
    [SerializeField]
    private HexCell[] neighbors;
    public RectTransform uiRect;
    /// <summary>
    /// 单元所在地图块的引用
    /// </summary>
    public HexGridChunk chunk;

    /// <summary>
    /// 高度等级
    /// </summary>
    private int elevation = int.MinValue;
    public int Elevation
    {
        get
        {
            return elevation;
        }
        set
        {
            if(elevation == value)
            {
                return;
            }
            elevation = value;
            Vector3 position = transform.localPosition;
            position.y = value * HexMetrics.elevationStep;
            position.y += value * (HexMetrics.SampleNoise(position).y * 2 - 1) * HexMetrics.elevationPerturbStrength;
            transform.localPosition = position;

            // 改变坐标显示高度
            Vector3 uiPosition = uiRect.localPosition;
            uiPosition.z = elevation * -HexMetrics.elevationStep;
            uiRect.localPosition = uiPosition;
            Refresh();
        }
    }

    public Vector3 Position { get { return transform.localPosition; } }

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

    /// <summary>
    /// 获取特定方向的连接类型
    /// </summary>
    public HexEdgeType GetEdgeType(HexDirection direction)
    {
        return HexMetrics.GetEdgeType(elevation, neighbors[(int)direction].elevation);
    }

    /// <summary>
    /// 获取与任一其它单元的连接类型
    /// </summary>
    public HexEdgeType GetEdgeType(HexCell otherCell)
    {
        return HexMetrics.GetEdgeType(elevation, otherCell.elevation);
    }

    /// <summary>
    /// 刷新地图块
    /// </summary>
    private void Refresh()
    {
        if (chunk)
        {
            chunk.Refresh();
            for (int i = 0; i < neighbors.Length; i++)
            {
                HexCell neighbor = neighbors[i];
                if(neighbor != null && neighbor.chunk != chunk)
                {
                    neighbor.chunk.Refresh();
                }
            }
        }
    }
}
