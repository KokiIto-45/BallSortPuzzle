namespace MyApp.MyBSP
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]

    public class PuzzleData
    {
        readonly int boardIndex; 
        public int stepCount = 0;
        public int seconds = 0;
        public int restartCount = 0;
        public List<string[]> boardData = new List<string[]>();

        public PuzzleData(List<string[]> boardData, int boardIndex)
        {
            this.boardData = boardData;
            this.boardIndex = boardIndex;
        }
    }
}

