using UnityEditor;
using UnityEngine;

namespace Andtech.Codecast.Editor
{

    public class CodecastWindow : EditorWindow
    {
        string ipAddress;
        int port;
        bool autoStart = false;
        bool sendUnityLogs = false;
        bool rawUnityLogs = false;

        [MenuItem("Custom/Codecast")]
        static void Init()
        {
            CodecastWindow window = (CodecastWindow)EditorWindow.GetWindow(typeof(CodecastWindow));
            window.ReadPrefs();

            window.Show();
        }

        void ReadPrefs()
        {
            ipAddress = EditorPrefs.GetString(nameof(ipAddress), "127.0.0.1");
            port = EditorPrefs.GetInt(nameof(port), 8080);
            autoStart = EditorPrefs.GetBool(nameof(autoStart), false);
            sendUnityLogs = EditorPrefs.GetBool(nameof(sendUnityLogs), false);
            rawUnityLogs = EditorPrefs.GetBool(nameof(rawUnityLogs), false);
        }

        void WritePrefs()
        {
            EditorPrefs.SetString(nameof(ipAddress), ipAddress);
            EditorPrefs.SetInt(nameof(port), port);
            EditorPrefs.SetBool(nameof(autoStart), autoStart);
            EditorPrefs.SetBool(nameof(sendUnityLogs), sendUnityLogs);
            EditorPrefs.SetBool(nameof(rawUnityLogs), rawUnityLogs);
        }

        void OnGUI()
        {
            var previousIpAddress = ipAddress;
            var previousPort = port;
            var previousAutoStart = autoStart;
            var previousSendUnityLogs = sendUnityLogs;
            var previousRawUnityLogs = rawUnityLogs;
            GUILayout.Label("Codecast Settings", EditorStyles.boldLabel);
            ipAddress = EditorGUILayout.TextField("IP Address", ipAddress);
            port = EditorGUILayout.IntField("Server Port", port);
            sendUnityLogs = EditorGUILayout.Toggle("Relay Unity Logs", sendUnityLogs);
            rawUnityLogs = EditorGUILayout.Toggle("Raw Unity Logs", rawUnityLogs);
            EditorGUILayout.Separator();
            autoStart = EditorGUILayout.Toggle("Auto Start", autoStart);

            var isDirty = false;
            isDirty |= previousIpAddress != ipAddress;
            isDirty |= previousPort != port;
            isDirty |= previousAutoStart != autoStart;
            isDirty |= previousSendUnityLogs != sendUnityLogs;
            isDirty |= previousRawUnityLogs != rawUnityLogs;
            if (isDirty)
            {
                WritePrefs();
            }

            Codecast.SendUnityLogs = sendUnityLogs;
            Codecast.RawUnityLogs = rawUnityLogs;

            if (!Codecast.IsActive && autoStart)
            {
                Codecast.Start(ipAddress, port);
            }

            if (previousAutoStart != autoStart)
            {
                if (!autoStart)
                {
                    Codecast.Stop();
                }
            }

            if (Codecast.IsActive)
            {
                EditorGUILayout.HelpBox($"Codecast Server running at {ipAddress}:{port}", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("Codecast Server: INACTIVE", MessageType.Warning);
            }
        }
    }
}
