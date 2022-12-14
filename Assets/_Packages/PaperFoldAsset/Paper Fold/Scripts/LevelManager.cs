using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JetSystems;
using Eiko.YaSDK;

public class LevelManager : MonoBehaviour
{
    public const int SCALE_EARNNED_COINS_WITH_AD_VALUE = 3;


    public delegate void OnPaperInstantiated(Paper paper);
    public static OnPaperInstantiated onPaperInstantiated;

    public delegate void OnThemeUnlocked(ThemeData themeData, int themeUnlockLevelStep = 0);
    public static OnThemeUnlocked themeUnlocked;

    public delegate void ThemeUnlockProgressUpdated(int themeUnlockProgress, int themeUnlcokLevleStep);
    public static ThemeUnlockProgressUpdated themeUnlockProgressUpdated;

    [Header(" Settings ")]
    [SerializeField] private int adCoinsCount = 45;
    [SerializeField] private int maxCoinsCount = 15;
    [SerializeField] private int minCoinsCount = 5;
    [Min(1)]
    [SerializeField] private int takenCoinsCount = 5;
    [Min(1)]
    [SerializeField] private int unlockThemeLevelStep;
    [SerializeField] private Paper[] papersPrefabs;
    [SerializeField] private ThemeData[] unlockableThemes;


    private int level;
    private Paper currentPaper;
    private int earnedCoins;
    private readonly Queue<ThemeData> unlockableThemesQueue = new Queue<ThemeData>();

    private void Awake()
    {
        level = PlayerPrefsManager.GetLevel();
        UIManager.onNextLevelButtonPressed += SpawnNextLevel;
        UIManager.onNextLevelButtonPressedWithAd += SpawnNextLevelWithAdditionalCoins;
        UIManager.wrongPaperFolded += DecreaseEarnedCoins;
        UIManager.onLevelCompleteSet += IncrementThemeUnlockProgress;


        foreach(ThemeData unlockableTheme in unlockableThemes)
        {
            if (PlayerPrefsManager.HasUnlokedTheme(unlockableTheme.Id))
                continue;

            unlockableThemesQueue.Enqueue(unlockableTheme);
        }

    }

    private void OnDestroy()
    {
        UIManager.onNextLevelButtonPressed -= SpawnNextLevel;
        UIManager.onNextLevelButtonPressedWithAd -= SpawnNextLevelWithAdditionalCoins;
        UIManager.wrongPaperFolded -= DecreaseEarnedCoins;
        UIManager.onLevelCompleteSet -= IncrementThemeUnlockProgress;
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

        YandexSDK.instance.ShowInterstitial();
    }

    private void SpawnNextLevelWithAdditionalCoins()
    {
        earnedCoins = adCoinsCount;
        SpawnNextLevel();
    }

    private void IncrementThemeUnlockProgress(int starsCount)
    {
        if (unlockableThemesQueue.Count <= 0)
            return;

        if (PlayerPrefsManager.HasUnlokedTheme(unlockableThemesQueue.Peek().Id))
        {
            ChangeUnlockableTheme();
            IncrementThemeUnlockProgress(starsCount);
            return;
        }
            
        int unlockThemeProgress = PlayerPrefsManager.GetUnlockThemeProgress();
        unlockThemeProgress++;

        if(unlockThemeProgress >= unlockThemeLevelStep)
        {
            themeUnlocked?.Invoke(unlockableThemesQueue.Peek(), unlockThemeLevelStep);
            PlayerPrefsManager.AddUnlockedTheme(unlockableThemesQueue.Peek().Id);
            UIManager.instance?.THEMES.ChangeTheme(unlockableThemesQueue.Peek());
            ChangeUnlockableTheme();
            PlayerPrefsManager.SetUnlockThemeProgress(0);
        }
        else
        {
            themeUnlockProgressUpdated?.Invoke(unlockThemeProgress, unlockThemeLevelStep);
            PlayerPrefsManager.SetUnlockThemeProgress(unlockThemeProgress);
        }
    }

    private void ChangeUnlockableTheme()
    {
        unlockableThemesQueue.Dequeue();
    }

    private void SpawnLevel()
    {
        earnedCoins = maxCoinsCount;
        UIManager.instance.UpdateEarnedCoins(earnedCoins, adCoinsCount);
        int correctedLevelIndex = level;

        if (level > papersPrefabs.Length-1)
            correctedLevelIndex = Random.Range(0, papersPrefabs.Length - 1);

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

        UIManager.instance?.UpdateEarnedCoins(earnedCoins, adCoinsCount);
    }
}
