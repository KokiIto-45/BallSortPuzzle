#if UNITY_EDITOR
namespace MyApp.MyBSP.Editors
{
    using UnityEditor;
    using UnityEditor.SceneManagement;

    public class BSPBasketEditorWindow : EditorWindow
    {
        private static BSPGameManager _bSPGameManager;
        private static BSPBasket _bSPBasket;
        private static int _currentIndex;
        private SerializedObject serializedObject;
        private bool removeOverFlowBalls_dirty;
        public static void InitWindow(int currentIndex, ref BSPGameManager bSPGameManager)
        {
            // Get existing open window or if none, make a new one:
            BSPBasketEditorWindow window = (BSPBasketEditorWindow)EditorWindow.GetWindow(typeof(BSPBasketEditorWindow));
            window.Show();
            initData(currentIndex, ref bSPGameManager);
        }
        private static void initData(int currentIndex, ref BSPGameManager bSPGameManager)
        {
            _currentIndex = currentIndex;
            _bSPGameManager = bSPGameManager;
            _bSPBasket = bSPGameManager.getBasket(_currentIndex);
        }
        #region Functions
        public void OnEnable()
        {
        }
        public void OnDisable()
        {
        }
        void OnGUI()
        {
            if (Target == null) { this.Close(); return; }
            serializedObject = EditorTools.CreateSerializedObject(Target);
            serializedObject.Update();
            GetExtraInfo();
            Parameters();
            ShowEvents();
            Balls();
            NextPreviousBTN();
            serializedObject.ApplyModifiedProperties();
            checkForUpdates();
        }
        #endregion
        #region variable
        protected BSPBasket Target { get { return (BSPBasket)_bSPBasket; } }
        #endregion
        #region OnGUI
        private void GetExtraInfo()
        {
            EditorTools.Box_Open();
            EditorTools.Info("Basket Index", (_currentIndex + 1) + " / " + _bSPGameManager.bspBasketsCount());
            EditorTools.Box_Close();
        }
        private void Parameters()
        {
            EditorTools.Box_Open();
            EditorTools.PropertyField(serializedObject, "isEnable", "Enable", "Module is enable / disable.");
            EditorGUI.BeginChangeCheck();
            EditorTools.PropertyField(serializedObject, "Capacity", "Ball Capacity");
            if (EditorGUI.EndChangeCheck()) {
                removeOverFlowBalls_dirty = true;
            }
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
                    "Auto Ball Sorting",
               "Auto BoxCollider Initialize"},
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
                        Target.ApplyBallsClusterIndexChange();
                        Target.AutoBallSorting();
                        break;
                    case 3:
                        Target._bSPBasketUI.InitBoxCollider();
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
                EditorTools.HelpBox("No settings found.", MessageType.Warning);
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
            if (node == null) return;
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
        #region Next Previous btn
        private void NextPreviousBTN()
        {
            EditorTools.Gap();
            EditorTools.Box_Open();
            var boolArray = EditorTools.Buttons(new string[]
            {
                "< Previous","Next >",
                "Close"
            }, 2);
            for (int i = 0; i < boolArray.Length; i++)
            {
                if (!boolArray[i]) continue;
                switch (i)
                {
                    case 0:
                        {
                            int nextIndex = (_currentIndex - 1) % _bSPGameManager.bspBasketsCount();
                            if (nextIndex < 0) nextIndex = _bSPGameManager.bspBasketsCount()-1;
                            //this.Close();
                            //InitWindow(nextIndex, ref _bSPGameManager);
                            initData(nextIndex, ref _bSPGameManager);
                        }
                        break;
                    case 1:
                        {
                            int nextIndex = (_currentIndex + 1) % _bSPGameManager.bspBasketsCount();
                            //this.Close();
                            //InitWindow(nextIndex, ref _bSPGameManager);
                            initData(nextIndex, ref _bSPGameManager);
                        }
                        break;
                    case 2:
                        {
                            this.Close();
                        }
                        break;
                }
            }
            EditorTools.Box_Close();
        }
        #endregion
        private void checkForUpdates()
        {
            if (removeOverFlowBalls_dirty)
            {
                removeOverFlowBalls_dirty = false;
                Target.RemoveOverFlowBalls();
            }
        }
        #endregion
    }
}
#endif