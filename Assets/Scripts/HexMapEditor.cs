using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HexMapEditor : MonoBehaviour {

    public Color[] colors;

    public HexGrid hexGrid;

    private Color activeColor;
    private int activeElevations; // 高度

    void Awake()
    {
        SelectColor(0);
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            HandleInput();
        }
    }

    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            EditCell(hexGrid.GetCell(hit.point));
        }
    }

    /// <summary>
    /// 选择颜色
    /// </summary>
    /// <param name="index"></param>
    public void SelectColor(int index)
    {
        activeColor = colors[index];
    }

    /// <summary>
    /// 编辑网格
    /// </summary>
    /// <param name="cell"></param>
    private void EditCell(HexCell cell)
    {
        cell.Color = activeColor;
        cell.Elevation = activeElevations;
        //hexGrid.Refresh();
    }

    /// <summary>
    /// 设置高度
    /// </summary>
    public void SetElevation(float elevation)
    {
        activeElevations = (int)elevation;
    }
}
