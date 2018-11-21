using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 六边形网格块
/// </summary>
public class HexGridChunk : MonoBehaviour {

    private HexCell[] cells;

    private HexMesh hexMesh;
    private Canvas gridCanvas;

    private void Awake()
    {
        gridCanvas = GetComponentInChildren<Canvas>();
        hexMesh = GetComponentInChildren<HexMesh>();

        cells = new HexCell[HexMetrics.chunkSizeX * HexMetrics.chunkSizeZ];
        ShowUI(false);
    }

    private void Start()
    {
        //hexMesh.Triangulate(cells);
    }

    private void LateUpdate()
    {
        hexMesh.Triangulate(cells);
        this.enabled = false;
    }

    /// <summary>
    /// 将地图单元加入地图块
    /// </summary>
    /// <param name="index"></param>
    /// <param name="cell"></param>
    public void AddCell(int index, HexCell cell)
    {
        cells[index] = cell;
        cell.chunk = this;
        cell.transform.SetParent(transform, false);
        cell.uiRect.SetParent(gridCanvas.transform, false);
    }

    /// <summary>
    /// 刷新地图块
    /// </summary>
    public void Refresh()
    {
        this.enabled = true;
    }
    
    /// <summary>
    /// 控制地图块自身UI的显示
    /// </summary>
    /// <param name="visible"></param>
    public void ShowUI(bool visible)
    {
        gridCanvas.gameObject.SetActive(visible);
    }
}
