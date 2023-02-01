using UnityEngine;
using Eiko.YaSDK;
using UnityEngine.UI;
using JetSystems;

public class LevelLangSwitcher : MonoBehaviour
{
    [SerializeField] private string ru;
    [SerializeField] private string en;
    [SerializeField] private Text text;

    private string levelLabel;

    private void Start()
    {
        switch (YandexSDK.instance.Lang)
        {
            case "ru":
                levelLabel = ru;
            break;

            case "en":
                levelLabel = en;
                break;
        }

        UpdateLevelNumber(PlayerPrefsManager.GetLevel());
    }

    public void UpdateLevelNumber(int level)
    {
        text.text = $"{levelLabel} {level}";
    }    
}
