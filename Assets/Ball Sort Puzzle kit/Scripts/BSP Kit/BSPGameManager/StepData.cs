namespace MyApp.MyBSP
{
    [System.Serializable]
    public class StepData
    {
        public int Basket1Index;
        public int Basket2Index;
        public int BallCount;
        private bool isReverse = false;
        public StepData() : this(-1, -1, -1) { }
        public StepData(int basket1Index, int basket2Index, int ballCount)
        {
            this.Basket1Index = basket1Index;
            this.Basket2Index = basket2Index;
            this.BallCount = ballCount;
        }
        public void Reverse()
        {
            int temp = Basket1Index;
            Basket1Index = Basket2Index;
            Basket2Index = temp;
            isReverse = !isReverse;
        }
        public void BackToOriginal()
        {
            if (isReverse) Reverse();
        }
        public StepData getReverse()
        {
            return new StepData(this.Basket2Index, this.Basket1Index, this.BallCount);
        }
        public StepData getClone(StepData node)
        {
            if (node == null) return null;
            return new StepData((int)node.Basket1Index, (int)node.Basket2Index, (int)node.BallCount);
        }
        public bool isEqual(StepData other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }
            if (this.GetType() != other.GetType())
            {
                return false;
            }
            return this.Basket1Index.Equals(other.Basket1Index)
                && this.Basket2Index.Equals(other.Basket2Index)
                && this.BallCount.Equals(other.BallCount);
        }
    }
}