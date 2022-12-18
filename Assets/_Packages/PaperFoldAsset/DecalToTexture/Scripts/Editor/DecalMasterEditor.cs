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

        GUILayout.Space(30);

        if(GUILayout.Button("Project Single Paper") && !decalMaster.ProjectingActive)
        {
            Debug.Log("Project Single Paper");
            decalMaster.ProjectSinglePaper();
        }

        if (GUILayout.Button("Project All Papers") && !decalMaster.ProjectingActive)
        {
            Debug.Log("Projecting All Papers");
            decalMaster.ProjectAllPapers();
        }

        if(GUILayout.Button("Stop") && decalMaster.ProjectingActive)
        {
            Debug.Log("Stoped");
            decalMaster.StopCurrentProjecting();
        }
            
    }

    private void RandomizeNextPaper()
    {
        decalMaster.EnableNextPaper();
    }
}
#endif