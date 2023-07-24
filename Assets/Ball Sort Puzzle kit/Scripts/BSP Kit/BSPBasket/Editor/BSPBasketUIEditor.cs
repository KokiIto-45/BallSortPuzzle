#if UNITY_EDITOR
namespace MyApp.MyBSP.Editors
{
    using UnityEditor;
    [CustomEditor(typeof(BSPBasketUI))]
    public class BSPBasketUIEditor : Editor
    {
        #region variable
        protected BSPBasketUI Target { get { return (BSPBasketUI)target; } }
        #endregion
        #region Inspector
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            base.OnInspectorGUI();
            Parameters();
            serializedObject.ApplyModifiedProperties();

        }
        private void Parameters()
        {
            EditorTools.Box_Open();
            bool[] boolArray = EditorTools.Buttons(
                new string[] { "Auto BoxCollider Initialize"},
                2);
            for (int i = 0; i < boolArray.Length; i++)
            {
                if (!boolArray[i]) continue;
                switch (i)
                {
                    case 0:
                        Target.InitBoxCollider();
                        break;
                    case 1:

                        break;
                }
            }
            EditorTools.Box_Close();
        }
        #endregion
    }
}
#endif