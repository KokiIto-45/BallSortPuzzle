namespace MyApp.MyBSP
{
    using MyApp.MyBSP.AI;
    using UnityEngine.Events;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class BSPGameManager : MonoBehaviour
    {
        #region variable
        public const string BallDIR = "Prefabs/BSPballs/BSPball";
        public const string BasketDIR = "Prefabs/BSPbaskets/BSPbasket";
        private bool gameIsOverFlag = false;
        public BSPBallSettings ballSettings = new BSPBallSettings();
        [Min(1)]
        public int defaultBasketCapacity = 4;
        public float defaultBallEscapePositionDeltaX = .01f;
        public Vector3 basketScale = Vector3.one;
        [Min(1)]
        public int basketLineCount = 1;
        [Min(0f)]
        public float basketLeftMargin = 1f;
        [Min(0f)]
        public float basketTopMargin = 1f;
        public Vector3 glassPositionDeltaX = Vector3.zero;
        public BSPBasketTerminationCondition basketTerminationCondition = BSPBasketTerminationCondition.BasketIsFullAndSameBallsCluster;
        #region BSP Insert New Basket
        public LimitedUnlimitedMode bSPInsertNewBasketMode = LimitedUnlimitedMode.Unlimited;
        [Min(0)]
        public int insertNewBasketCount = 1;
        #endregion
        #region UnDo
        public LimitedUnlimitedMode unDoCountMode = LimitedUnlimitedMode.Unlimited;
        [Min(0)]
        public int unDoCount = 1;
        #endregion
        #region game events
        public UnityEvent onGameIsOverEvent;
        #endregion
        #region chaos
        public ChaosData chaosData;
        #endregion
        private bool gameIsLock = false;
        private int _lastBasketIndex = -1;
        private bool checkGameIsOverFlag;
        #region HideInInspector
        #region BSPGameStates 
        [HideInInspector] public List<StepData> steps;
        [HideInInspector] public List<int> targetBasketIndex;
        [HideInInspector] public BSPGameStates _GameState;
        #endregion
#if UNITY_EDITOR
        [HideInInspector] public bool gameEventsEnable;
        [HideInInspector] public bool basketSettingsEnable;
        [HideInInspector] public bool makeChaosEnable;
        [HideInInspector] public bool InformationEnable;
        [HideInInspector] public bool BallsEnable;
#endif
        [HideInInspector] public List<BSPBasket> bSPBaskets;
        [HideInInspector] public List<int> serverBasketsIndex;
        #endregion
        #endregion
        #region Indexer
        public BSPBasket this[int index]
        {
            get
            {
                return getBasket(index);
            }
        }
        #endregion
        #region Functions
        public void OnValidate()
        {
            if (ballSettings == null) ballSettings = new BSPBallSettings();
            ballSettings.OnValidate();
            var anothers = FindObjectsOfType<BSPGameManager>();
            if (anothers != null)
            {
                for (int i = 1; i < anothers.Length; i++)
                {
                    if (Application.isPlaying)
                        Destroy(anothers[i]);
                    else
                        DestroyImmediate(anothers[i], true);
                }
            }
        }
        public void Awake()
        {
            bSPBaskets_checkForNulls();
        }
        public void Update()
        {
            if (gameIsOverFlag) return;
            checkGameState();
            if (gameIsLock) return;
            if (checkGameIsOverFlag)
            {
                checkGameIsOverFlag = false;
                if (gameIsOverFlag = GameIsOver())
                {
                    Debug.Log("Game Is Over");
                    new EventManager().AddEvent(onGameIsOverEvent).Invoke();
                }
            }
        }
        #endregion
        #region functions
        #region serverBaskets
        private void sserverBasketsIndex_add(int index)
        {
            if (serverBasketsIndex == null) serverBasketsIndex = new List<int>();
            serverBasketsIndex.Add(index);
        }
        #endregion
        #region targetBasketIndex
        private void targetBasketIndex_Add(int index)
        {
            if (targetBasketIndex == null) targetBasketIndex = new List<int>();
            targetBasketIndex.Add(index);
        }
        public void targetBasketIndex_Remove(int index)
        {
            if (targetBasketIndex == null) return;
            targetBasketIndex.Remove(index);
        }
        private void targetBasketIndex_Clear()
        {
            if (targetBasketIndex == null) return;
            targetBasketIndex.Clear();
        }
        #endregion
        #region Game State
        public BSPGameStates GameState
        {
            get { return _GameState; }
            private set
            {
                _GameState = value;
                if (_GameState == BSPGameStates.None)
                {
                    checkGameIsOverFlag = true;
                }
            }
        }
        #endregion
        #region Game is over conditions
        public bool GameIsOver()
        {
            if (bSPBaskets == null || bSPBaskets.Count == 0)
            {
                Debug.LogWarning("No baskets found.");
                return true;
            }
            for (int i = 0; i < bSPBaskets.Count; i++)
            {
                var bNode = bSPBaskets[i];
                if (bNode.isEnable
                    && !checkGameIsOver(ref bNode, basketTerminationCondition)) return false;
            }
            return true;
        }
        private bool checkGameIsOver(ref BSPBasket node, BSPBasketTerminationCondition tCondition)
        {
            if (node == null) return false;
            switch (tCondition)
            {
                case BSPBasketTerminationCondition.BasketIsFullAndSameBallsCluster:
                    {
                        return node.isEmpty() || node.isFullAndSameCluster();
                    }
                case BSPBasketTerminationCondition.SameBallsCluster:
                    {
                        return node.isEmpty() || node.isAllBallsHaveSameCluster();
                    }
                case BSPBasketTerminationCondition.BasketIsEmpty:
                    {
                        return node.isEmpty();
                    }
                case BSPBasketTerminationCondition.BasketDefaultCondition:
                    {
                        return node.isDefaultCondition();
                    }
                default:
                    return false;
            }
        }
        #endregion
        #region logics
        private void checkGameState()
        {
            switch (GameState)
            {
                case BSPGameStates.None:
                case BSPGameStates.Null:
                    return;
                case BSPGameStates.OnRisenBall:
                    {
                        if (isTargetBasketIndex_Busy(GameState)) return;
                        GameState = BSPGameStates.RisenBallComplete;
                    }
                    break;
                case BSPGameStates.OnFallenBall:
                    {
                        if (isTargetBasketIndex_Busy(GameState)) return;
                        GameState = BSPGameStates.FallenBallComplete;
                        //GameState = BSPGameStates.None;
                    }
                    break;
                case BSPGameStates.OnBallsRelocation:
                    {
                        if (isTargetBasketIndex_Busy(GameState)) return;
                        if (isServerBasketsIndex_Busy()) return;
                        GameState = BSPGameStates.BallsRelocationComplete;
                        //GameState = BSPGameStates.None;
                    }
                    break;
                case BSPGameStates.FallenBallComplete:
                case BSPGameStates.BallsRelocationComplete:
                    {
                        GameState = BSPGameStates.None;
                    }
                    break;
            }
        }
        private bool isServerBasketsIndex_Busy()
        {
            if (serverBasketsIndex == null) return false;
            for (int i = 0; i < serverBasketsIndex.Count; i++)
            {
                var node = getBasket(serverBasketsIndex[i]);
                if (node == null)
                {
                    serverBasketsIndex.RemoveAt(i--);
                    continue;
                }
                if (node.isBusy())
                {
                    return true;
                }
                serverBasketsIndex.RemoveAt(i--);
            }
            return false;
        }
        private bool isTargetBasketIndex_Busy(BSPGameStates currentState)
        {
            if (targetBasketIndex == null) return false;
            for (int i = 0; i < targetBasketIndex.Count; i++)
            {
                int index = targetBasketIndex[i];
                if (bSPBaskets[index] == null || !bSPBaskets[index].isBusy())
                {
                    switch (currentState)
                    {
                        case BSPGameStates.OnRisenBall:
                            bSPBaskets[index].onRissenBallEventCompleteInvoke();
                            break;
                        case BSPGameStates.OnFallenBall:
                            bSPBaskets[index].onFallenBallEventCompleteInvoke();
                            break;
                        case BSPGameStates.OnBallsRelocation:
                            //TODO: nothing
                            break;
                    }
                    targetBasketIndex.RemoveAt(i--);
                    continue;
                }
                return true;
            }
            return false;
        }
        #endregion
        #region bSPBaskets
        #region Refresh
        public void BasketsScaleRefresh()
        {
            if (bSPBaskets == null) return;
            for (int i = 0; i < bSPBaskets.Count; i++)
            {
                if (bSPBaskets[i] == null)
                {
                    bSPBaskets.RemoveAt(i--);
                    continue;
                }
                bSPBaskets[i]._bSPBasketUI.SetGlassScale(basketScale);
            }
        }
        public void BasketsBallEscapePositionDeltaXRefresh()
        {
            if (bSPBaskets == null) return;
            for (int i = 0; i < bSPBaskets.Count; i++)
            {
                if (bSPBaskets[i] == null)
                {
                    bSPBaskets.RemoveAt(i--);
                    continue;
                }
                bSPBaskets[i].SetBallEscapePositionDeltaX(defaultBallEscapePositionDeltaX);
            }
        }
        public void BallsRefresh()
        {
            if (bSPBaskets == null) return;
            for (int i = 0; i < bSPBaskets.Count; i++)
            {
                if (bSPBaskets[i] == null)
                {
                    bSPBaskets.RemoveAt(i--);
                    continue;
                }
                bSPBaskets[i].ApplyBallsClusterIndexChange();
            }
        }
        public void BasketsGlassPositionDeltaXRefresh()
        {
            if (bSPBaskets == null) return;
            for (int i = 0; i < bSPBaskets.Count; i++)
            {
                if (bSPBaskets[i] == null)
                {
                    bSPBaskets.RemoveAt(i--);
                    continue;
                }
                bSPBaskets[i]._bSPBasketUI.setGlassPositionDeltaX(glassPositionDeltaX);
            }
        }
        #endregion
        public int bspBasketsCount()
        {
            return bSPBaskets == null ? 0 : bSPBaskets.Count;
        }
        #region basket hit
        public void BasketHit(BSPBasket node)
        {
            if (gameIsOverFlag || gameIsLock || node == null) return;
            int nodeIndex = bSPBasketsNodeIndex(node);
            switch (GameState)
            {
                case BSPGameStates.None:
                    {
                        //TODO: Peek the top ball
                        if ((lastBasketIndex = nodeIndex) > -1)
                        {
                            node.OnRisenBall();
                            targetBasketIndex_Add(nodeIndex);
                            GameState = BSPGameStates.OnRisenBall;
                            return;
                        }
                    }
                    break;
                case BSPGameStates.RisenBallComplete:
                    {
                        if (nodeIndex == -1)
                        {

                            break;
                        }
                        //TODO: get the ball back to the basket
                        if (nodeIndex == lastBasketIndex)
                        {
                            getTheBallBackToTheBasket(nodeIndex, ref node);
                            break;
                        }
                        //TODO: Check to move the ball over other basket
                        var lastBasket = getBasket(lastBasketIndex);
                        if (lastBasket == null || !node.HasCapacity())
                        {
                            getTheBallBackToTheBasket(nodeIndex, ref lastBasket);
                            break;
                        }
                        var topBall = lastBasket.PeekBall();
                        //TODO: 
                        if (topBall == null)
                        {
                            break;
                        }
                        var downBall = node.PeekBall();
                        //TODO: move ball
                        if (downBall == null || downBall.ClusterIndex == topBall.ClusterIndex)
                        {
                            int freeCapacity = node.getFreeCapacitySize();
                            if (freeCapacity > 0)
                            {
                                sserverBasketsIndex_add(nodeIndex);
                                moveBallFromTo(lastBasketIndex, nodeIndex, freeCapacity, true);
                            }
                            else
                            {
                                //TODO: No free space. get the ball back to the basket
                                getTheBallBackToTheBasket(nodeIndex, ref lastBasket);
                            }
                            break;
                        }
                        //TODO: get the ball back to the basket
                        getTheBallBackToTheBasket(nodeIndex, ref lastBasket);
                    }
                    break;
            }
        }
        private void getTheBallBackToTheBasket(int nodeIndex, ref BSPBasket node)
        {
            targetBasketIndex_Add(nodeIndex);
            if (!node.OnFallenBall())
            {
                Debug.LogWarning("Not complete: On fallen ball.");
            }
            node.onFallenBallEventInvoke();
            BasketNodeIndex_Reset();
            GameState = BSPGameStates.OnFallenBall;
        }
        #endregion
        private int bSPBasketsNodeIndex(BSPBasket node)
        {
            if (node == null || bSPBaskets == null) return -1;
            return (bSPBaskets.IndexOf(node));
        }
        public BSPBasket getBasket(int index)
        {
            if (bSPBaskets == null || index < 0 || index >= bSPBaskets.Count) return null;
            return bSPBaskets[index];
        }
        public void AddBasket()
        {
            if (bSPBaskets == null) bSPBaskets = new List<BSPBasket>();
            var basketOBJ = Resources.Load(BasketDIR) as GameObject;
            if (basketOBJ == null) return;
            var obj = Object.Instantiate(basketOBJ);
            obj.transform.SetParent(this.transform);
            var cls = obj.GetComponent<BSPBasket>();
            if (cls == null)
            {
                cls = obj.AddComponent<BSPBasket>();
            }
            if (cls != null)
            {
                bSPBaskets_checkForNulls();
                cls.setGameManager(this);
                cls.Capacity = defaultBasketCapacity;
                cls._bSPBasketUI.InitBoxCollider();
                cls._bSPBasketUI.SetGlassScale(basketScale);
                cls.basketTerminationCondition = basketTerminationCondition;
                cls._bSPBasketUI.setGlassPositionDeltaX(glassPositionDeltaX);
                bSPBaskets.Add(cls);
                bSpBasketLineSorting();
            }
        }
        public void RemoveBasket()
        {
            if (bSPBaskets == null || bSPBaskets.Count == 0) return;
            int index = bSPBaskets.Count - 1;
            Object.DestroyImmediate(bSPBaskets[index].gameObject);
            bSPBaskets.RemoveAt(index);
        }
        public void AutoBallSorting()
        {
            if (bSPBaskets == null) return;
            for (int i = 0; i < bSPBaskets.Count; i++)
            {
                if (bSPBaskets[i] == null)
                {
                    bSPBaskets.RemoveAt(i--);
                    continue;
                }
                bSPBaskets[i].AutoBallSorting();
            }
        }
        private void bSPBaskets_checkForNulls()
        {
            if (bSPBaskets == null) return;
            for (int i = 0; i < bSPBaskets.Count; i++)
            {
                if (bSPBaskets[i] == null) { bSPBaskets.RemoveAt(i--); }
            }
        }
        #endregion
        #region bSpBasketLine
        public void bSpBasketLineSorting()
        {
            if (bSPBaskets == null || bSPBaskets.Count < 1) return;
            Vector3 position = bSPBaskets[0].Position;
            for (int i = 1; i < bSPBaskets.Count; i++)
            {
                position += Vector3.right * basketLeftMargin;
                if (i % basketLineCount == 0) { position.x = bSPBaskets[0].Position.x; position += Vector3.down * basketTopMargin; }
                bSPBaskets[i].Position = position;
            }
        }
        #endregion
        #region move balls

        private bool moveBallFromTo(int basket1Index, int basket2Index, int ballsCount, bool pushStep = false)
        {
            return moveBallFromTo(new StepData(basket1Index, basket2Index, ballsCount), pushStep);
        }
        private bool moveBallFromTo(StepData step, bool pushStep = false)
        {
            if (step == null) return false;
            var basket1 = getBasket(step.Basket1Index);
            var basket2 = getBasket(step.Basket2Index);
            if (basket1 == null || basket2 == null) return false;
            var balls1 = basket1.PopSameBalls(step.BallCount);
            if (balls1 == null || balls1.Count == 0)
            {
                //TODO: get the ball back to the basket
                getTheBallBackToTheBasket(step.Basket2Index, ref basket1);
                return false;
            }
            step.BallCount = balls1.Count;
            Vector3[] mergePositions = new Vector3[] { basket1.getEscapePosition(), basket2.getEscapePosition() };

            for (int i = 0; i < balls1.Count; i++)
            {
                basket2.PushBallAndSetPosition(balls1[i], mergePositions);
            }
            if (pushStep)
            {
                steps_Push(step);
            }
            BasketNodeIndex_Reset();
            GameState = BSPGameStates.OnBallsRelocation;
            return true;
        }
        private bool moveBallFromTo_Immediate(StepData step, bool pushStep = false)
        {
            if (step == null) return false;
            var basket1 = getBasket(step.Basket1Index);
            var basket2 = getBasket(step.Basket2Index);
            if (basket1 == null || basket2 == null) return false;
            var balls1 = basket1.PopSameBalls(step.BallCount);
            if (balls1 == null || balls1.Count == 0)
            {
                //TODO: get the ball back to the basket
                //getTheBallBackToTheBasket(step.Basket2Index, ref basket1);
                return false;
            }
            step.BallCount = balls1.Count;
            for (int i = 0; i < balls1.Count; i++)
            {
                basket2.PushBallAndSetPosition_Immediate(balls1[i]);
            }
            if (pushStep)
            {
                steps_Push(step);
            }
            //BasketNodeIndex_Reset();
            return true;
        }
        #endregion
        #region steps
        public int steps_Count()
        {
            return steps == null ? 0 : steps.Count;
        }
        public void steps_Push(int basket1Index, int basket2Index, int ballCount = 1)
        {
            steps_Push(new StepData(basket1Index, basket2Index, ballCount));
        }
        public void steps_Push(StepData step)
        {
            if (steps == null) steps = new List<StepData>();
            steps.Add(step);
        }
        public StepData steps_Pop()
        {
            if (steps == null || steps.Count == 0) return null;
            var result = steps[steps.Count - 1];
            steps.RemoveAt(steps.Count - 1);
            return result;
        }
        public StepData steps_Peek()
        {
            return steps_Peek(steps_Count() - 1);
        }
        public StepData steps_Peek(int index)
        {
            if (steps == null || index < 0 || index >= steps.Count) return null;
            return steps[index];
        }
        public int getSteps()
        {
            return steps == null ? 0 : steps.Count;
        }
        public void Undo()
        {
            UnDoStep();
        }
        public bool UnDoStep()
        {
            if (gameIsOverFlag || GameState != BSPGameStates.None) return false;
            if (unDoCountMode == LimitedUnlimitedMode.Limited)
            {
                if (unDoCount < 1) return false;
                unDoCount--;
            }
            gameIsLock = true;
            var s = steps_Pop();
            if (s != null)
            {
                s.Reverse();
                moveBallFromTo(s);
            }
            gameIsLock = false;
            return true;
        }
        public bool doSteps(ref List<StepData> nodes)
        {
            if (nodes == null) return false;
            for (int i = 0; i < nodes.Count; i++)
            {
                moveBallFromTo(nodes[i], false);
            }
            return true;
        }
        public bool doSteps_Immediate(ref List<StepData> nodes)
        {
            if (nodes == null) return false;
            for (int i = 0; i < nodes.Count; i++)
            {
                moveBallFromTo_Immediate(nodes[i], false);
            }
            return true;
        }
        #endregion
        #region last basket Node Index
        public int lastBasketIndex
        {
            get { return _lastBasketIndex; }
            private set { _lastBasketIndex = value; }
        }
        private void BasketNodeIndex_Reset()
        {
            lastBasketIndex = -1;
        }
        #endregion
        #region chaos
        public bool MakeChaos()
        {
            ChaosMaker cMaker = new ChaosMaker(chaosData);
            cMaker.basketSampling(bSPBaskets);
            var nodes = cMaker.MakeChaos();
            return doSteps_Immediate(ref nodes);
        }
        #endregion
        #region BSP Insert New Basket
        public bool isInsertNewBasket(int count = 1)
        {
            return count > 0
                && (bSPInsertNewBasketMode == LimitedUnlimitedMode.Unlimited || (insertNewBasketCount - count) >= 0);
        }
        public void InsertOneNewBasket()
        {
            InsertNewBasket();
        }
        public bool InsertNewBasket(int count = 1)
        {
            if (count < 1) return false;
            if (bSPInsertNewBasketMode != LimitedUnlimitedMode.Unlimited)
            {
                int c = insertNewBasketCount - count;
                if (c < 0) return false;
                insertNewBasketCount = c;
            }
            AddBasket();
            return true;
        }
        #endregion
        public void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        #endregion
    }
}