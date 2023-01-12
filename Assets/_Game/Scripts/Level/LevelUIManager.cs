using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace _Game.Scripts.Level
{
    public class LevelUIManager : MonoBehaviour
    {
        [SerializeField] private BaseLevelDataSO baseLevelData;
        [SerializeField] private GameObject levelUIPrefab;
        [SerializeField] private Transform levelUIContentParent;
        [SerializeField] private Button deleteFileButton;

        private int normalizedLevelIndex;
        private int[,] levelDataMatrix;

        private void Awake()
        {
            InitializeLevelsData();
            CreateLevelUIs();

            deleteFileButton.onClick.AddListener(() =>
            {
                SaveLoadManager.Instance.DeleteFile();
            });
        }

        private void CreateLevelUIs()
        {
            AllLevelsData data = SaveLoadManager.Instance.Load();
            
            for (var i = 0; i < baseLevelData.totalLevelCount; i++)
            {
                LevelData levelData = data.allLevelsData[i];
                
                LevelUIEntity levelUIEntity = Instantiate(levelUIPrefab, levelUIContentParent).GetComponent<LevelUIEntity>();
                levelUIEntity.levelData = levelData;
                
                levelUIEntity.Initialize();
            }
        }

        private void InitializeLevelsData()
        {
            AllLevelsData data = SaveLoadManager.Instance.Load();
            if (data == null)
            {
                data = new AllLevelsData();
                data.allLevelsData = new List<LevelData>();
                
                for (int i = 0; i < baseLevelData.totalLevelCount; i++)
                {
                    LevelData levelData = new LevelData
                    {
                        levelIndex = i,
                        isReadyToPlay = i == 0 ? true : false,
                        earnedStarsCount = 0,
                        progressBarEarnedStarsCount = 0,
                        progressBarNeededStarsCount = (i + 1) % 5 == 0 ? i * 2 : 0
                    };

                    data.allLevelsData.Add(levelData);
                }
                
                SaveLoadManager.Instance.SaveInitializedData(data);
            }
        }
    }
}