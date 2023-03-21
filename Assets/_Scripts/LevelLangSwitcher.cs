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
        switch (YandexSDK.instance.Lang)
        {
            case "ru":
                levelLabel = $"{ru}";
                break;

            case "en":
                levelLabel = $"{en}";
                break;
        }

        var level = YanGamesSaveManager.GetLevel();
        
        if (level % 10 == 0)
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
        
        UpdateLevelNumber(level);
    }

    public void UpdateLevelNumber(int level)
    {
        text.text = $"{levelLabel} {level}";
    }    
}
