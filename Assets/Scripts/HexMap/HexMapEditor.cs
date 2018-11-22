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

    /// <summary>
    /// 
    /// </summary>
    private OptionToggle riverMode;
    /// <summary>
    /// 是否拖拽
    /// </summary>
    private bool isDrag;
    /// <summary>
    /// 拖拽方向
    /// </summary>
    HexDirection dragDirection;
    /// <summary>
    /// 上一个拖拽单元
    /// </summary>
    HexCell previousCell;


    void Awake()
    {
        SelectColor(0);

        transform.FindRecursive("BrushSizeSlider").GetComponent<Slider>().onValueChanged.AddListener(SetBrushSize);
        transform.Find("RiverPanel").GetChild(0).GetComponent<Toggle>().onValueChanged.AddListener((a) =>
        {
            if (a) SetRiverMode(0);
        });
        transform.Find("RiverPanel").GetChild(1).GetComponent<Toggle>().onValueChanged.AddListener((a) =>
        {
            if (a) SetRiverMode(1);
        });
        transform.Find("RiverPanel").GetChild(2).GetComponent<Toggle>().onValueChanged.AddListener((a) =>
        {
            if (a) SetRiverMode(2);
        });
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            HandleInput();
        }
        else
        {
            previousCell = null;
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
            HexCell currentCell = hexGrid.GetCell(hit.point);
            if (previousCell && previousCell != currentCell)
            {
                ValidateDrag(currentCell);
            }
            else
            {
                isDrag = false;
            }
            EditCells(currentCell);
            previousCell = currentCell;
            isDrag = true;
        }
        else
        {
            previousCell = null;
        }
    }

    /// <summary>
    /// 判断某一个单元是否为上一次存储的单元的邻居
    /// </summary>
    /// <param name="currentCell"></param>
    private void ValidateDrag(HexCell currentCell)
    {
        for (dragDirection = HexDirection.NE; dragDirection <= HexDirection.NW; dragDirection++)
        {
            if (previousCell.GetNeighbor(dragDirection) == currentCell)
            {
                isDrag = true;
                return;
            }
        }
        isDrag = false;
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

            if (riverMode == OptionToggle.No)
            {
                cell.RemoveRiver();
            }
            else if (isDrag && riverMode == OptionToggle.Yes)
            {
                HexCell otherCell = cell.GetNeighbor(dragDirection.Opposite());
                if (otherCell)
                    previousCell.SetOutgoingRiver(dragDirection);
            }
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

    /// <summary>
    /// 设置河流模式
    /// </summary>
    /// <param name="mode"></param>
    public void SetRiverMode(int mode)
    {
        riverMode = (OptionToggle)mode;
    }

    /// <summary>
    /// 河流编辑枚举
    /// </summary>
    enum OptionToggle
    {
        Ignore, Yes, No
    }
}
