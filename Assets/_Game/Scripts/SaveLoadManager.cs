using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using _Game.Scripts.Level;
using UnityEngine;

namespace _Game.Scripts
{
    public class SaveLoadManager : MonoBehaviour
    {
        public static SaveLoadManager Instance;
        public const string fileName = "/levelsData.dat";

        private void Awake()
        {
            if (Instance != null && Instance != this) 
            { 
                Destroy(this); 
            } 
            else 
            { 
                Instance = this; 
            }
        }

        public void Save(AllLevelsData data, LevelData levelData, int levelNumber)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + fileName);

            if (levelNumber < data.allLevelsData.Count)
            {
                data.allLevelsData[levelNumber] = levelData;
            }
            else
            {
                data.allLevelsData.Add(levelData);
            }
            
            bf.Serialize(file, data);
            file.Close();
        }

        public AllLevelsData Load()
        {
            AllLevelsData allLevelsData = null;
            
            if (File.Exists(Application.persistentDataPath + fileName))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + fileName, FileMode.Open);

                allLevelsData = (AllLevelsData)bf.Deserialize(file);
                file.Close();
            }

            return allLevelsData;
        }
    }
}