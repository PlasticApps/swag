using System.IO;
using UnityEditor;
using UnityEngine;

namespace SW
{
    [CustomEditor(typeof(DialogueContainer))]
    public class DialgoueContainerEditor: Editor
    {

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Generate Asset"))
            {
                string path = EditorUtility.SaveFilePanelInProject("Save Dialogue as Asset", "dialog", "byte", "Save");
                if (!string.IsNullOrEmpty(path))
                {
                    string json = JsonUtility.ToJson(target);
                    File.WriteAllText(path, json);
                }
            }
            DrawDefaultInspector();
        }
    }
}