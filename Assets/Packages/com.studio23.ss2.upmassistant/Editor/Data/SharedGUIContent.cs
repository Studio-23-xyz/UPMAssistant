using UnityEditor;
using UnityEngine;

namespace Studio23.SS2.UPMAssistant.Editor.Data
{
    [CreateAssetMenu(fileName = "SharedGUIContent", menuName = "Editor/Shared GUI Content")]
    public class SharedGUIContent : ScriptableObject
    {
        // Define variables and methods to handle GUI content
        public void DrawGUIContent()
        {
            GUILayout.Label($" SharedGUIContent", EditorStyles.boldLabel);
        }
    }
}
