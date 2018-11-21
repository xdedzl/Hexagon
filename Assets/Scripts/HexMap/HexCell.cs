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

            // 保证改变高度时河流不会从低到高 
            if (HasOutgoingRiver && elevation < GetNeighbor(outgoingRiver).elevation)
            {
                RemoveOutgoingRiver();
            }
            if (HasIncomingRiver && elevation < GetNeighbor(incomingRiver).elevation)
            {
                RemoveIncomingRiver();
            }

            Refresh();
        }
    }

    public Vector3 Position { get { return transform.localPosition; } }

    /// <summary>
    /// 是否为一条河流的起点
    /// </summary>
    private bool hasIncomingRiver;
    /// <summary>
    /// 是否为一条河流终点
    /// </summary>
    private bool hasOutgoingRiver;
    /// <summary>
    /// 河流流入方向
    /// </summary>
    private HexDirection incomingRiver;
    /// <summary>
    /// 河流流出方向
    /// </summary>
    private HexDirection outgoingRiver;
    public bool HasIncomingRiver
    {
        get
        {
            return hasIncomingRiver;
        }
    }
    public bool HasOutgoingRiver
    {
        get
        {
            return hasOutgoingRiver;
        }
    }
    public HexDirection IncomingRiver
    {
        get
        {
            return incomingRiver;
        }
    }
    public HexDirection OutgoingRiver
    {
        get
        {
            return outgoingRiver;
        }
    }
    /// <summary>
    /// 是否有河流流过
    /// </summary>
    public bool HasRiver { get { return hasIncomingRiver || HasOutgoingRiver; } }
    /// <summary>
    /// 是否拥有河流的首段或尾端
    /// </summary>
    public bool HasRiverBeginOrEnd { get { return hasIncomingRiver || hasOutgoingRiver; } }

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
    /// 河流是否流经某个方向的边
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public bool HasRiverThroughEdge(HexDirection direction)
    {
        return hasIncomingRiver && incomingRiver == direction ||
            HasOutgoingRiver && outgoingRiver == direction;
    }

    /// <summary>
    /// 添加河流流出部分
    /// </summary>
    /// <param name="direction"></param>
    public void SetOutgoingRiver(HexDirection direction)
    {
        // 指定方向已经有流出了直接返回
        if (hasOutgoingRiver && outgoingRiver == direction)
            return;

        // 指定方向单元不能比自身高
        HexCell neighbor = GetNeighbor(direction);
        if (!neighbor || elevation < neighbor.elevation)
            return;

        // 移除原先的流出
        RemoveOutgoingRiver();
        if (hasIncomingRiver && incomingRiver == direction)
        {
            RemoveIncomingRiver();
        }

        // 设置新的流出
        hasOutgoingRiver = true;
        outgoingRiver = direction;
        RefreshSelfOnly();

        // 给邻居设置流入
        neighbor.RemoveIncomingRiver();
        neighbor.hasIncomingRiver = true;
        neighbor.incomingRiver = direction.Opposite();
        neighbor.RefreshSelfOnly();
    }

    /// <summary>
    /// 移除河流流出部分
    /// </summary>
    public void RemoveOutgoingRiver()
    {
        if (!hasOutgoingRiver)
            return;
        hasIncomingRiver = false;
        RefreshSelfOnly();

        HexCell neighbor = GetNeighbor(outgoingRiver);
        neighbor.hasIncomingRiver = false;
        neighbor.RefreshSelfOnly();
    }

    /// <summary>
    /// 移除河流流入部分
    /// </summary>
    public void RemoveIncomingRiver()
    {
        if (!hasIncomingRiver)
        {
            return;
        }
        hasIncomingRiver = false;
        RefreshSelfOnly();

        HexCell neighbor = GetNeighbor(incomingRiver);
        neighbor.hasOutgoingRiver = false;
        neighbor.RefreshSelfOnly();
    }

    /// <summary>
    /// 移除河流流入和流出部分
    /// </summary>
    public void RemoveRiver()
    {
        RemoveOutgoingRiver();
        RemoveIncomingRiver();
    }

    /// <summary>
    /// 刷新地图块自身以及相邻块
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

    /// <summary>
    /// 仅刷新地图块自身
    /// </summary>
    private void RefreshSelfOnly()
    {
        chunk.Refresh();
    }
}
