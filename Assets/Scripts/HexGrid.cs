using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 六边形地图
/// </summary>
public class HexGrid : MonoBehaviour
{
    /// <summary>
    /// 横向六边形数量
    /// </summary>
    private int cellCountX = 6;
    /// <summary>
    /// 纵向六边形数量
    /// </summary>
    private int cellCountZ = 6;
    /// <summary>
    /// 地图块数量
    /// </summary>
    public int chunkCountX = 4;
    public int chunkCountZ = 3;

    /// <summary>
    /// 地图单元预制
    /// </summary>
    public HexCell cellPrefab;
    private HexCell[] cells;

    public Text cellLabelPrefab;
    //private Canvas gridCanvas;
    //private HexMesh hexMesh;

    /// <summary>
    /// 地图块预制
    /// </summary>
    public HexGridChunk chunkPrefab;
    private HexGridChunk[] chunks;

    public Color defaultColor = Color.white;

    /// <summary>
    /// 噪声
    /// </summary>
    public Texture2D noiseSource;

    private void Awake()
    {
        HexMetrics.noiseSource = noiseSource;   // 赋值噪音

        //gridCanvas = GetComponentInChildren<Canvas>();
        //hexMesh = GetComponentInChildren<HexMesh>();

        cellCountX = chunkCountX * HexMetrics.chunkSizeX;
        cellCountZ = chunkCountZ * HexMetrics.chunkSizeZ;

        CreateChunks();
        CreatCells();
    }

    /// <summary>
    /// 重新编译时会被调用
    /// </summary>
    void OnEnable()
    {
        HexMetrics.noiseSource = noiseSource;
    }

    //private void Start()
    //{
    //    hexMesh.Triangulate(cells);
    //}

    #region 测试
    HexCell cell;
    GameObject obj;
    private void Update()
    {
        if(cell == null)
        {
            cell = cells[0];
            obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.transform.position = cell.transform.position;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            cell = cell.GetNeighbor(HexDirection.NE);// 东北
            obj.transform.position = cell.transform.position;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            cell = cell.GetNeighbor(HexDirection.SW);// 西南
            obj.transform.position = cell.transform.position;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            cell = cell.GetNeighbor(HexDirection.W);// xi
            obj.transform.position = cell.transform.position;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            cell = cell.GetNeighbor(HexDirection.E);// 东
            obj.transform.position = cell.transform.position;
        }
    }

    #endregion

    /// <summary>
    /// 创建一群地图块
    /// </summary>
    private void CreateChunks()
    {
        chunks = new HexGridChunk[chunkCountX * chunkCountZ];

        for (int z = 0, i = 0; z < chunkCountZ; z++)
        {
            for (int x = 0; x < chunkCountX; x++)
            {
                HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
                chunk.transform.SetParent(transform);
            }
        }
    }

    /// <summary>
    /// 创建地图
    /// </summary>
    private void CreatCells()
    {
        cells = new HexCell[cellCountZ * cellCountX];

        for (int z = 0, i = 0; z < cellCountZ; z++)
        {
            for (int x = 0; x < cellCountX; x++)
            {
                CreateCell(x, z, i++);
            }
        }
    }

    /// <summary>
    /// 创建单元
    /// </summary>
    private void CreateCell(int x, int z, int i)
    {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        HexCell cell = cells[i] = Instantiate(cellPrefab);
        //cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.Color = defaultColor;

        // 添加邻居
        if (x > 0)
        {
            cell.SetNeighbor(HexDirection.W, cells[i - 1]);   // 设置西边邻居
        }
        if (z > 0)
        {
            if ((z & 1) == 0)  // z 为偶数
            {
                cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX]);  // 设置东南邻居
                if (x > 0)
                {
                    cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX - 1]);
                }
            }
            else
            {
                cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX]);
                if (x < cellCountX - 1)
                {
                    cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX + 1]);
                }
            }
        }

        // 单元上显示六边形坐标
        Text label = Instantiate(cellLabelPrefab);
        //label.rectTransform.SetParent(gridCanvas.transform, false);
        label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
        label.text = cell.coordinates.ToStringOnSeparateLines();
        cell.uiRect = label.rectTransform;

        cell.Elevation = 0;

        AddCellToChunk(x, z, cell);
    }

    /// <summary>
    /// 通过坐标找到对应的块
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <param name="cell"></param>
    private void AddCellToChunk(int x, int z, HexCell cell)
    {
        int chunkX = x / HexMetrics.chunkSizeX;
        int chunkZ = z / HexMetrics.chunkSizeZ;
        HexGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];

        int localX = x - chunkX * HexMetrics.chunkSizeX;
        int localZ = z - chunkZ * HexMetrics.chunkSizeZ;
        chunk.AddCell(localX + localZ * HexMetrics.chunkSizeX, cell);
    }

    /// <summary>
    /// 改变某一位置对应的六边形的颜色
    /// </summary>
    public HexCell GetCell(Vector3 position)
    {
        // 把世界坐标系转化成本地坐标系
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index = coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;
        return cells[index];
    }

    /// <summary>
    /// 刷新网格
    /// </summary>
    //public void Refresh()
    //{
    //    hexMesh.Triangulate(cells);
    //}
}
