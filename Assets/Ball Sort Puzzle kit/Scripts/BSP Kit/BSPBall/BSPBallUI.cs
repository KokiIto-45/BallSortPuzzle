namespace MyApp.MyBSP
{
    using UnityEngine;
    [RequireComponent(typeof(SpriteRenderer))]
    public class BSPBallUI : MonoBehaviour
    {
        #region variable
        #region HideInInspector
        [HideInInspector] public SpriteRenderer _spriteRenderer;
        #endregion
        #endregion
        #region Functions
        public void OnValidate()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
        #endregion
        #region functions
        public void SetSprite(Sprite sprite)
        {
            _spriteRenderer.sprite = sprite;
        }
        public void SetSpriteColor(Color c)
        {
            _spriteRenderer.color = c;
        }
        public void SetSpriteColor(ref BSPBallData data)
        {
            _spriteRenderer.color = data.SpriteColor;
        }
        public void SetScaleVector(Vector3 v)
        {
            transform.localScale = v;
        }
        #endregion
    }
}