using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 六边形坐标系
/// 一个尖朝上的六边形 X+ 朝右 ， Z+朝右上 
/// </summary> 
[System.Serializable]
public struct HexCoordinates {
    [SerializeField]
    private int x, z;

	public int X { get { return x; } }
	public int Z { get { return z; } }
    /// <summary>
    /// 六边形左边系的第三个维度，不是三维中的Y轴
    /// </summary>
    public int Y { get { return -X - Z; } }

    public HexCoordinates(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    /// <summary>
    /// 使用普通偏移坐标
    /// </summary>
    public static HexCoordinates FromOffsetCoordinates(int x, int z)
    {
        return new HexCoordinates(x - z / 2, z);
    }

    public override string ToString()
    {
        return "(" + X.ToString() + ", " + Y.ToString() + ", " + Z.ToString() + ")";
    }

    public string ToStringOnSeparateLines()
    {
        return X.ToString() + "\n" + Y.ToString() + "\n" + Z.ToString();
    }

    /// <summary>
    /// 将一个世界坐标系的值转为一个六边形坐标系的值
    /// </summary>
    public static HexCoordinates FromPosition(Vector3 position)
    {
        // x除以六边形的水平宽
        float x = position.x / (HexMetrics.innerRadius * 2f);
        // if(z == 0) y = -x
        float y = -x;

        // 算出z，x,y计算偏差
        float offset = position.z / (HexMetrics.outerRadius * 3f);
        x -= offset;
        y -= offset;

        int iX = Mathf.RoundToInt(x);
        int iY = Mathf.RoundToInt(y);
        int iZ = Mathf.RoundToInt(-x - y);
        if (iX + iY + iZ != 0)
        {
            // 离一个单元的中心距离越远，取整的幅度越大。所以这里假设取整幅度最大的坐标是错误的
            float dX = Mathf.Abs(x - iX);
            float dY = Mathf.Abs(y - iY);
            float dZ = Mathf.Abs(-x - y - iZ);
            if (dX > dY && dX > dZ)
            {
                iX = -iY - iZ;
            }
            else if (dZ > dY)
            {
                iZ = -iX - iY;
            }
        }
        return new HexCoordinates(iX, iZ);
    }
}