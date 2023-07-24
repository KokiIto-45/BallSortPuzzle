#if UNITY_EDITOR
namespace MyApp.MyBSP
{
    using UnityEditor;
    using UnityEngine;

    public static class EditorTools
    {
        public static void Gap()
        {
            EditorGUILayout.LabelField("");
        }
        public static void Line()
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }
        #region Box
        public static void Box_Open(string name = null)
        {
            EditorGUI.BeginChangeCheck();
            GUILayout.BeginVertical("box");
            if (!string.IsNullOrEmpty(name))
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(name, EditorStyles.boldLabel);
                GUILayout.EndHorizontal();
            }
        }
        public static bool Box_Close()
        {
            GUILayout.EndVertical();
            return EditorGUI.EndChangeCheck();
        }
        #endregion
        #region info
        public static void Info(string caption, string value)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(caption + ": " + value);
            GUILayout.EndHorizontal();
        }
        #endregion
        #region Objects
        #region Toogle
        public static bool Toggle(ref bool value, string title = null)
        {
            string t = string.IsNullOrEmpty(title) ? "" : title;
            EditorGUI.BeginChangeCheck();
            GUILayout.BeginHorizontal();
            value = GUILayout.Toggle(value, t);
            GUILayout.EndHorizontal();

            return EditorGUI.EndChangeCheck();
        }
        #endregion
        #region get value
        public static bool getIntValue(string name, ref int value)
        {
            if (string.IsNullOrEmpty(name))
            {
                value = default;
                return false;
            }
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label(name);
            value = EditorGUILayout.IntField(value);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            return true;
        }
        #endregion
        #region Help Box
        public static void HelpBox(string msg, MessageType messageType)
        {
            if (string.IsNullOrEmpty(msg)) return;
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox(msg, messageType);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }
        #endregion
        #region Button
        public static bool Button(string title)
        {
            var result = Buttons(new string[] { title });
            if (result == null || result.Length < 1) return false;
            return result[0];
        }
        public static bool[] Buttons(string[] titles, int rowCount = 1)
        {
            if (titles == null || titles.Length < 1) return null;
            bool[] result = new bool[titles.Length];
            if (rowCount < 1) rowCount = 1;
            GUILayout.BeginVertical();
            int c = 0;
            for (int i = 0; i < titles.Length; i++)
            {
                string t = string.IsNullOrEmpty(titles[i]) ? "" : titles[i];
                if (c == 0)
                {
                    GUILayout.BeginHorizontal();
                }
                result[i] = GUILayout.Button(t);
                if (++c == rowCount || i == titles.Length - 1)
                {
                    GUILayout.EndHorizontal();
                    c = 0;
                }
            }
            GUILayout.EndVertical();
            return result;
        }
        #endregion
        #region Popup
        public static bool Popup(ref int index, string[] options, string title = null)
        {
            string t = string.IsNullOrEmpty(title) ? "" : title;

            EditorGUI.BeginChangeCheck();
            GUILayout.BeginHorizontal();
            GUILayout.Label(t);
            index = EditorGUILayout.Popup(index, options);
            GUILayout.EndHorizontal();

            return EditorGUI.EndChangeCheck();
        }
        public static int Popup<T>(string title, int selectedIndex)
        {
            if (selectedIndex < 0) selectedIndex = 0;
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label(string.IsNullOrEmpty(title) ? "" : title);
            int result = EditorGUILayout.Popup(selectedIndex % System.Enum.GetValues(typeof(T)).Length, System.Enum.GetNames(typeof(T)));
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            return result;
        }
        public static int Popup(string title, string[] displayedOptions, int selectedIndex)
        {
            if (displayedOptions == null) return -1;
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label(string.IsNullOrEmpty(title) ? "" : title);
            int result = EditorGUILayout.Popup(selectedIndex < 0 ? 0 : selectedIndex, displayedOptions);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            return result;
        }
        #endregion
        #endregion
        public static bool Foldout(ref bool foldout, string title = null)
        {
            return foldout = EditorGUILayout.Foldout(foldout, title);
        }
        #region SerializedObject
        public static SerializedObject CreateSerializedObject(Object obj)
        {
            return new SerializedObject(obj);
        }
        public static SerializedProperty PropertyField(SerializedObject input, string name, string caption = null, string hint = null)
        {
            if (string.IsNullOrEmpty(name)) return null;

            var result = input.FindProperty(name);

            if (string.IsNullOrEmpty(caption))
            {
                EditorGUILayout.PropertyField(result, true);
            }
            else
            {
                if (string.IsNullOrEmpty(hint))
                    EditorGUILayout.PropertyField(result, new GUIContent(caption), true);
                else EditorGUILayout.PropertyField(result, new GUIContent(caption, hint), true);
            }
            return result;
        }
        #endregion
    }
}
#endif