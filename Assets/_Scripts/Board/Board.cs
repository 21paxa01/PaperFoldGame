using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JetSystems;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    [SerializeField] private Image _boardImage;
    
    public void ChangeTheme(ThemeData themeData)
    {
        string changedThemeId = themeData.Id;

        if (changedThemeId != PlayerPrefsManager.GetUsedTheme())
            PlayerPrefsManager.SetUsedTheme(changedThemeId);

        _boardImage.sprite = themeData.Texture;
    }
}
