using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlaneUtils
{
    public static Vector3[] GetFourPlanePoints(Vector3 position, float angle)
    {
        Vector3[] points = new Vector3[4];

        float cosAngle = Mathf.Cos(angle * Mathf.Deg2Rad);
        float sinAngle = Mathf.Sin(angle * Mathf.Deg2Rad);

        Vector3 rightUnit = Vector3.right * cosAngle;
        Vector3 forwardUnit = Vector3.forward * sinAngle;

        points[0] = position + rightUnit + forwardUnit + Vector3.up;
        points[1] = points[0] + Vector3.down * 2;
        points[2] = position - rightUnit - forwardUnit + Vector3.down;
        points[3] = points[2] + Vector3.up * 2;

        return points;
    }

    
}
