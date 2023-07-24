namespace MyApp.MyBSP
{
    using UnityEngine;
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(BSPBasket))]
    public class BSPBasketUI : MonoBehaviour
    {
        #region variables
        public Transform BottomPoint;
        public Transform TopPoint;
        public Color gizmoBallColor = Color.white;
        public Color gizmoLineColor = Color.red;
        public GameObject GlassGameObject;
        [HideInInspector] public BoxCollider2D _boxCollider;
        [HideInInspector] public BSPBasket _bspBasket;
        #endregion
        #region Functions
        public void OnValidate()
        {
            _boxCollider = GetComponent<BoxCollider2D>();
            _bspBasket = GetComponent<BSPBasket>();
        }
        public void Update()
        {
            if (Input.GetMouseButtonDown(0) && _bspBasket.isEnable && _bspBasket.gameManager != null)
            {
                Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
                if (hit.collider != null && hit.collider == _boxCollider)
                {
                    _bspBasket.gameManager.BasketHit(_bspBasket);
                }
            }
        }
        #region Gizmos
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Color defaultColor = Gizmos.color;
            Gizmos.color = gizmoBallColor;
            if (_bspBasket.gameManager == null || _bspBasket.gameManager.ballSettings == null)
            {
                int d = _bspBasket.Capacity == 0 ? 1 : _bspBasket.Capacity;
                Gizmos.DrawWireSphere(_bspBasket.getEscapePosition(), (TopPosition - BottomPosition).magnitude / d);
            }
            else
            {
                Gizmos.DrawWireSphere(_bspBasket.getEscapePosition(), _bspBasket.gameManager.ballSettings.BallRadiusMargin);
            }
            Gizmos.color = gizmoLineColor;
            Gizmos.DrawLine(BottomPosition, TopPosition);
            Gizmos.color = defaultColor;
        }
#endif
        #endregion
        #endregion
        #region functions
        #region positions
        public Vector3 BasketDirection
        {
            get
            {
                return (TopPosition - BottomPosition).normalized;
            }
        }
        public Vector3 TopPosition
        {
            get
            {
                if (TopPoint == null) return (BottomPosition);
                Vector3 result = TopPoint.position;
                if (_bspBasket.gameManager == null || _bspBasket.gameManager.ballSettings == null)
                {
                    return result;
                }
                float magnitude = _bspBasket.Capacity * _bspBasket.gameManager.ballSettings.BallDiameterMargin;
                Vector3 direction = (result - BottomPosition).normalized;
                return BottomPosition + magnitude * direction;
            }
        }
        public Vector3 BottomPosition
        {
            get
            {
                return BottomPoint == null ? this.transform.position : BottomPoint.position;
            }
        }
        #endregion
        #region collider
        public void InitBoxCollider()
        {
            if (_boxCollider == null && (_boxCollider = GetComponent<BoxCollider2D>()) == null) return;
            _boxCollider.isTrigger = true;
            if (_bspBasket.gameManager == null || _bspBasket.gameManager.ballSettings == null) return;
            float diameter = _bspBasket.gameManager.ballSettings.BallDiameterMargin;
            float length = _bspBasket.Capacity * diameter;
            _boxCollider.size = new Vector2(diameter, length);
            _boxCollider.offset = new Vector2(0f, length / 2);
        }
        #endregion
        #region Glass
        public void setGlassPositionDeltaX(Vector3 deltaX)
        {
            if (GlassGameObject == null) return;
            GlassGameObject.transform.position =
                this.transform.position + deltaX;
        }
        public bool SetGlassScale(Vector3 scale)
        {
            if (GlassGameObject == null) return false;
            GlassGameObject.transform.localScale = scale;
            return true;
        }
        #endregion
        #endregion
    }
}