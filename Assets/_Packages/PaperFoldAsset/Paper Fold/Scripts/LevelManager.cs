using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JetSystems;

public class LevelManager : MonoBehaviour
{
    public const int SCALE_EARNNED_COINS_WITH_AD_VALUE = 3;


    public delegate void OnPaperInstantiated(Paper paper);
    public static OnPaperInstantiated onPaperInstantiated;

    public delegate void OnThemeUnlocked(ThemeData themeData, int themeUnlockLevelStep);
    public static OnThemeUnlocked themeUnlocked;

    public delegate void ThemeUnlockProgressUpdated(ThemeData themeData, int themeUnlockProgress, int themeUnlcokLevleStep);
    public static ThemeUnlockProgressUpdated themeUnlockProgressUpdated;

    [Header(" Settings ")]
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
        PlayerPrefsManager.ClearAllData();
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
    }

    private void SpawnNextLevelWithAdditionalCoins()
    {
        earnedCoins *= SCALE_EARNNED_COINS_WITH_AD_VALUE;
        SpawnNextLevel();
    }

    private void IncrementThemeUnlockProgress(int starsCount)
    {
        if (unlockableThemesQueue.Count <= 0)
            return;

        if (PlayerPrefsManager.HasUnlokedTheme(unlockableThemesQueue.Peek().Id))
            ChangeUnlockableTheme();

        int unlockThemeProgress = PlayerPrefsManager.GetUnlockThemeProgress();
        unlockThemeProgress++;

        if(unlockThemeProgress >= unlockThemeLevelStep)
        {
            themeUnlocked?.Invoke(unlockableThemesQueue.Peek(), unlockThemeLevelStep);
            PlayerPrefsManager.AddUnlockedTheme(unlockableThemesQueue.Peek().Id);
            ChangeUnlockableTheme();
        }
        else
        {
            themeUnlockProgressUpdated?.Invoke(unlockableThemesQueue.Peek(), unlockThemeProgress, unlockThemeLevelStep);
            PlayerPrefsManager.SetUnlockThemeProgress(unlockThemeProgress);
        }
    }

    private void ChangeUnlockableTheme()
    {
        unlockableThemesQueue.Dequeue();
        PlayerPrefsManager.SetUnlockThemeProgress(0);
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
