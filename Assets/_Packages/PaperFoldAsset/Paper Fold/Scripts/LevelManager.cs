using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JetSystems;

public class LevelManager : MonoBehaviour
{
    public const int SCALE_EARNNED_COINS_WITH_AD_VALUE = 3;


    public delegate void OnPaperInstantiated(Paper paper);
    public static OnPaperInstantiated onPaperInstantiated;

    [Header(" Settings ")]
    [SerializeField] private int maxCoinsCount = 15;
    [SerializeField] private int minCoinsCount = 5;
    [Min(1)]
    [SerializeField] private int takenCoinsCount = 5;
    [SerializeField] private Paper[] papersPrefabs;


    private int level;
    private Paper currentPaper;
    private int earnedCoins;

    private void Awake()
    {
        PlayerPrefsManager.SaveLevel(0);
        level = PlayerPrefsManager.GetLevel();
        UIManager.onNextLevelButtonPressed += SpawnNextLevel;
        UIManager.onNextLevelButtonPressedWithAd += SpawnNextLevelWithAdditionalCoins;
        UIManager.wrongPaperFolded += DecreaseEarnedCoins;

    }

    private void OnDestroy()
    {
        UIManager.onNextLevelButtonPressed -= SpawnNextLevel;
        UIManager.onNextLevelButtonPressedWithAd -= SpawnNextLevelWithAdditionalCoins;
        UIManager.wrongPaperFolded -= DecreaseEarnedCoins;
    }

    void Start()
    {
        SpawnLevel();

        try
        {
            UIManager.instance.SetMenu();
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    private void SpawnNextLevelWithAdditionalCoins()
    {
        earnedCoins *= SCALE_EARNNED_COINS_WITH_AD_VALUE;
        SpawnNextLevel();
    }

    private void SpawnLevel()
    {
        earnedCoins = maxCoinsCount;
        UIManager.instance.UpdateEarnedCoins(earnedCoins);
        int correctedLevelIndex = level % papersPrefabs.Length;

        transform.Clear();
        currentPaper = Instantiate(papersPrefabs[correctedLevelIndex], transform);

        onPaperInstantiated?.Invoke(currentPaper);
    }

    private void SpawnLevel(int levelIndex)
    {
        transform.Clear();
        
        currentPaper = Instantiate(papersPrefabs[levelIndex], transform);

        onPaperInstantiated?.Invoke(currentPaper);
    }

    private void SpawnNextLevel()
    {
        UIManager.AddCoins(earnedCoins);

        level++;
        PlayerPrefsManager.SaveLevel(level);
        SpawnLevel();

        try
        {
            UIManager.instance.SetGame();
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
        }

        if(AdManager.instance != null)
            AdManager.instance.ShowInterstitialAd();
    }

    public void RetryLevel()
    {
        currentPaper.UnfoldAllFoldings();
    }

    private void DecreaseEarnedCoins()
    {
        if (earnedCoins <= minCoinsCount)
            return;

        earnedCoins -= takenCoinsCount;

        if (earnedCoins < minCoinsCount)
            earnedCoins = minCoinsCount;

        UIManager.instance?.UpdateEarnedCoins(earnedCoins);
    }
}
