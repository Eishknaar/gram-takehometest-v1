
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridHandler), true)]
public class GridHandlerEditor : Editor
{
   

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GridHandler myTarget = (GridHandler)target;
        if (GUILayout.Button("cache everything"))
        {
            myTarget.HackishInitialization();
        }
    }

 
}