using System;
using Cysharp.Threading.Tasks;
using Eiko.YaSDK;
using JetSystems;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Random = UnityEngine.Random;

public class LevelManager : MonoBehaviour
{
    public const int SCALE_EARNNED_COINS_WITH_AD_VALUE = 3;


    public delegate void OnPaperInstantiated(Paper paper);

    public static OnPaperInstantiated onPaperInstantiated;

    public delegate void OnThemeUnlocked(ThemeData themeData, int themeUnlockLevelStep = 0);

    public static OnThemeUnlocked themeUnlocked;

    public delegate void ThemeUnlockProgressUpdated(int themeUnlockProgress, int themeUnlcokLevleStep);

    public static ThemeUnlockProgressUpdated themeUnlockProgressUpdated;

    [Header(" Settings ")] [SerializeField]
    private int adCoinsCount = 45;

    [SerializeField] private int maxCoinsCount = 15;
    [SerializeField] private int minCoinsCount = 5;
    [Min(1)] [SerializeField] private int takenCoinsCount = 5;
    [Min(1)] [SerializeField] private int unlockThemeLevelStep;
    [SerializeField] private AssetReference[] papers;
    [SerializeField] private ThemeData[] unlockableThemes;


    private Paper currentPaper;
    private int level;
    private int earnedCoins;
    private readonly Queue<ThemeData> unlockableThemesQueue = new Queue<ThemeData>();
    private int currentLoadedLevelIndex = -1;
    private int nextLoadedLevelIndex = -1;

    private void Awake()
    {
        if (YanGamesSaveManager.GetAdOff())
            YandexSDK.instance.AdsOff();

        level = YanGamesSaveManager.GetLevel();

        if (YandexSDK.instance != null)
            YandexSDK.instance.onInterstitialShown += OnOtherAdShown;

        UIManager.onNextLevelButtonPressed += SpawnNextLevel;
        UIManager.onNextLevelButtonPressedWithAd += SpawnNextLevelWithAdditionalCoins;
        UIManager.wrongPaperFolded += DecreaseEarnedCoins;
        UIManager.onLevelCompleteSet += IncrementThemeUnlockProgress;

        foreach (ThemeData unlockableTheme in unlockableThemes)
        {
            if (YanGamesSaveManager.HasUnlokedTheme(unlockableTheme.Id))
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

    private async void Start()
    {
        UpdateEarnedCoins();
        await SpawnLevel();
        StartUp.EndLoad();

        try
        {
            UIManager.instance.SetMenu();
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }

        YandexSDK.instance.ShowInterstitial();
    }
    
#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
            SpawnNextLevel().Forget();
    }
#endif
    
    private async UniTask SpawnNextLevelWithAdditionalCoins()
    {
        earnedCoins = adCoinsCount;
        await SpawnNextLevel();
        AppMetricaWeb.Event("45coinsAd");
    }

    private void IncrementThemeUnlockProgress(int starsCount)
    {
        if (unlockableThemesQueue.Count <= 0)
            return;

        if (YanGamesSaveManager.HasUnlokedTheme(unlockableThemesQueue.Peek().Id))
        {
            ChangeUnlockableTheme();
            IncrementThemeUnlockProgress(starsCount);
            return;
        }

        int unlockThemeProgress = YanGamesSaveManager.GetUnlockThemeProgress();
        unlockThemeProgress++;

        if (unlockThemeProgress >= unlockThemeLevelStep)
        {
            themeUnlocked?.Invoke(unlockableThemesQueue.Peek(), unlockThemeLevelStep);
            YanGamesSaveManager.AddUnlockedTheme(unlockableThemesQueue.Peek().Id);
            UIManager.instance?.THEMES.ChangeTheme(unlockableThemesQueue.Peek());
            ChangeUnlockableTheme();
            YanGamesSaveManager.SetUnlockThemeProgress(0);
        }
        else
        {
            themeUnlockProgressUpdated?.Invoke(unlockThemeProgress, unlockThemeLevelStep);
            YanGamesSaveManager.SetUnlockThemeProgress(unlockThemeProgress);
        }
    }

    private void ChangeUnlockableTheme()
    {
        unlockableThemesQueue.Dequeue();
    }

    public async UniTask SpawnLevel()
    {
        transform.Clear();

        // Просчитали индекс уровня
        int correctedLevelIndex = level;

        if (level > papers.Length - 1)
            correctedLevelIndex = Random.Range(0, papers.Length - 1);
        else
            AppMetricaWeb.Event($"lvl{level + 1}");

        // Выгружаем текущий левел если он есть и не равен 
        if (currentLoadedLevelIndex > -1 && nextLoadedLevelIndex > -1 &&
            currentLoadedLevelIndex != nextLoadedLevelIndex)
        {
            UnloadLevelPaper(currentLoadedLevelIndex);
            currentLoadedLevelIndex = nextLoadedLevelIndex;
        }

        // Загружаем текущий если его нету
        if (currentLoadedLevelIndex <= -1)
        {
            currentLoadedLevelIndex = correctedLevelIndex;
        }

        Paper levelPrefab = await LoadPaperLevel(currentLoadedLevelIndex);
        if (levelPrefab == null)
        {
            throw new InvalidOperationException($"Loading of level with index {currentLoadedLevelIndex}");
        }

        currentPaper = Instantiate(levelPrefab, transform);
        onPaperInstantiated?.Invoke(currentPaper);

        // Загружаем в фоне следующий
        LoadNextLevel(level + 1).Forget();
    }

    public async UniTask LoadNextLevel(int level)
    {
        int correctedLevelIndex = level;
        if (level > papers.Length - 1)
            correctedLevelIndex = Random.Range(0, papers.Length - 1);

        if (correctedLevelIndex == currentLoadedLevelIndex)
        {
            nextLoadedLevelIndex = currentLoadedLevelIndex;
        }
        else
        {
            await LoadPaperLevel(correctedLevelIndex);
            nextLoadedLevelIndex = correctedLevelIndex;
        }
    }

    private async UniTask<Paper> LoadPaperLevel(int level)
    {
        GameObject paperGameObject = null;
        if (papers[level].Asset is GameObject)
            paperGameObject = papers[level].Asset as GameObject;

        if (paperGameObject == null)
        {
            AsyncOperationHandle<GameObject> paperHandler = papers[level].LoadAssetAsync<GameObject>();
            paperGameObject = await paperHandler.Task;
        }

        return paperGameObject.GetComponent<Paper>();
    }

    private void UnloadLevelPaper(int level)
    {
        papers[level].ReleaseAsset();
    }

    private async UniTask SpawnNextLevel()
    {
        UIManager.AddCoins(earnedCoins);
        level++;
        YanGamesSaveManager.SaveLevel(level);

        UpdateEarnedCoins();
        await SpawnLevel();

        try
        {
            UIManager.instance.SetGame();
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    private void UpdateEarnedCoins()
    {
        earnedCoins = maxCoinsCount;
        UIManager.instance.UpdateEarnedCoins(earnedCoins, adCoinsCount);
    }

    public void RetryLevel()
    {
        currentPaper.UnfoldAllFoldings();
    }

    public async void SkipLevel()
    {
        level++;
        YanGamesSaveManager.SaveLevel(level);

        earnedCoins = maxCoinsCount;
        UIManager.instance.UpdateEarnedCoins(earnedCoins, adCoinsCount);
        await SpawnLevel();

        try
        {
            UIManager.instance.SetGame();
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
        }

        YandexSDK.instance.ShowRewarded("SkipLevel");
        AppMetricaWeb.Event("skipAd");
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

    private void OnOtherAdShown()
    {
        AppMetricaWeb.Event("otherAd");
    }
}


public class LoadedLevel
{
    public int LevelIndex { get; private set; }
    public Paper PaperLevel { get; private set; }

    public LoadedLevel(int levelIndex, Paper paperLevel)
    {
        LevelIndex = levelIndex;
        PaperLevel = paperLevel;
    }
}