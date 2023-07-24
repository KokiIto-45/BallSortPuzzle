#if UNITY_EDITOR
namespace MyApp.MyBSP.Editors
{
    using UnityEditor;
    [CustomEditor(typeof(BSPGameManager))]
    public class BSPGameManagerEditor : Editor
    {
        #region variable
        protected BSPGameManager Target { get { return (BSPGameManager)target; } }
        #endregion
        #region Inspector
        #region Auto Updates
        private bool bSpBasketLineSorting_dirty;
        private bool BasketsScaleRefresh_dirty;
        private bool BallsRefresh_dirty;
        private bool BasketsBallEscapePositionDeltaXRefresh_dirty;
        private bool basketsGlassPositionDeltaX_dirty;
        private void checkForUpdates()
        {
            if (bSpBasketLineSorting_dirty)
            {
                bSpBasketLineSorting_dirty = false;
                Target.bSpBasketLineSorting();
            }
            if (BasketsScaleRefresh_dirty)
            {
                BasketsScaleRefresh_dirty = false;
                Target.BasketsScaleRefresh();
            }
            if (BallsRefresh_dirty)
            {
                BallsRefresh_dirty = false;
                Target.BallsRefresh();
            }
            if (BasketsBallEscapePositionDeltaXRefresh_dirty)
            {
                BasketsBallEscapePositionDeltaXRefresh_dirty = false;
                Target.BasketsBallEscapePositionDeltaXRefresh();
            }
            if (basketsGlassPositionDeltaX_dirty)
            {
                basketsGlassPositionDeltaX_dirty = false;
                Target.BasketsGlassPositionDeltaXRefresh();
            }
        }
        #endregion
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            GameEvents();
            BallParameters();
            BasketParameters();
            makeChaos();
            Info();
            serializedObject.ApplyModifiedProperties();
            checkForUpdates();
        }
        private void GameEvents()
        {
            if (!EditorTools.Foldout(ref Target.gameEventsEnable, "Game events")) return;
            EditorTools.Box_Open();
            EditorTools.PropertyField(serializedObject, "onGameIsOverEvent", "onGameIsOver");
            EditorTools.Box_Close();
        }
        private void BallParameters()
        {
            if (!EditorTools.Foldout(ref Target.BallsEnable, "Ball(s)")) return;
            EditorTools.Box_Open();
            EditorTools.PropertyField(serializedObject, "ballSettings", "Ball Settings");
            if (EditorTools.Box_Close())
            {
                BallsRefresh_dirty = true;
            }
        }
        private void BasketParameters()
        {
            if (!EditorTools.Foldout(ref Target.basketSettingsEnable, "Basket(s)")) return;
            EditorTools.Box_Open();
            {
                //Insert new basket options
                EditorTools.Box_Open("Insert new basket options");
                EditorTools.PropertyField(serializedObject, "bSPInsertNewBasketMode", "Mode");
                switch (Target.bSPInsertNewBasketMode)
                {
                    case LimitedUnlimitedMode.Limited:
                        EditorTools.PropertyField(serializedObject, "insertNewBasketCount", "Count");
                        break;
                    default:
                        break;
                }
                EditorTools.Box_Close();
            }
            {
                //UnDo options
                EditorTools.Box_Open("UnDo options");
                EditorTools.PropertyField(serializedObject, "unDoCountMode", "Mode");
                switch (Target.unDoCountMode)
                {
                    case LimitedUnlimitedMode.Limited:
                        EditorTools.PropertyField(serializedObject, "unDoCount", "Count");
                        break;
                    default:
                        break;
                }
                EditorTools.Box_Close();
            }
            EditorTools.PropertyField(serializedObject, "defaultBasketCapacity", "Default basket capacity");
            {
                EditorGUI.BeginChangeCheck();
                EditorTools.PropertyField(serializedObject, "basketLineCount", "Basket line count", "The count of basket(s) in each line.");
                EditorTools.PropertyField(serializedObject, "basketLeftMargin", "Basket left margin");
                EditorTools.PropertyField(serializedObject, "basketTopMargin", "Basket top margin");
                if (EditorGUI.EndChangeCheck())
                {
                    bSpBasketLineSorting_dirty = true;
                }
            }
            {
                EditorGUI.BeginChangeCheck();
                EditorTools.PropertyField(serializedObject, "defaultBallEscapePositionDeltaX", "Basket default ball escape position");
                if (EditorGUI.EndChangeCheck())
                {
                    BasketsBallEscapePositionDeltaXRefresh_dirty = true;
                }
            }
            {
                EditorGUI.BeginChangeCheck();
                EditorTools.PropertyField(serializedObject, "basketScale", "Basket Scale");
                if (EditorGUI.EndChangeCheck())
                {
                    BasketsScaleRefresh_dirty = true;
                }
            }
            {
                EditorGUI.BeginChangeCheck();
                EditorTools.PropertyField(serializedObject, "glassPositionDeltaX", "Basket glass position deltaX");
                if (EditorGUI.EndChangeCheck())
                {
                    basketsGlassPositionDeltaX_dirty = true;
                }
            }
            EditorTools.PropertyField(serializedObject, "basketTerminationCondition", "Baskets termination condition");
            EditorTools.Line();
            btnRegion();
            EditorTools.Box_Close();
        }
        private void btnRegion()
        {
            EditorTools.Box_Open();
            var boolArray = EditorTools.Buttons(new string[] { "Add basket", 
                "Remove Basket", 
                "Auto balls sorting", 
                "Auto baskets sorting" }, 2);
            for (int i = 0; i < boolArray.Length; i++)
            {
                if (!boolArray[i]) continue;
                switch (i)
                {
                    case 0:
                        Target.AddBasket();
                        break;
                    case 1:
                        Target.RemoveBasket();
                        break;
                    case 2:
                        Target.AutoBallSorting();
                        break;
                    case 3:
                        Target.bSpBasketLineSorting();
                        Target.BasketsScaleRefresh();
                        break;
                }
            }
            if (Target.bspBasketsCount() > 0)
            {
                string name = "Basket" + (Target.bspBasketsCount() > 1 ? "s" : "") + " settings";
                if (EditorTools.Button(name))
                {
                    var t = (BSPGameManager)target;
                    BSPBasketEditorWindow.InitWindow(0, ref t);
                }
            }
            EditorTools.Box_Close();
        }
        private void makeChaos()
        {
            if (!EditorTools.Foldout(ref Target.makeChaosEnable, "Chaos")) return;
            EditorTools.Box_Open();
            EditorTools.PropertyField(serializedObject, "chaosData", "Chaos data");
            if (EditorTools.Button("Make chaos"))
            {
                Target.MakeChaos();
            }
            EditorTools.Box_Close();

        }
        private void Info()
        {
            if (!EditorTools.Foldout(ref Target.InformationEnable, "Information")) return;
            EditorTools.Box_Open();
            EditorTools.Info("Total steps", (Target.steps == null ? 0 : Target.steps.Count).ToString());
            EditorTools.Info("Game State", Target._GameState.ToString());
            EditorTools.Box_Close();
        }
        #endregion
    }
}
#endif