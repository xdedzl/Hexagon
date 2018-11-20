using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HexMapEditor : MonoBehaviour {

    public Color[] colors;
    public HexGrid hexGrid;

    /// <summary>
    /// 目标颜色
    /// </summary>
    private Color activeColor;
    /// <summary>
    /// 目标高度
    /// </summary>
    private int activeElevations;
    /// <summary>
    /// 笔刷大小
    /// </summary>
    private int brushSize;

    /// <summary>
    /// 是否改变颜色
    /// </summary>
    private bool applyColor = true;
    /// <summary>
    /// 是否改变高度
    /// </summary>
    private bool applyElevation = true;

    void Awake()
    {
        SelectColor(0);

        transform.FindRecursive("BrushSizeSlider").GetComponent<Slider>().onValueChanged.AddListener(SetBrushSize);
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            HandleInput();
        }
    }

    /// <summary>
    /// 处理输入
    /// </summary>
    private void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            EditCells(hexGrid.GetCell(hit.point));
        }
    }

    /// <summary>
    /// 编辑一组六边形单元
    /// </summary>
    private void EditCells(HexCell center)
    {
        int centerX = center.coordinates.X;
        int centerZ = center.coordinates.Z;

        for (int r = 0, z = centerZ - brushSize; z <= centerZ; z++, r++)
        {
            for (int x = centerX - r; x <= centerX + brushSize; x++)
            {
                EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
            }
        }

        for (int r = 0, z = centerZ + brushSize; z > centerZ; z--, r++)
        {
            for (int x = centerX - brushSize; x <= centerX + r; x++)
            {
                EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
            }
        }
    }

    /// <summary>
    /// 编辑单个六边形单元
    /// </summary>
    /// <param name="cell"></param>
    private void EditCell(HexCell cell)
    {
        if (cell)
        {
            if (applyColor)
                cell.Color = activeColor;
            if (applyElevation)
                cell.Elevation = activeElevations;
        }
    }

    /// <summary>
    /// 选择颜色
    /// </summary>
    /// <param name="index"></param>
    public void SelectColor(int index)
    {
        applyColor = index >= 0;
        if (applyColor)
        {
            activeColor = colors[index];
        }
    }

    /// <summary>
    /// 设置高度
    /// </summary>
    public void SetElevation(float elevation)
    {
        activeElevations = (int)elevation;
    }

    /// <summary>
    /// 设置是否改变高度
    /// </summary>
    public void SetApplyElevation(bool toggle)
    {
        applyElevation = toggle;
    }

    /// <summary>
    /// 改变笔刷大小
    /// </summary>
    /// <param name="size"></param>
    public void SetBrushSize(float size)
    {
        brushSize = (int)size;
    }
}
