using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Game.Scripts.Level
{
    public class LevelUIEntity : MonoBehaviour
    {
        public LevelData levelData;
        public GameObject starsParent;
        public TextMeshProUGUI levelHeaderTMP;

        [Space(10)] [Header("ProgressBar")]
        public GameObject progressBarParent;
        public RectTransform progressBarFillImageTransform;
        public TextMeshProUGUI progressBarInfoTMP;

        [Space(10)] [Header("PlayButton")]
        public Button playButton;
        public TextMeshProUGUI playButtonTMP;
        public Sprite enabledPlayButtonSprite;
        public Sprite disabledPlayButtonSprite;
        
        private int normalizedLevelIndex;

        public void Initialize()
        {
            normalizedLevelIndex = levelData.levelIndex + 1;
            
            levelHeaderTMP.text = "LEVEL-" + normalizedLevelIndex.ToString();
            
            playButton.onClick.AddListener(() =>
            {
                PlayerPrefs.SetInt("ActiveLevelIndex", levelData.levelIndex);
                PlayerPrefs.Save();
                
                SceneManager.LoadScene("GameplayScene");
            });
            
            for (var i = 0; i < levelData.earnedStarsCount; i++)
            {
                starsParent.transform.GetChild(i).GetComponent<Image>().color = Color.white;
            }
            
            playButton.GetComponent<Image>().sprite =
                levelData.isReadyToPlay == true ? enabledPlayButtonSprite : disabledPlayButtonSprite;
            playButtonTMP.text = levelData.isReadyToPlay == true ? "PLAY" : "LOCKED";
            
            if (normalizedLevelIndex % 5 == 0)
            {
                progressBarParent.SetActive(true);
                progressBarInfoTMP.text = levelData.progressBarEarnedStarsCount.ToString() + "/" +
                                          levelData.progressBarNeededStarsCount.ToString();
                
                Vector2 initialSizeDelta = progressBarFillImageTransform.sizeDelta;
                float sizeX = initialSizeDelta.x / levelData.progressBarNeededStarsCount *
                              levelData.progressBarEarnedStarsCount;
                progressBarFillImageTransform.sizeDelta = sizeX * Vector2.right + initialSizeDelta.y * Vector2.up;
            }
            else
            {
                starsParent.SetActive(true);
            }
        }
    }
}
