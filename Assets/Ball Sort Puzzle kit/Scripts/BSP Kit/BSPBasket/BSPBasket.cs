namespace MyApp.MyBSP
{
    using UnityEngine;
    using UnityEngine.Events;
    using System.Collections.Generic;
    [RequireComponent(typeof(BSPBasketUI))]
    public class BSPBasket : MonoBehaviour
    {
        #region variables
        public bool isEnable = true;
        [Min(1)]
        public int Capacity = 1;
        public float BallEscapePositionDeltaX = .01f;
        public BSPBasketTerminationCondition basketTerminationCondition = BSPBasketTerminationCondition.BasketIsFullAndSameBallsCluster;
        #region Events
        public UnityEvent onFallenBallEvent;
        public UnityEvent onFallenBallEventComplete;
        public UnityEvent onRissenBallEvent;
        public UnityEvent onRissenBallEventComplete;
        #endregion
        [HideInInspector] public BSPBasketUI _bSPBasketUI;
        [HideInInspector] public BSPGameManager gameManager;
        [HideInInspector] public List<BSPBall> balls;
#if UNITY_EDITOR
        [HideInInspector] public bool ballsInformationEnable;
        [HideInInspector] public bool showEventsEnable;
#endif
        #endregion
        #region Functions
        public void OnValidate()
        {
            _bSPBasketUI = GetComponent<BSPBasketUI>();
        }
        public void OnDestroy()
        {
            //while (GetBallsCount() > 0)
            {
                // RemoveBall();
            }
        }
        #endregion
        #region functions
        #region vectors
        #region getEscapePosition
        public Vector3 getEscapePosition()
        {
            return getEscapePosition(BallEscapePositionDeltaX);
        }
        public Vector3 getEscapePosition(float deltaX)
        {
            if (gameManager == null || gameManager.ballSettings == null) return Vector3.zero;
            var settings = gameManager.ballSettings;
            Vector3 dir = _bSPBasketUI.BasketDirection;
            Vector3 v0 = _bSPBasketUI.BottomPosition + dir * settings.BallRadiusMargin;
            return (v0 + Capacity * (dir * (settings.BallDiameterMargin + deltaX)));
        }
        #endregion
        public Vector3[] GetBallsPositions()
        {
            return GetBallsPositions(Capacity);
        }
        public Vector3[] GetBallsPositions(int capacity)
        {
            if (gameManager == null || gameManager.ballSettings == null) return null;
            Vector3[] result = new Vector3[capacity];
            var settings = gameManager.ballSettings;
            Vector3 dir = _bSPBasketUI.BasketDirection;
            Vector3 v0 = _bSPBasketUI.BottomPosition + dir * settings.BallRadiusMargin;
            for (int i = 0; i < capacity; i++)
            {
                result[i] = v0 + i * (dir * settings.BallDiameterMargin);
            }
            return result;
        }
        public Vector3 getTopBallPosition()
        {
            return GetBallPosition(GetBallsCount() - 1);
        }
        public Vector3 GetBallPosition(int ballIndex)
        {
            if (gameManager == null || gameManager.ballSettings == null) return Vector3.zero;
            var settings = gameManager.ballSettings;
            Vector3 dir = _bSPBasketUI.BasketDirection;
            Vector3 v0 = _bSPBasketUI.BottomPosition + dir * settings.BallRadiusMargin;
            return (v0 + ballIndex * (dir * settings.BallDiameterMargin));
        }
        #endregion
        #region gameManager
        public void setGameManager(BSPGameManager node)
        {
            gameManager = node;
        }
        #endregion
        #region balls
        public bool isDefaultCondition()
        {
            switch (basketTerminationCondition)
            {
                case BSPBasketTerminationCondition.BasketIsFullAndSameBallsCluster:
                    return isFullAndSameCluster();
                case BSPBasketTerminationCondition.SameBallsCluster:
                    return isAllBallsHaveSameCluster();
                case BSPBasketTerminationCondition.BasketIsEmpty:
                    return isEmpty();
            }
            return false;
        }
        public bool isEmpty()
        {
            return GetBallsCount() == 0;
        }
        public bool isFullAndSameCluster()
        {
            return GetBallsCount() == Capacity && isAllBallsHaveSameCluster();
        }
        public bool isAllBallsHaveSameCluster()
        {
            if (balls == null || balls.Count == 0) return false;
            int clusterIndex = balls[0].ClusterIndex;
            for (int i = 1; i < balls.Count; i++)
            {
                if (balls[i].ClusterIndex != clusterIndex) return false;
            }
            return true;
        }
        public void ApplyBallsClusterIndexChange()
        {
            if (balls == null) return;
            for (int i = 0; i < balls.Count; i++)
            {
                if (balls[i] == null)
                {
                    balls.RemoveAt(i--);
                    continue;
                }
                balls[i].ApplyClusterIndex();
            }
        }
        public void AutoBallSorting()
        {
            balls_checkForNulls();
            if (GetBallsCount() < 1) return;
            while (GetBallsCount() > Capacity)
            {
                RemoveBall();
            }
            Vector3[] vectors = GetBallsPositions(GetBallsCount());
            if (vectors == null) return;
            for (int i = 0; i < GetBallsCount(); i++)
            {
                balls[i].Position = vectors[i];
            }
        }
        public void RemoveOverFlowBalls()
        {
            balls_checkForNulls();
            if (GetBallsCount() < 1) return;
            while (GetBallsCount() > Capacity)
            {
                RemoveBall();
            }
        }
        #region stack
        public bool PushBall(BSPBall node)
        {
            if (node == null) return false;
            if (balls == null) balls = new List<BSPBall>();
            if (balls.Count == Capacity || balls.IndexOf(node) > -1) return false;
            balls.Add(node);
            return true;
        }
        public bool PushBallAndSetPosition_Immediate(BSPBall node)
        {
            if (!PushBall(node)) return false;
            node.Position = getTopBallPosition();
            return true;
        }
        public bool PushBallAndSetPosition(BSPBall node, Vector3[] mergePositions = null)
        {
            if (!PushBall(node)) return false;
            int size = (mergePositions == null) ? 1 : mergePositions.Length + 1;
            Vector3[] positions = new Vector3[size];
            if (mergePositions != null)
            {
                for (int i = 0; i < mergePositions.Length; i++)
                {
                    positions[i] = mergePositions[i];
                }
            }
            positions[positions.Length - 1] = getTopBallPosition();
            node.SetTargetPositions(positions);
            return true;
        }
        public BSPBall PeekBall()
        {
            return PeekBall(GetBallsCount() - 1);
        }
        public BSPBall PeekBall(int index)
        {
            if (GetBallsCount() < 1 || index < 0 || index >= GetBallsCount()) return null;
            return balls[index];
        }
        #region Pop
        public List<BSPBall> PopSameBalls(int count = 1)
        {
            List<BSPBall> result = new List<BSPBall>();
            if (balls == null || balls.Count == 0 || count < 1) return result;
            int clusterIndex = -1;
            for (int i = 0; i < count; i++)
            {
                BSPBall node = PeekBall();
                if (i == 0)
                {
                    clusterIndex = node.ClusterIndex;
                }
                if (node == null || node.ClusterIndex != clusterIndex) break;
                result.Add(PopBall());
            }
            return result;
        }
        public List<BSPBall> PopBalls(int count = 1)
        {
            List<BSPBall> result = new List<BSPBall>();
            if (balls == null || balls.Count == 0 || count < 1) return result;
            for (int i = 0; i < count; i++)
            {
                BSPBall node = PopBall();
                if (node == null) break;
                result.Add(node);
            }
            return result;
        }
        public BSPBall PopBall()
        {
            if (balls == null || balls.Count == 0) return null;
            BSPBall result = balls[balls.Count - 1];
            balls.RemoveAt(balls.Count - 1);
            return result;
        }
        #endregion
        #region Capacity
        public bool HasCapacity(int count = 1)
        {
            return GetBallsCount() + count <= Capacity;
        }
        public int getFreeCapacitySize()
        {
            return Capacity - GetBallsCount();
        }
        #endregion
        public int GetBallsCount()
        {
            return balls == null ? 0 : balls.Count;
        }
        private void balls_checkForNulls()
        {
            if (balls == null) return;
            for (int i = 0; i < balls.Count; i++)
            {
                if (balls[i] == null)
                {
                    balls.RemoveAt(i--);
                }
            }
        }
        public void balls_Clean()
        {
            if (balls == null) return;
            while (balls.Count > 0)
            {
                RemoveBall();
            }
        }
        #endregion
        #region add
        public bool InstantiateBall()
        {
            if (GetBallsCount() >= Capacity) return false;
            balls_checkForNulls();
            var ballOBJ = Resources.Load(BSPGameManager.BallDIR) as GameObject;
            if (ballOBJ == null) return false;
            var obj = Object.Instantiate(ballOBJ);
            var cls = obj.GetComponent<BSPBall>();
            cls.gameManager = gameManager;
            cls.Position = GetBallPosition(GetBallsCount());
            cls.transform.SetParent(this.transform);
            cls.ApplyClusterIndex();
            //
            return PushBall(cls);
        }
        #endregion
        #region remove
        public void RemoveBall()
        {
            balls_checkForNulls();
            var node = PopBall();
            if (node == null) return;
            Object.DestroyImmediate(node.gameObject, true);
        }
        #endregion
        #endregion
        #region Events
        public void onFallenBallEventInvoke()
        {
            new EventManager().AddEvent(onFallenBallEvent).Invoke();
        }
        public void onFallenBallEventCompleteInvoke()
        {
            new EventManager().AddEvent(onFallenBallEventComplete).Invoke();
        }
        public void onRissenBallEventInvoke()
        {
            new EventManager().AddEvent(onRissenBallEvent).Invoke();
        }
        public void onRissenBallEventCompleteInvoke()
        {
            new EventManager().AddEvent(onRissenBallEventComplete).Invoke();
        }
        #endregion
        public Vector3 Position { get { return transform.position; } set { transform.position = value; } }
        public bool OnRisenBall()
        {
            var node = PeekBall();
            if (node == null) return false;
            node.SetTargetPositions(getEscapePosition());
            return true;
        }
        public bool OnRisenBall_Immediate()
        {
            var node = PeekBall();
            if (node == null) return false;
            node.Position = getEscapePosition();
            return true;
        }
        public bool OnFallenBall()
        {
            var node = PeekBall();
            if (node == null) return false;
            node.SetTargetPositions(GetBallPosition(GetBallsCount() - 1));
            return true;
        }
        public bool OnFallenBall_Immediate()
        {
            var node = PeekBall();
            if (node == null) return false;
            node.Position = GetBallPosition(GetBallsCount() - 1);
            return true;
        }
        public bool isBusy()
        {
            if (balls == null) return false;
            for (int i = balls.Count - 1; i >= 0; i--)
            {
                if (balls[i].IsBusy()) return true;
            }
            return false;
        }
        public void SetBallEscapePositionDeltaX(float dx)
        {
            BallEscapePositionDeltaX = dx;
        }
        #endregion
    }
}