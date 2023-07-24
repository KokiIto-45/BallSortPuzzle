namespace MyApp.MyBSP
{
    using UnityEngine;
    using System.Collections.Generic;
    [RequireComponent(typeof(BSPBallUI))]
    public class BSPBall : MonoBehaviour
    {
        #region variable
        #region HideInInspector
        //public float Speed;
        public const float TargetDistanceHit = .001f;
        [HideInInspector] public BSPBallState ballState;
        [HideInInspector] public List<Vector3> targets;
        [HideInInspector] public BSPBallUI _bSPBallUI;
        [HideInInspector] public int ClusterIndex = 0;
        [HideInInspector] public BSPGameManager gameManager;
        [HideInInspector] public BSPBallData ballData = new BSPBallData();

#if UNITY_EDITOR
        [HideInInspector] public bool informationEnable;
#endif
        #endregion
        #endregion
        #region Functions
        public void OnValidate()
        {
            _bSPBallUI = GetComponent<BSPBallUI>();
        }
        public void Update()
        {
            if (ballState == BSPBallState.Moving)
                updateMovementPosition();
        }
        #region Gizmos
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            UnityEditor.Handles.Label(this.transform.position, ClusterIndex.ToString());

            //if (settings == null) findSetting();
            if (gameManager != null && gameManager.ballSettings != null)
            {
                //draw Circle
                Color defaultColor = Gizmos.color;
                Gizmos.color = gameManager.ballSettings.BallMarginColor;

                Gizmos.DrawWireSphere(this.transform.position, gameManager.ballSettings.BallRadiusMargin);
                Gizmos.color = gameManager.ballSettings.BallRadiusColor;
                Gizmos.DrawWireSphere(this.transform.position, gameManager.ballSettings.BallRadius);

                Gizmos.color = defaultColor;
            }
        }
#endif
        #endregion
        #endregion
        #region functions
        #region next positions
        public void SetTargetPositions(Vector3 position, bool readfromLowIndexToHigh = true)
        {
            SetTargetPositions(new Vector3[] { position }, readfromLowIndexToHigh);
        }
        public void SetTargetPositions(Vector3[] positions, bool readfromLowIndexToHigh = true)
        {
            if (positions == null || positions.Length < 1) return;
            if (targets == null) targets = new List<Vector3>();
            int index = readfromLowIndexToHigh ? 0 : positions.Length - 1;
            while (true)
            {
                if ((readfromLowIndexToHigh && index == positions.Length)
                    || (!readfromLowIndexToHigh && index == -1)) break;
                targets.Add(positions[index]);
                if (readfromLowIndexToHigh)
                    index++;
                else
                    index--;
            }
            ballState = BSPBallState.Moving;
        }
        #endregion
        #region move
        private void updateMovementPosition()
        {
            if (targets == null || targets.Count < 1)
            {
                ballState = BSPBallState.None;
                return;
            }
            if (Vector3.Distance(targets[0], Position) < TargetDistanceHit)
            {
                Position = targets[0];
                targets.RemoveAt(0);
                if (targets.Count < 1)
                {
                    //var r = this.transform.rotation;
                    //r.x = r.z = 0f;
                    //this.transform.rotation = r;
                    ballState = BSPBallState.None;
                }
                return;
            }
            //Vector3 dir = (targets[0] - Position);
            //if (dir != Vector3.zero) this.transform.rotation = Quaternion.LookRotation(dir);
            Position = Vector3.MoveTowards(Position, targets[0], (Time.deltaTime * Speed));
        }
        #endregion
        public bool IsBusy()
        {
            return ballState != BSPBallState.None;
        }
        public Vector3 Position
        {
            get { return this.transform.position; }
            set { this.transform.position = value; }
        }
        public void ApplyClusterIndex()
        {
            if (!InitBallData()) return;
            _bSPBallUI.SetSprite(ballData.Sprite);
            _bSPBallUI.SetSpriteColor(ballData.SpriteColor);
            _bSPBallUI.SetScaleVector(ballData.ScaleVector);
        }
        public float Speed
        {
            get
            {
                if (ballData == null && !InitBallData())
                {
                    return 1f;
                }
                return ballData.Speed;
            }
        }
        public bool InitBallData()
        {
            return gameManager != null
                && gameManager.ballSettings != null
                && gameManager.ballSettings.GetBallData(ClusterIndex, out ballData);
        }
        #endregion
    }
}