using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using _Game.Scripts.Level;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Game.Scripts
{
    public class SaveLoadManager : MonoBehaviour
    {
        public static SaveLoadManager Instance;
        public const string fileName = "/levelsData.dat";

        private string path;

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

            path = DataPath();
            Debug.Log(path);
        }

        public void Save(AllLevelsData data, LevelData levelData, int levelNumber)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(path);

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
            
            if (File.Exists(path))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(path, FileMode.Open);

                allLevelsData = (AllLevelsData)bf.Deserialize(file);
                file.Close();
            }

            return allLevelsData;
        }
        
        public void SaveInitializedData(AllLevelsData data)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(path);
            
            bf.Serialize(file, data);
            file.Close();
            
            Debug.Log("Initialize Data");
        }

        public void DeleteFile()
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            SceneManager.LoadScene("MenuScene");
        }
        
        private string DataPath()
        {
            if (Directory.Exists(Application.persistentDataPath))
            {
                return Path.Combine(Application.persistentDataPath + fileName);
            }
            
            return Path.Combine(Application.streamingAssetsPath + fileName);
        }
    }
}