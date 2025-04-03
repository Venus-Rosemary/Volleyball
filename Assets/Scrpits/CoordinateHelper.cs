using UnityEngine;

public static class CoordinateHelper
{
    public static readonly float SceneRotationY = 124f; // 场景旋转角度

    public static Vector3 WorldToGameSpace(Vector3 worldPosition)
    {
        return Quaternion.Euler(0, -SceneRotationY, 0) * worldPosition;
    }

    public static Vector3 GameToWorldSpace(Vector3 gamePosition)
    {
        return Quaternion.Euler(0, SceneRotationY, 0) * gamePosition;
    }
}