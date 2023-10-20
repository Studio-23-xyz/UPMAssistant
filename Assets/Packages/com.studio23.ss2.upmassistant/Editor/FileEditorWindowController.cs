using Cysharp.Threading.Tasks;
using Studio23.SS2.UPMAssistant.Editor.Utilities;
using UnityEditor;
using UnityEngine;

namespace Studio23.SS2.UPMAssistant.Editor
{
    public class FileEditorWindowController : EditorWindow
    {
        private static string _markupText = "";
        private static string _filePath = "";
        private readonly float _windowWidth = 600;
        private readonly float _windowHeight = 600;
        private readonly float _textAreaHeight = 500;
        private static FileEditorWindowController _instance;

        public static FileEditorWindowController Instance
        {
            get
            {
                if (_instance == null) _instance = CreateInstance<FileEditorWindowController>();

                return _instance;
            }
        }

        public async void ShowWindow(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                _filePath = path;
                var window = GetWindow<FileEditorWindowController>("UPM Editor Window");
                window.minSize = new Vector2(_windowWidth, _windowHeight);
                _markupText = await _filePath.ReadFileAsync();
                window.Show();
            }
            else
            {
                Debug.LogError("File path is empty!");
            }
        }

        private void OnGUI()
        {
            GUILayout.Label("UPM Editor", EditorStyles.boldLabel);
            GUILayout.Label($"File Location: {_filePath}", EditorStyles.label);

            _markupText = EditorGUILayout.TextField(_markupText, GUILayout.Height(_textAreaHeight));

            var btnStatus = !string.IsNullOrEmpty(_filePath) && !string.IsNullOrEmpty(_markupText);
            EditorGUI.BeginDisabledGroup(!btnStatus);
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Save", GUILayout.Height(40)))
            {
                if (!string.IsNullOrEmpty(_filePath))
                    _filePath.WriteFileAsync(_markupText).ContinueWith(() => { AssetDatabase.Refresh(); });
                else
                    Debug.LogError("File path is empty!");
            }

            GUI.backgroundColor = Color.white; // Reset the background color
            EditorGUI.EndDisabledGroup();
        }

        private void OnDisable()
        {
            if (!string.IsNullOrEmpty(_filePath))
                _filePath.WriteFileAsync(_markupText).ContinueWith(() => { AssetDatabase.Refresh(); });
            else
                Debug.LogError("File path is empty!");
        }
    }
}