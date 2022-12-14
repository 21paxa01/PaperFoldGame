using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Folding : MonoBehaviour
{
    [Header(" Gizmos Settings ")]
    public bool showGizmos;

    [Header(" Settings ")]
    [SerializeField] private RotationAxis rotationAxis;
    [SerializeField] private float foldingAngle;
    [SerializeField] private Folding[] requiredFoldings;
    private bool folded;

    [Header(" Mesh Settings ")]
    List<int> backFoldedVerticesIndices = new List<int>();
    List<int> frontFoldedVerticesIndices = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public RotationAxis GetRotationAxis()
    {
        return rotationAxis;
    }

    public void SetRotationAxis(RotationAxis rt)
    {
        rotationAxis = rt;
    }

    public float GetRotationAngle()
    {
        return foldingAngle;
    }

    public Folding[] GetRequiredFoldings()
    {
        return requiredFoldings;
    }

    public void SetFoldedState(bool folded)
    {
        this.folded = folded;
    }

    public bool IsFolded()
    {
        return folded;
    }

    public Plane GetFoldingPlane()
    {
        return rotationAxis.AsPlane();
    }

    public Vector3 GetFoldingPosition()
    {
        return rotationAxis.position;
    }



    public void SetBackFoldedVerticesIndices(int[] foldedVerticesIndices)
    {
        backFoldedVerticesIndices.AddRange(foldedVerticesIndices);
    }

    public void SetFrontFoldedVerticesIndices(int[] foldedVerticesIndices)
    {
        frontFoldedVerticesIndices.AddRange(foldedVerticesIndices);
    }



    public int[] GetBackFoldedVerticesIndices()
    {
        return backFoldedVerticesIndices.ToArray();
    }

    public int[] GetFrontFoldedVerticesIndices()
    {
        return frontFoldedVerticesIndices.ToArray();
    }




    public void ClearFoldedVertices()
    {
        backFoldedVerticesIndices.Clear();
        frontFoldedVerticesIndices.Clear();
    }

}

[System.Serializable]
public struct RotationAxis
{
    public Vector3 position;
    public float angle;

    public Vector3 AsVector()
    {
        Vector3[] fourPlanePoints = PlaneUtils.GetFourPlanePoints(position, angle);
        return fourPlanePoints[2] - fourPlanePoints[1];
    }

    public Plane AsPlane()
    {
        Vector3[] fourPlanePoints = PlaneUtils.GetFourPlanePoints(position, angle);
        return new Plane(fourPlanePoints[0], fourPlanePoints[2], fourPlanePoints[1]);
    }

    public Vector3[] AsLine()
    {
        Vector3[] fourPlanePoints = PlaneUtils.GetFourPlanePoints(position, angle);
        return new Vector3[] { fourPlanePoints[2], fourPlanePoints[1] };

    }
}