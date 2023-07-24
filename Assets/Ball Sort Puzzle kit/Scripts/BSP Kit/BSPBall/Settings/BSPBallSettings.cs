namespace MyApp.MyBSP
{
    using UnityEngine;
    [System.Serializable]
    public class BSPBallSettings
    {
        #region variables
        #region radius
        [Min(0)] public float BallRadius = .1f;
        [Min(0)] public float BallDeltaRadiusMargin = .1f;
        //color
        public Color BallRadiusColor = Color.green;
        public Color BallMarginColor = Color.red;
        #endregion
        public BSPBallData[] ballDatas;
        private int lastBallDatasSize = -1;
        #endregion
        #region Functions
        public BSPBallSettings()
        {
            OnValidate();
        }
        public void OnValidate()
        {
            check_BSPballData_OnValidate();
        }
        #endregion
        #region functions
        #region radius
        public float BallRadiusMargin { get { return BallDeltaRadiusMargin + BallRadius; } }
        public float BallDiameterMargin { get { return BallRadiusMargin * 2; } }
        #endregion
        #region BSPballData
        public BSPBallData GetBallData(int index)
        {
            if (ballDatas == null || index < 0 || index >= ballDatas.Length) return null;
            return ballDatas[index];
        }
        public bool GetBallData(int index, out BSPBallData result)
        {
            if (ballDatas == null || index < 0 || index >= ballDatas.Length)
            {
                result = default;
                return false;
            }
            result = GetBallData(index);
            return true;
        }
        private void check_BSPballData_OnValidate()
        {
            if (ballDatas == null) return;//ballDatas = new BSPBallData[] { new BSPBallData() };
            if (lastBallDatasSize < 0) { lastBallDatasSize = ballDatas.Length; }
            if (lastBallDatasSize == ballDatas.Length) return;
            if (ballDatas.Length == 0)
            {
                ballDatas = new BSPBallData[] { new BSPBallData() };
                ballDatas[0].Name = "Type 0";
                ballDatas[0].ScaleVector = Vector3.one;
                ballDatas[0].Speed = 10f;
                ballDatas[0].SpriteColor = Color.white;
                ballDatas[0].ScaleVector = ballDatas[0].SCALE_VECTOR;
            }
            else if (lastBallDatasSize <= ballDatas.Length)
            {
                for (int i = (lastBallDatasSize < 0 ? 0 : lastBallDatasSize); i < ballDatas.Length; i++)
                {
                    ballDatas[i] = new BSPBallData();
                    ballDatas[i].Name = "Type " + i.ToString();
                    if (i == 0)
                    {
                        ballDatas[i].Speed = 10f;
                        ballDatas[i].SpriteColor = Color.white;
                        ballDatas[i].ScaleVector = ballDatas[0].SCALE_VECTOR;
                    }
                    else
                    {
                        ballDatas[i].SetValues(ballDatas[i - 1]);
                    }
                }
            }
            lastBallDatasSize = ballDatas.Length;
        }
        public string[] Get_BSPballDataTitles()
        {
            if (ballDatas == null || ballDatas.Length < 1) return null;
            string[] result = new string[ballDatas.Length];
            for (int i = 0; i < ballDatas.Length; i++)
            {
                result[i] = ballDatas[i].Name;
            }
            return result;
        }
        #endregion
        #endregion
    }
}