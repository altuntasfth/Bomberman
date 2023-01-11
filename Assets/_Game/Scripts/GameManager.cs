using System;
using System.IO;
using _Game.Scripts.Level;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Game.Scripts
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private BaseLevelDataSO baseLevelData;
        [SerializeField] private Transform gridsParent;
        [SerializeField] private GameObject cellPrefab;
        [SerializeField] private GameObject brickPrefab;

        [Header("UI")] [Space(10)] 
        [SerializeField] private TextMeshProUGUI levelTMP;
        [SerializeField] private TextMeshProUGUI bombCountTMP;
        
        private int levelIndex;
        private int normalizedLevelIndex;
        public string levelData;
        private int[,] levelDataMatrix;
        private int rowCount;
        private int columnCount;

        private void Awake()
        {
            levelIndex = PlayerPrefs.GetInt("ActiveLevelIndex", 0);
            normalizedLevelIndex = levelIndex + 1;

            levelTMP.text = "Level-" + normalizedLevelIndex.ToString();
        }
        
        private void Start()
        {
            GetLevelDataFromResources();
            GetLevelDataRowAndColumnCount();
            WriteLevelDataToMatrix();
            GenerateGrids();
        }

        public void OnClickExitButton()
        {
            SceneManager.LoadScene("MenuScene");
        }
        
        private void GetLevelDataFromResources()
        {
            string path = baseLevelData.levelsPath + normalizedLevelIndex.ToString();
            levelData = File.ReadAllText(path);
        }
        
        /*private IEnumerator GetLevelDataFromServer()
        {
            string url = $"https://engineering-case-study.s3.eu-north-1.amazonaws.com/LS_Case_Level-{normalizedLevelIndex}";
            UnityWebRequest www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Show results as text
                Debug.Log(www.downloadHandler.text);

                // Or retrieve results as binary data
                levelData = www.downloadHandler.text;
            }
            
            Debug.Log(true);
        }*/
        
        private void GetLevelDataRowAndColumnCount()
        {
            for (int i = 0; i < levelData.Length; i++)
            {
                char letter = levelData[i];
                if (letter == '\n')
                {
                    if (columnCount == 0)
                    {
                        columnCount = (i + 1) / 2;
                    }

                    rowCount++;
                }
            }

            rowCount++;
        }
        
        private void WriteLevelDataToMatrix()
        {
            levelDataMatrix = new int[rowCount, columnCount];
            int index = 0;
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    levelDataMatrix[i, j] = levelData[index] - '0';
                    index += 2;
                }
            }
        }

        private void GenerateGrids()
        {
            float scaleFactor = brickPrefab.transform.localScale.x;
            Vector3 initialPosition = -(columnCount + scaleFactor / 2f) * Vector3.right +
                                      (rowCount + scaleFactor / 2f) * Vector3.up;
            Vector3 position = initialPosition;
            
            for (int i = 0; i < rowCount; i++)
            {
                position = position.x * Vector3.right +
                           (initialPosition.y - (i * 3)) * Vector3.up;
                
                for (int j = 0; j < columnCount; j++)
                {
                    int gridValue = levelDataMatrix[i, j];
                    position = (initialPosition.x + (j * 3)) * Vector3.right +
                               position.y * Vector3.up;

                    if (gridValue == 0)
                    {
                        CellEntity cell = Instantiate(cellPrefab, position, Quaternion.identity, gridsParent)
                            .GetComponent<CellEntity>();
                    }
                    else
                    {
                        BrickEntity brick = Instantiate(brickPrefab, position, Quaternion.identity, gridsParent)
                            .GetComponent<BrickEntity>();
                    }
                }
            }
        }
    }
}