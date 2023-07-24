#if UNITY_EDITOR
namespace MyApp.MyBSP.Editors
{
    using UnityEditor;
    [CustomEditor(typeof(BSPBall))]
    public class BSPBallEditor : Editor
    {
        #region variable
        protected BSPBall Target { get { return (BSPBall)target; } }
        #endregion
        #region Inspector
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            Parameters();
            Info();
            serializedObject.ApplyModifiedProperties();
        }
        private void Parameters()
        {
            EditorTools.Box_Open();
            if (Target.gameManager != null||Target.gameManager.ballSettings!=null)
            {
                Target.ClusterIndex = EditorTools.Popup("Ball Type", Target.gameManager.ballSettings.Get_BSPballDataTitles(), Target.ClusterIndex);
                if (EditorTools.Button("Apply Change"))
                {
                    Target.ApplyClusterIndex();
                }
            }
            EditorTools.Box_Close();
        }
        private void Info()
        {
            if (!EditorTools.Foldout(ref Target.informationEnable, "Information")) return;
            EditorTools.Box_Open();
            EditorTools.Info("Cluster Index", Target.ClusterIndex.ToString());
            EditorTools.Box_Close();
        }
        #endregion
    }
}
#endif