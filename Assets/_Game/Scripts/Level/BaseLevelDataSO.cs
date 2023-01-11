using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Scripts.Level
{
    [CreateAssetMenu(fileName = "BaseLevel", menuName = "Level/Create Level Data")]
    public class BaseLevelDataSO : ScriptableObject
    {
        public string levelsPath = "Assets/_Game/Resources/Levels/LS_Case_Level-";
        public int totalLevelCount = 20;
    }

    [Serializable]
    public class AllLevelsData
    {
        public List<LevelData> allLevelsData;
    }
    
    [Serializable]
    public class LevelData
    {
        public int levelIndex;
        public bool isReadyToPlay;
        public int earnedStarsCount;
        
        public int progressBarNeededStarsCount;
        public int progressBarEarnedStarsCount;
    }
}