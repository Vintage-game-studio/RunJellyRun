using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(BlockSeqence))]
public class GenetatorUI : Editor
{
    public override void OnInspectorGUI()
    {
        BlockSeqence myScript = (BlockSeqence)target;
        if (GUILayout.Button("Generate!"))
        {
            myScript.Generate();
        }

        if (GUILayout.Button("Delete childs"))
        {
            myScript.DeleteBlocks();
        }

        DrawDefaultInspector();
    }
}

[CustomEditor(typeof(BlockRandomSequence))]
public class rGenetatorUI : Editor
{
    public override void OnInspectorGUI()
    {
        BlockRandomSequence myScript = (BlockRandomSequence)target;
        if (GUILayout.Button("Generate!"))
        {
            myScript.Generate();
        }
        if (GUILayout.Button("Delete childs"))
        {
            myScript.DeleteBlocks();
        }

        DrawDefaultInspector();
    }
}

[CustomEditor(typeof(Replacer))]
public class rGenetatorUI2 : Editor
{
    public override void OnInspectorGUI()
    {
        Replacer myScript = (Replacer)target;
        if (GUILayout.Button("Replace!"))
        {
            myScript.Replace();
        }

        DrawDefaultInspector();
    }
}
[CustomEditor(typeof(AdvanceBlock))]
public class rGenetatorUI23 : Editor
{
    public override void OnInspectorGUI()
    {
        AdvanceBlock myScript = (AdvanceBlock)target;
        if (GUILayout.Button("Run!"))
        {
            Debug.ClearDeveloperConsole();
            myScript.Generate();
        }
        if (GUILayout.Button("Delete childs"))
        {
            myScript.DeleteBlocks();
        }
        
        DrawDefaultInspector();
        myScript.Program= EditorGUILayout.TextArea(myScript.Program);
    }
}
[CustomEditor(typeof(LineScatter))]
public class rGenetatorUI234 : Editor
{
    public override void OnInspectorGUI()
    {
        LineScatter myScript = (LineScatter)target;

        if (GUILayout.Button("Scatter!"))
        {
            Debug.ClearDeveloperConsole();
            myScript.Scatter();
        }

        DrawDefaultInspector();
    }
}