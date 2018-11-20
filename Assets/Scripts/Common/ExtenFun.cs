using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 这个类管理一系列的拓展函数
/// 自己再开发过程中想到的或者看到的积累
/// </summary>
public static class ExtenFun
{
    #region Vector相关
    public static Vector3 WithX(this Vector3 v, float x)
    {
        return new Vector3(x, v.y, v.z);
    }

    public static Vector3 WithY(this Vector3 v, float y)
    {
        return new Vector3(v.x, y, v.z);
    }

    public static Vector3 WithZ(this Vector3 v, float z)
    {
        return new Vector3(v.x, v.y, z);
    }
    #endregion

    #region Transform

    /// <summary>
    /// 找寻名字为name的子物体
    /// </summary>
    public static Transform FindRecursive(this Transform transform, string name)
    {
        if (transform.name.Equals(name))
        {
            return transform;
        }
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform transform2 = transform.GetChild(i).FindRecursive(name);
            if (transform2 != null)
            {
                return transform2;
            }
        }
        return null;
    }
    #endregion

    #region Rigibogy

    /// <summary>
    /// 重置刚体
    /// </summary>
    public static void ResetDynamics(this Rigidbody body)
    {
        Vector3 zero = Vector3.zero;
        body.angularVelocity = zero;
        body.velocity = zero;
    }

    #endregion

    #region Quaternion 加减貌似只在其他两个轴为0的时候起作用

    /// <summary>
    /// 将q加上rotation并返回
    /// </summary>
    public static Quaternion AddRotation(this Quaternion q, Quaternion rotation)
    {
        return q * rotation;
    }
    public static Quaternion AddRotation(this Quaternion q, Vector3 angle)
    {
        return q * Quaternion.Euler(angle);
    }

    /// <summary>
    /// 将减去rotation并返回
    /// </summary>
    public static Quaternion SubtractRotation(this Quaternion q, Quaternion rotation)
    {
        return q * Quaternion.Inverse(rotation);
    }
    public static Quaternion SubtractRotation(this Quaternion q, Vector3 angle)
    {
        return q * Quaternion.Inverse(Quaternion.Euler(angle));
    }

    #endregion
}
