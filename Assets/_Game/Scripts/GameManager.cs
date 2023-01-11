using System;
using System.Collections.Generic;
using System.IO;
using _Game.Scripts.Level;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Game.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public Camera mainCamera;
        public PlayerMechanic player;
        public bool isGameStarted;
        public bool isGameOver;
        public bool showOptimumBombs;
        
        [SerializeField] private BaseLevelDataSO baseLevelData;
        [SerializeField] private Transform gridsParent;
        [SerializeField] private GameObject cellPrefab;
        [SerializeField] private GameObject brickPrefab;
        [SerializeField] private GameObject bombPrefab;
        [SerializeField] private int minimumBombCount;
        [SerializeField] private float gridScaleAnimationDuration = 1f;

        [Header("UI")] [Space(10)] 
        [SerializeField] private TextMeshProUGUI levelTMP;
        [SerializeField] private TextMeshProUGUI bombCountTMP;
        [SerializeField] private GameObject levelCompleteScreen;
        [SerializeField] private TextMeshProUGUI levelCompleteHeaderTMP;
        [SerializeField] private GameObject starsParent;

        private int droppedBombCount;
        private int levelIndex;
        private int normalizedLevelIndex;
        public string levelData;
        private int[,] levelDataMatrix;
        private Grid[,] gridsDataMatrix;
        private List<CellEntity> cellsList;
        private List<BombEntity> optimumBombsList;
        private List<BombEntity> droppedBombsList;
        private int rowCount;
        private int columnCount;

        private void OnEnable()
        {
            player.DropBomb += OnDropBomb;
        }

        private void OnDisable()
        {
            player.DropBomb -= OnDropBomb;
        }

        private void Awake()
        {
            Application.targetFrameRate = 60;
            levelIndex = PlayerPrefs.GetInt("ActiveLevelIndex", 0);
            normalizedLevelIndex = levelIndex + 1;

            optimumBombsList = new List<BombEntity>();
            droppedBombsList = new List<BombEntity>();

            levelTMP.text = "Level-" + normalizedLevelIndex.ToString();
        }
        
        private void Start()
        {
            GetLevelDataFromResources();
            GetLevelDataRowAndColumnCount();
            
            mainCamera.orthographicSize = columnCount * 3;
            
            WriteLevelDataToMatrix();
            GenerateGrids();
            GetEachCellsNeighbor();
            CalculateMinimumBombCount();
            
            bombCountTMP.text = minimumBombCount.ToString();

            DOVirtual.DelayedCall(gridScaleAnimationDuration + 0.5f, () =>
            {
                isGameStarted = true;
            });
        }

        private void Update()
        {
            DebugControl();
        }

        private void DebugControl()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                levelIndex = PlayerPrefs.GetInt("ActiveLevelIndex");
                PlayerPrefs.SetInt("ActiveLevelIndex", Mathf.Clamp(levelIndex - 1, 0, baseLevelData.totalLevelCount - 1));
                PlayerPrefs.Save();
                DOTween.KillAll();
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            
            if (Input.GetKeyDown(KeyCode.D))
            {
                levelIndex = PlayerPrefs.GetInt("ActiveLevelIndex");
                PlayerPrefs.SetInt("ActiveLevelIndex", Mathf.Clamp(levelIndex + 1, 0, baseLevelData.totalLevelCount - 1));
                PlayerPrefs.Save();
                DOTween.KillAll();
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                showOptimumBombs = !showOptimumBombs;
                DebugOptimumBomb();
            }
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
            int index = 0;
            gridsDataMatrix = new Grid[rowCount, columnCount];
            cellsList = new List<CellEntity>();
            Vector3 initialPosition = -((columnCount * 3) / 2f - 1.5f) * Vector3.right +
                                      (rowCount + 1.5f) * Vector3.up;
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
                        cell.transform.DOScale(Vector3.one, gridScaleAnimationDuration);
                        cell.name = "Cell_" + i + "," + j;
                        cell.isUsed = false;

                        cellsList.Add(cell);
                        gridsDataMatrix[i, j] = cell;
                    }
                    else
                    {
                        BrickEntity brick = Instantiate(brickPrefab, position, Quaternion.identity, gridsParent)
                            .GetComponent<BrickEntity>();
                        brick.transform.DOScale(Vector3.one, gridScaleAnimationDuration);
                        brick.isUsed = true;
                        brick.name = "Brick_" + i + "," + j;
                        gridsDataMatrix[i, j] = brick;
                    }

                    index++;
                }
            }
        }

        private void GetEachCellsNeighbor()
        {
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    if (gridsDataMatrix[i, j].GetType() == typeof(CellEntity))
                    {
                        CellEntity cell = gridsDataMatrix[i, j].GetComponent<CellEntity>();
                        cell.neighborCellsList = new List<CellEntity>();
                        
                        if (j != 0)
                        {
                            if (gridsDataMatrix[i, j - 1].GetType() == typeof(CellEntity))
                            {
                                cell.neighborCellsList.Add(gridsDataMatrix[i, j - 1].GetComponent<CellEntity>());
                            }
                        }
                        
                        if (j != columnCount - 1)
                        {
                            if (gridsDataMatrix[i, j + 1].GetType() == typeof(CellEntity))
                            {
                                cell.neighborCellsList.Add(gridsDataMatrix[i, j + 1].GetComponent<CellEntity>());
                            }
                        }
                        
                        if (i != 0)
                        {
                            if (gridsDataMatrix[i - 1, j].GetType() == typeof(CellEntity))
                            {
                                cell.neighborCellsList.Add(gridsDataMatrix[i - 1, j].GetComponent<CellEntity>());
                            }
                        }
                        
                        if (i != rowCount - 1)
                        {
                            if (gridsDataMatrix[i + 1, j].GetType() == typeof(CellEntity))
                            {
                                cell.neighborCellsList.Add(gridsDataMatrix[i + 1, j].GetComponent<CellEntity>());
                            }
                        }
                    }
                }
            }
        }

        private void CalculateMinimumBombCount()
        {
            CellEntity cellWithMaximumNeighbor = null;
            for (var i = 0; i < cellsList.Count; i++)
            {
                int maximumNeighborCount = 0;
                
                CellEntity cell = cellsList[i];
                if (cell.isUsedForCalculation == false)
                {
                    maximumNeighborCount = cell.neighborCellsList.Count;
                    cellWithMaximumNeighbor = cell;

                    for (int j = 0; j < cell.neighborCellsList.Count; j++)
                    {
                        CellEntity neighborCell = cell.neighborCellsList[j];

                        if (neighborCell.isUsedForCalculation == false)
                        {
                            if (maximumNeighborCount <= neighborCell.neighborCellsList.Count)
                            {
                                maximumNeighborCount = neighborCell.neighborCellsList.Count;
                                cellWithMaximumNeighbor = neighborCell;
                            }
                        }
                    }

                    minimumBombCount++;
                    cellWithMaximumNeighbor.isUsedForCalculation = true;
                    cellWithMaximumNeighbor.isOptimumPointForBomDrop = true;
                    
                    BombEntity bomb = Instantiate(bombPrefab,
                        cellWithMaximumNeighbor.transform.position - Vector3.forward, Quaternion.identity).GetComponent<BombEntity>();
                    bomb.gameObject.SetActive(false);
                    optimumBombsList.Add(bomb);

                    for (var j = 0; j < cellWithMaximumNeighbor.neighborCellsList.Count; j++)
                    {
                        cellWithMaximumNeighbor.neighborCellsList[j].isUsedForCalculation = true;
                    }
                }
                
            }

            minimumBombCount += 2;
        }

        private void DebugOptimumBomb()
        {
            for (var i = 0; i < optimumBombsList.Count; i++)
            {
                optimumBombsList[i].gameObject.SetActive(showOptimumBombs);
            }
        }

        private void OnDropBomb(CellEntity cell)
        {
            if (!cell.isUsed)
            {
                if (droppedBombCount != minimumBombCount)
                {
                    droppedBombCount++;
                    BombEntity bomb = Instantiate(bombPrefab,
                        cell.transform.position - Vector3.forward, Quaternion.identity).GetComponent<BombEntity>();
                    bomb.currentCell = cell;
                    droppedBombsList.Add(bomb);
                    bombCountTMP.text = (minimumBombCount - droppedBombCount).ToString();

                    if (cell.isOptimumPointForBomDrop)
                    {
                        bomb.isInOptimumCell = true;
                    }
                
                    cell.isUsed = true;
                    Debug.Log("Drop bomb");
                }
            }
        }
    }
}