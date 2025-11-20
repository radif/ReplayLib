#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Replay.Utils;

namespace Replay.Utils
{

    [CustomEditor(typeof(DeepLinkReceiver), true)]
    public class DeepLinkReceiverEditor : Editor
    {
        private string _deepLinkURL = "";

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (Application.isPlaying)
            {
                var receiver = (DeepLinkReceiver)target;
                if (receiver != null)
                {
                    GUILayout.Space(20);
                    GUILayout.Label("Test Deep Link:", EditorStyles.boldLabel);
                    _deepLinkURL = EditorGUILayout.TextField("URL: ", _deepLinkURL);
                    if (GUILayout.Button("Trigger Deep Link"))
                        receiver.onDeepLinkActivated?.Invoke(_deepLinkURL);
                }
            }
        }
    }
}
#endif