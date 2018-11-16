using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMapCamera : MonoBehaviour {

    /// <summary>
    /// 用于控制相机旋转
    /// </summary>
    private Transform swivel;
    /// <summary>
    /// 用于控制相机位置
    /// </summary>
    private Transform stick;

    /// <summary>
    /// 焦距 0表示镜头拉到最远，1表示镜头拉到最近
    /// </summary>
    private float zoom = 1;
    /// <summary>
    /// 相机架最大距离
    /// </summary>
    public float stickMinZoom;
    /// <summary>
    /// 相机架最小距离
    /// </summary>
    public float stickMaxZoom;
    /// <summary>
    /// 最大旋转量
    /// </summary>
    public float swivelMinZoom;
    /// <summary>
    /// 最小旋转量
    /// </summary>
    public float swivelMaxZoom;

    /// <summary>
    /// 相机移动速度
    /// </summary>
    public float moveSpeed;
    /// <summary>
    /// 最大移动速度
    /// </summary>
    public float moveSpeedMinZoom;
    /// <summary>
    /// 最小移动速度
    /// </summary>
    public float moveSpeedMaxZoom;

    /// <summary>
    /// 地图
    /// </summary>
    public HexGrid grid;

    private void Awake()
    {
        swivel = transform.GetChild(0);
        stick = swivel.GetChild(0);
    }

    private void Update()
    {
        // 控制焦距
        float zoomDelta = Input.GetAxis("Mouse ScrollWheel");
        if(zoomDelta != 0f)
        {
            AdjustZoom(zoomDelta);
        }

        // 控制位置
        float xDelta = Input.GetAxis("Horizontal");
        float zDelta = Input.GetAxis("Vertical");
        if (xDelta != 0f || zDelta != 0f)
        {
            AdjustPosition(xDelta, zDelta);
        }
    }

    /// <summary>
    /// 调整焦距
    /// </summary>
    /// <param name="delta"></param>
    private void AdjustZoom(float delta)
    {
        zoom = Mathf.Clamp01(zoom + delta);

        float distance = Mathf.Lerp(stickMinZoom, stickMaxZoom, zoom);
        stick.localPosition = new Vector3(0f, 0f, distance);

        float angle = Mathf.Lerp(swivelMinZoom, swivelMaxZoom, zoom);
        swivel.localRotation = Quaternion.Euler(angle, 0f, 0f);
    }

    /// <summary>
    /// 控制位置
    /// </summary>
    /// <param name="xDelta"></param>
    /// <param name="zDelta"></param>
    void AdjustPosition(float xDelta, float zDelta)
    {
        Vector3 dirction = new Vector3(xDelta, 0f, zDelta).normalized;
        float damping = Mathf.Max(Mathf.Abs(xDelta), Mathf.Abs(zDelta));  // 使用两个方向的大值控制速度
        float distance = Mathf.Lerp(moveSpeedMinZoom, moveSpeedMaxZoom, zoom) * damping * Time.deltaTime;  // 距离越远，速度越快

        Vector3 position = transform.localPosition;
        position += dirction * distance;
        transform.localPosition = position;

        transform.localPosition = ClampPosition(position);
    }

    /// <summary>
    /// 限定位置在地图范围内
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    private Vector3 ClampPosition(Vector3 position)
    {
        float xMax = (grid.chunkCountX * HexMetrics.chunkSizeX - 0.5f) * (2f * HexMetrics.innerRadius);
        position.x = Mathf.Clamp(position.x, 0f, xMax);
        float zMax = (grid.chunkCountZ * HexMetrics.chunkSizeZ - 1) * (1.5f * HexMetrics.outerRadius);
        position.z = Mathf.Clamp(position.z, 0f, zMax);

        return position;
    }
}
