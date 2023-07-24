namespace MyApp.MyBSP.AI
{
    using System.Collections.Generic;
    public class ChaosMaker
    {
        public ChaosData data;
        public List<int>[] baskets;
        public ChaosMaker(ChaosData data)
        {
            this.data = data;
        }
        #region baskets
        public void basketSampling(List<BSPBasket> bSPBaskets)
        {
            baskets = new List<int>[bSPBaskets.Count];
            for (int i = 0; i < bSPBaskets.Count; i++)
            {
                baskets[i] = new List<int>(bSPBaskets[i].Capacity);
                for (int j = 0; j < bSPBaskets[i].GetBallsCount(); j++)
                {
                    var node = bSPBaskets[i].PeekBall(j);
                    if (node == null) continue;
                    baskets[i].Add(node.ClusterIndex);
                }
            }
        }
        private bool basketHasBall(int index)
        {
            return baskets[index].Count > 0;
        }
        private bool basketHasFreeCapasity(int index)
        {
            return getBasketFreeCapacity(index) > 0;
        }
        private int getBasketFreeCapacity(int index)
        {
            return baskets[index].Capacity - baskets[index].Count;
        }
        #region random index
        private bool chooseTwoRandomBaskets(out int index1, out int index2, out int startIndex1, out int finishIndex1)
        {
            index1 = getRandomBasketIndex_basket1();
            index2 = getRandomBasketIndex_basket2();
            startIndex1 = finishIndex1 = default;
            if (index1 == -1 || index2 == -1)
            {
                return false;
            }
            int iteration = -1;
            do
            {
                if (index1 == index2)
                {
                    index2 = getRandomBasketIndex_basket2();
                    if (iteration > data.iteration / 2)
                    {
                        index1 = getRandomBasketIndex_basket1();
                    }
                    continue;
                }
                if (!getBallSampleIndex(index1, out startIndex1, out finishIndex1))
                {
                    index1 = getRandomBasketIndex_basket1();
                    continue;
                }
                return true;
            } while (iteration++ < data.iteration);
            return false;
        }

        private int getRandomBasketIndex_basket1()
        {
            List<int> l = new List<int>();
            for (int i = 0; i < baskets.Length; i++)
            {
                if (!basketHasBall(i)) continue;
                int s, f;
                if (!getBallSampleIndex(i, out s, out f)) continue;
                l.Add(i);
            }
            if (l.Count == 0) return -1;
            int index = new System.Random().Next(l.Count);
            return l[index];
        }
        private int getRandomBasketIndex_basket2()
        {
            List<int> l = new List<int>();
            for (int i = 0; i < baskets.Length; i++)
            {
                if (!basketHasFreeCapasity(i)) continue;
                l.Add(i);
            }
            if (l.Count == 0) return -1;
            int index = new System.Random().Next(l.Count);
            return l[index];
        }
        private bool getBallSampleIndex(int basketIndex, out int startIndex, out int finishIndex)
        {
            var node = baskets[basketIndex];
            startIndex = finishIndex = node.Count - 1;
            if (finishIndex > 0)
            {
                int clusterIndex = node[finishIndex];
                int index = finishIndex;
                while (--index > -1)
                {
                    if (node[index] == clusterIndex)
                    {
                        startIndex = index;
                        continue;
                    }
                    break;
                }
            }
            if (startIndex == finishIndex)
            {
                return node.Count == 1;
            }
            return true;
        }
        #endregion
        #endregion
        public List<StepData> MakeChaos()
        {
            int iteration = 0;
            List<StepData> steps = new List<StepData>();
            while (iteration++ < data.iteration)
            {
                StepData step = makeChaos();
                if (step == null) continue;

                if (steps.Count > 0)
                {
                    if (step.getReverse().isEqual(steps[steps.Count - 1])) continue;
                    if (data.uniqueSteps)
                    {
                        bool allowInsert = true;
                        for (int i = 0; i < steps.Count; i++)
                        {
                            if (steps[i].isEqual(step))
                            {
                                allowInsert = false;
                                break;
                            }
                        }
                        if (!allowInsert) continue;
                    }
                }
                steps.Add(step);
                if (steps.Count >= data.MaxSteps)
                {
                    break;
                }
            }
            return steps;
        }
        private StepData makeChaos()
        {

            //TODO: Choose 2 basket and Make ball sampling
            int basket1Index, basket2Index;
            int sb1Index, fb1Index;
            if (!chooseTwoRandomBaskets(out basket1Index, out basket2Index, out sb1Index, out fb1Index)) return null;
            //TODO: WARNING: in reverse solution we cant pop last ball...
            if (fb1Index > sb1Index)
                sb1Index++;
            //TODO: Put some balls from basket1 to basket2 if possible {basket2 is empty, basket2 on top has same cluster with sampling
            var basket1 = baskets[basket1Index];
            var basket2 = baskets[basket2Index];
            int basket1TopBallClusterIndex = basket1[fb1Index];
            if (basket2.Count > 0 && basket2[basket2.Count - 1] != basket1TopBallClusterIndex)
            {
                return null;
            }
            int randomStartIndex = new System.Random().Next(sb1Index, fb1Index);
            int basket1SampleSize = (fb1Index - randomStartIndex) + 1;
            int basket2FreeSpace = getBasketFreeCapacity(basket2Index);
            int totalRelocationBallCount = basket1SampleSize < basket2FreeSpace ? basket1SampleSize : basket2FreeSpace;
            for (int i = 0; i < totalRelocationBallCount; i++)
            {
                basket1.RemoveAt(basket1.Count - 1);
                basket2.Add(basket1TopBallClusterIndex);
            }
            return new StepData(basket1Index, basket2Index, totalRelocationBallCount);
        }
    }
}