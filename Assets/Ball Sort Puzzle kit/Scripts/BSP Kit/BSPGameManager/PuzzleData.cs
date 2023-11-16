namespace MyApp.MyBSP
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]

    public class PuzzleData
    {
        public int boardIndex;
        // リスタートでリセットされる、最後に表示されていた手数
        public int lastStepCount = 0;
        // リスタートでリセットされない、総じて掛かった手数
        public int totalStepCount = 0;
        // リスタートでリセットされる、ボールを動かした個数
        public int lastMoveBallsCount = 0;
        // リスタートでリセットされない、総じてボールを動かした個数
        public int totalMoveBallsCount = 0;
        // リスタートでリセットされる、最後に表示されていた所要時間
        public int lastSeconds = 0;
        // リスタートでリセットされない、総じて掛かった所要時間
        public int totalSeconds = 0;
        // TODO: リスタートした回数を記録して、規定回数を超えたらステージのスキップ機能をアクティブにする
        public int restartCount = 0;
        // ステージをリタイアしてスキップしたかどうか
        public bool isRetired = false;
        public List<string[]> boardData = new List<string[]>();

        public PuzzleData(List<string[]> boardData, int boardIndex)
        {
            this.boardData = boardData;
            this.boardIndex = boardIndex;
        }
    }
}

