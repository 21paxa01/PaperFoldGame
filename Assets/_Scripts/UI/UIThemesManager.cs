using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIThemesManager : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private UITheme _uiThemePrefab;
    [SerializeField] private Transform _uiThemesParent;
    [SerializeField] private ThemeData[] _availableThemes;
    [SerializeField] private Board _changableBoard;


    private readonly List<UITheme> _availableThemeUIObjects = new List<UITheme>();

    private void Start()
    {
        if (_availableThemes != null && _availableThemes.Length > 0)
            CreateThemesUI();
    }

    private void CreateThemesUI()
    {
        foreach (ThemeData themeData in _availableThemes)
        {
            UITheme uiTheme = Instantiate(_uiThemePrefab, _uiThemesParent);
            uiTheme.SetData(themeData, _changableBoard);
            _availableThemeUIObjects.Add(uiTheme);
        }
    }
}
