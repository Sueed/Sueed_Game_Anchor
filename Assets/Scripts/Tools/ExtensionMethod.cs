using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethod
{
    private const float dotTreshold = 0.5f;
    public static bool isFacingTarget(this Transform transform, Transform target)
    {
        var vectorToTarget = target.position - transform.position;
        //向量化
        vectorToTarget.Normalize();

        float dot = Vector3.Dot(transform.forward, vectorToTarget);

        return dot >= dotTreshold;
    }
}
