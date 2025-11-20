#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Replay.Utils;

[CustomEditor(typeof(MonoBehaviour), true)]
public class DebugLoggableEditor : Editor
{
    static bool _printsLogs = false;
    void OnEnable()
    {
        if (target is IDebugLoggable)
            EditorApplication.update += OnEditorUpdate;
    }

    void OnDisable()
    {
        if (target is IDebugLoggable)
            EditorApplication.update -= OnEditorUpdate;
    }
    private float _lastUpdateTime;
    private bool _shouldUpdate = false;
    void OnEditorUpdate()
    {
        if (_shouldUpdate && target is IDebugLoggable)
            if (Application.isPlaying)
            {
                if (Time.time - _lastUpdateTime >= _updateInterval)
                {
                    _lastUpdateTime = Time.time;
                    Repaint();
                }
            }
    }
    static float _updateInterval = 0.1F;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        DrawDebugLoggableInspector();
       
    }
    
    public void DrawDebugLoggableInspector()
    {
        if (target is IDebugLoggable)
        {
            GUILayout.Space(20);
            _shouldUpdate = EditorGUILayout.Toggle("Force Redraw on Update", _shouldUpdate);
            if(_shouldUpdate)
                _updateInterval = EditorGUILayout.Slider("Update Interval", _updateInterval, 0f, 1f);

            string className = target.GetType().Name;
            
            GUILayout.Space(20);
            GUILayout.Label(className + " (IDebugLoggable):");
            _printsLogs = EditorGUILayout.Toggle("Print Logs", _printsLogs);

            if (Application.isPlaying)
            {
                var r = (IDebugLoggable)target;
                if (r == null)
                {
                    GUILayout.Label(className + " is Not Initialized!");
                }
                else
                {
                    GUILayout.Label(r.ToDebugString());
                    GUILayout.Space(20);
                    if(_printsLogs)
                    {
                        GUILayout.Label("Logs:", EditorStyles.boldLabel);
                        GUILayout.Space(20);
                        GUILayout.Label(r.GetLogs());
                    }
                    
                }
            }
            else
            {
                GUILayout.Label("Enter Playmode!");
            }
        }
    }
}
#endif