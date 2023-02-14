using Cysharp.Threading.Tasks;
using Eiko.YaSDK;
using Eiko.YaSDK.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartUp : MonoBehaviour
{
    private static GameObject LoadingScreenInstance;
    [SerializeField] private GameObject LoadScreen;
    private const int gamePlaySceneIndex = 1; 

    private async void Start()
    {
        LoadingScreenInstance = LoadScreen;
        DontDestroyOnLoad(LoadScreen);
        YandexSDK.instance.InitializePurchases();

        await YandexPrefs.Init();
        await SceneManager.LoadSceneAsync(gamePlaySceneIndex);
    }

    /// <summary>
    /// ”ничтожает загрузочный экран
    /// </summary>
    public static void EndLoad()
    {
        if(LoadingScreenInstance)
            Destroy(LoadingScreenInstance);
        else
            Debug.LogWarning("EndLoad should be called once after initial initialization");
    }
}
