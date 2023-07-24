#if UNITY_EDITOR
namespace MyApp.MyBSP.Editors
{
    using UnityEditor;
    using UnityEditor.SceneManagement;

    [CustomEditor(typeof(BSPBasket))]
    public class BSPBasketEditor : Editor
    {
        #region variable
        protected BSPBasket Target { get { return (BSPBasket)target; } }
        #endregion
        #region Inspector
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            Parameters();
            ShowEvents();
            Balls();
            serializedObject.ApplyModifiedProperties();
        }
        private void Parameters()
        {
            EditorTools.Box_Open();
            EditorTools.PropertyField(serializedObject, "isEnable", "Enable", "Module is enable / disable.");
            EditorTools.PropertyField(serializedObject, "Capacity", "Ball Capacity");
            EditorTools.PropertyField(serializedObject, "BallEscapePositionDeltaX", "Ball escape DeltaX", "The position that the ball get out from basket.");
            if (Target.gameManager != null && Target.gameManager.basketTerminationCondition == BSPBasketTerminationCondition.BasketDefaultCondition)
            {
                EditorTools.PropertyField(serializedObject, "basketTerminationCondition", "Termination Condition");
                if (Target.basketTerminationCondition == BSPBasketTerminationCondition.BasketDefaultCondition)
                {
                    Target.basketTerminationCondition = 0;
                }
            }
            EditorTools.Info("Ball(s) count", Target.GetBallsCount().ToString() + " / " + Target.Capacity.ToString());
            EditorTools.Line();
            btnRegion();

            EditorTools.Box_Close();
        }
        private void btnRegion()
        {
            EditorTools.Box_Open();
            bool[] boolArray = EditorTools.Buttons(
                new string[] {
                    "Instantiate ball",
                    "Remove ball" ,
                    "Auto Ball Sorting" },
                2);
            for (int i = 0; i < boolArray.Length; i++)
            {
                if (!boolArray[i]) continue;
                switch (i)
                {
                    case 0:
                        Target.InstantiateBall();
                        break;
                    case 1:
                        Target.RemoveBall();
                        break;
                    case 2:
                        Target.AutoBallSorting();
                        Target.ApplyBallsClusterIndexChange();
                        break;
                }
            }
            EditorTools.Box_Close();
        }
        private void ShowEvents()
        {
            if (!EditorTools.Foldout(ref Target.showEventsEnable, "Events")) return;
            EditorTools.PropertyField(serializedObject, "onFallenBallEvent", "onFallenBall");
        }

        private void Balls()
        {
            if (!EditorTools.Foldout(ref Target.ballsInformationEnable, "Balls")) return;
            EditorTools.Box_Open();
            if (Target.gameManager == null || Target.gameManager.ballSettings == null)
            {
                EditorTools.HelpBox("No setting found.", MessageType.Warning);
            }
            else
            {
                for (int i = Target.GetBallsCount() - 1; i > -1; i--)
                {
                    Ball(i, Target.PeekBall(i));
                }
            }
            EditorTools.Box_Close();
        }
        private void Ball(int index, BSPBall node)
        {
            if(node == null) return;
            EditorTools.Box_Open("Ball[" + index.ToString() + "]");
            int i = node.ClusterIndex;
            node.ClusterIndex = EditorTools.Popup("Ball Type", node.gameManager.ballSettings.Get_BSPballDataTitles(), i);
            if (i != node.ClusterIndex)
            {
                node.ApplyClusterIndex();
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
            EditorTools.Box_Close();
        }
        #endregion
    }
}
#endif