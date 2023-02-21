using Cysharp.Threading.Tasks;
using Eiko.YaSDK;
using Eiko.YaSDK.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartUp : MonoBehaviour
{
    private static GameObject LoadingScreenInstance;

    [SerializeField] private GameObject LoadScreen;
    [SerializeField] private Image loadingImage;

    private const int gamePlaySceneIndex = 1;

    private async void Start()
    {
        LeanTween.rotateAroundLocal(loadingImage.gameObject, Vector3.forward, 360f, 2f).setLoopClamp();
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
