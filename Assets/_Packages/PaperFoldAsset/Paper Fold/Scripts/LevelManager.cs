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

    private void Awake()
    {
        level = PlayerPrefs.GetInt("LEVEL");

        UIManager.onNextLevelButtonPressed += SpawnNextLevel;
    }

    private void OnDestroy()
    {
        UIManager.onNextLevelButtonPressed -= SpawnNextLevel;
    }

    // Start is called before the first frame update
    void Start()
    {
        SpawnLevel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SpawnLevel()
    {
        int correctedLevelIndex = level % papersPrefabs.Length;
        lastPaperIndex = correctedLevelIndex;

        transform.Clear();
        Paper paperInstance = Instantiate(papersPrefabs[correctedLevelIndex], transform);

        onPaperInstantiated?.Invoke(paperInstance);
    }

    private void SpawnLevel(int levelIndex)
    {
        transform.Clear();
        Paper paperInstance = Instantiate(papersPrefabs[levelIndex], transform);

        onPaperInstantiated?.Invoke(paperInstance);
    }

    private void SpawnNextLevel()
    {
        level++;
        PlayerPrefs.SetInt("LEVEL", level);

        SpawnLevel();

        AdManager.instance.ShowInterstitialAd();
    }

    public void RetryLevel()
    {
        SpawnLevel(lastPaperIndex);

        /*
        if (UIManager.IsGame())
            FindObjectOfType<Paper>().StartUnfolding();
        */
    }
}
