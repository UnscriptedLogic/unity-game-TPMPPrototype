using UnityEditor;
using UnityEngine;

public class InheritedClassCreator : EditorWindow
{
    [MenuItem("Assets/UnscriptedEngine/Utils/Create Inherited Class From Script")]
    static void CreateInheritedClass()
    {
        // Get the currently selected script in the project window
        MonoScript selectedScript = Selection.activeObject as MonoScript;

        if (selectedScript == null)
        {
            Debug.LogError("Please select a script in the project window.");
            return;
        }

        // Get the path of the selected script
        string scriptPath = AssetDatabase.GetAssetPath(selectedScript);

        // Ask the user for the new script's name
        string newClassName = EditorGUILayout.DelayedTextField("New Class Name", "MyDerivedClass");

        if (string.IsNullOrEmpty(newClassName))
        {
            Debug.LogError("Please enter a valid class name.");
            return;
        }

        // Create the inherited class
        string newScriptPath = scriptPath.Replace(selectedScript.name + ".cs", newClassName + ".cs");

        AssetDatabase.CopyAsset(scriptPath, newScriptPath);

        // Refresh the asset database to make sure the new script is recognized
        AssetDatabase.Refresh();

        Debug.Log("Inherited class created: " + newClassName);
    }
}
