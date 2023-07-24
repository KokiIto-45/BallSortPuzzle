#if UNITY_EDITOR
namespace MyApp.MyBSP
{
    using UnityEditor;
    using UnityEngine;

    public class TextWindow : EditorWindow
    {
        public string Title = string.Empty;
        public string[] Descriptions;
        void OnGUI()
        {
            GUILayout.Label(Title, EditorStyles.boldLabel);
            if (Descriptions != null)
            {
                for (int i = 0; i < Descriptions.Length; i++)
                {
                    GUILayout.Label(string.IsNullOrEmpty(Descriptions[i]) ? string.Empty : Descriptions[i]);
                }
            }

            if (GUILayout.Button("Close"))
            {
                this.Close();
            }
        }
    }
}
#endif