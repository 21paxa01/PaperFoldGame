using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(DecalMaster))]
public class DecalMasterEditor : Editor
{
    DecalMaster decalMaster;

    private void OnEnable()
    {
        if (decalMaster == null)
            decalMaster = (DecalMaster)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(10);

        if(GUILayout.Button("Next Paper"))
        {
            RandomizeNextPaper();
        }
    }

    private void RandomizeNextPaper()
    {
        decalMaster.EnableNextPaper();
    }
}
#endif