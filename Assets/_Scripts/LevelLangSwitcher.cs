using UnityEngine;
using Eiko.YaSDK;
using UnityEngine.UI;
using JetSystems;

public class LevelLangSwitcher : MonoBehaviour
{
    [SerializeField] private string ru;
    [SerializeField] private string en;
    [SerializeField] private Text text;
    [SerializeField] private string ruExpertLevel;
    [SerializeField] private string enExpertLevel;

    private string levelLabel;

    private void Start()
    {
        var level = YanGamesSaveManager.GetLevel() + 1;
        UpdateLevelNumber(level);
    }

    public static bool IsCurrentLevelExpert(int level)
    {
        return (float) level % 10 == 0;
    }

    public void UpdateLevelNumber(int level)
    {
        switch (YandexSDK.instance.Lang)
        {
            case "ru":
                levelLabel = $"{ru}";
                break;

            case "en":
                levelLabel = $"{en}";
                break;
        }
        if (IsCurrentLevelExpert(level))
        {
            switch (YandexSDK.instance.Lang)
            {
                case "ru":
                    levelLabel = $"{ruExpertLevel} " + levelLabel;
                    break;
                case "en":
                    levelLabel = $"{enExpertLevel} " + levelLabel;
                    break;
            }
        }
        text.text = $"{levelLabel} {level}";
    }    
}
