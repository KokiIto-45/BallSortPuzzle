namespace MyApp.MyBSP.AI
{
    using UnityEngine;
    [System.Serializable]
    public class ChaosData
    {
        [Min(100)]
        public int iteration=1000;
        [Min(1)]
        public int MaxSteps = 10;
        public bool uniqueSteps=true;
    }
}