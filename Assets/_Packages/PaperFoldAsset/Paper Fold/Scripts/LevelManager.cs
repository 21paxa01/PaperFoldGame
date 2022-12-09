using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JetSystems;

public class LevelManager : MonoBehaviour
{
    public delegate void OnPaperInstantiated(Paper paper);
    public static OnPaperInstantiated onPaperInstantiated;

    [Header(" Settings ")]
    [SerializeField] private Paper[] papersPrefabs;
    int level;
    int lastPaperIndex;
    private Paper currentPaper;

    private void Awake()
    {
        PlayerPrefsManager.SaveLevel(0);
        level = PlayerPrefsManager.GetLevel();
        UIManager.onNextLevelButtonPressed += SpawnNextLevel;
    }

    private void OnDestroy()
    {
        UIManager.onNextLevelButtonPressed -= SpawnNextLevel;
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

    private void SpawnLevel()
    {
        int correctedLevelIndex = level % papersPrefabs.Length;
        lastPaperIndex = correctedLevelIndex;

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

        /*
        if (UIManager.IsGame())
            FindObjectOfType<Paper>().StartUnfolding();
        */
    }
}
