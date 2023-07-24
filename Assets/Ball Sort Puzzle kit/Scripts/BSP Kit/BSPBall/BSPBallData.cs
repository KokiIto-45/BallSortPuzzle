namespace MyApp.MyBSP
{
    using UnityEngine;
    [System.Serializable]
    public class BSPBallData
    {
        #region variable
        public string Name = string.Empty;
        [Min(.1f)]
        public float Speed = 1f;
        public Sprite Sprite;
        public Color SpriteColor = Color.white;
        public Vector3 ScaleVector = Vector3.one;
        public Vector3 SCALE_VECTOR
        {
            get
            {
                return new Vector3(.08f, .08f, 1f);
            }
        }
        #endregion
        #region functions
        public BSPBallData()
        {

        }
        public BSPBallData(string name, Color spriteColor, Vector3 ScaleVector, float speed = 1f, Sprite sprite = null)
        {
            this.Name = name;
            this.ScaleVector = ScaleVector;
            this.Sprite = sprite;
            this.SpriteColor = spriteColor;
            this.Speed = speed;
        }
        public void SetValues(BSPBallData otherNode)
        {
            if (otherNode == null) return;
            SetValues(otherNode.Speed, otherNode.Sprite,otherNode.ScaleVector);
        }
        public void SetValues(float speed, Sprite sprite,Vector3 scaleVector)
        {
            Speed = speed;
            Sprite = sprite;
            ScaleVector = scaleVector;
        }
        #endregion
    }
}