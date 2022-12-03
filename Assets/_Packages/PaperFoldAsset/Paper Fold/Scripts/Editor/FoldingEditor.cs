using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(Folding))]
public class FoldingEditor : Editor
{
    Folding folding;

    private void OnEnable()
    {
        if (folding == null)
            folding = (Folding)target;
    }

    private void OnSceneGUI()
    {
        if (!folding.showGizmos) return;

        DrawRotationAxisPositionHandle();
        DrawRotationAxis();
        DrawRotationAxisPlaneNormal();
    }

    private void DrawRotationAxisPositionHandle()
    {
        RotationAxis rt = folding.GetRotationAxis();

        EditorGUI.BeginChangeCheck();

        rt.position = Handles.PositionHandle(rt.position, Quaternion.identity);

        if(EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(folding, "RotationAxis position change.");
            EditorUtility.SetDirty(folding);
        }

        folding.SetRotationAxis(rt);
    }

    private void DrawRotationAxis()
    {
        RotationAxis rotationAxis = folding.GetRotationAxis();

        Vector3[] fourPlanePoints = PlaneUtils.GetFourPlanePoints(rotationAxis.position, rotationAxis.angle);

        Handles.color = Color.blue;

        Handles.DrawLine(fourPlanePoints[0], fourPlanePoints[1]);
        Handles.DrawLine(fourPlanePoints[1], fourPlanePoints[2]);
        Handles.DrawLine(fourPlanePoints[2], fourPlanePoints[3]);
        Handles.DrawLine(fourPlanePoints[3], fourPlanePoints[0]);
    }

    private void DrawRotationAxisPlaneNormal()
    {
        RotationAxis rotationAxis = folding.GetRotationAxis();
        Handles.color = Color.blue;
        Handles.ArrowHandleCap(0, rotationAxis.position, Quaternion.Euler(0, -rotationAxis.angle + 180, 0), 0.5f, EventType.Repaint);
    }
}
#endif