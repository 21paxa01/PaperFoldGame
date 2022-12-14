using System.Collections.Generic;
using UnityEngine;
using JetSystems;
using System.Linq;

[RequireComponent(typeof(CanvasGroup))]
public class UIThemesManager : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private UITheme _uiThemePrefab;
    [SerializeField] private Transform _uiThemesParent;
    [SerializeField] private AvailableTheme[] _availableThemes;
    [SerializeField] private Board _changebleBoard;
    [SerializeField] private AudioSource _clickAudio;


    private readonly List<UITheme> _availableThemeUIObjects = new List<UITheme>();
    private UITheme _uiActiveTheme;


    private CanvasGroup _cachedCanvasGroup;
    public CanvasGroup CachedCanvasGroup
    {
        get
        {
            if (_cachedCanvasGroup == null)
                _cachedCanvasGroup = GetComponent<CanvasGroup>();
            return _cachedCanvasGroup;
        }
    }

    private void Start()
    {
        if (_availableThemes != null && _availableThemes.Length > 0)
            CreateThemesUI();
    }


    private void CreateThemesUI()
    {
        string activeThemeId = PlayerPrefsManager.GetUsedTheme();

        foreach (AvailableTheme themeData in _availableThemes)
        {
            UITheme uiTheme = Instantiate(_uiThemePrefab, _uiThemesParent);
            uiTheme.CachedButton.onClick.AddListener(_clickAudio.Play);
            uiTheme.SetData(themeData.Theme, themeData.IsAvailable);
            uiTheme.ThemeAvailableClicked.AddListener(ChangeTheme);
            _availableThemeUIObjects.Add(uiTheme);

            if (_uiActiveTheme == null && activeThemeId == themeData.Theme.Id)
                ChangeTheme(uiTheme, themeData.Theme);
        }

        if(_uiActiveTheme == null )
        {
            AvailableTheme defaultAvailableTheme = _availableThemes.FirstOrDefault((a) => a.IsAvailable);

            if (defaultAvailableTheme.IsNull())
                throw new System.Exception("Default available theme not found!");

            UITheme defaultAvailableThemeUI = 
                _availableThemeUIObjects.FirstOrDefault((ui) => ui.ThemeData.Id == defaultAvailableTheme.Theme.Id);

            if(defaultAvailableThemeUI == null)
                throw new System.Exception("Default available theme UI not found!");

            ChangeTheme(defaultAvailableThemeUI, defaultAvailableThemeUI.ThemeData);
        }
    }


    public void ChangeTheme(UITheme uiTheme, ThemeData themeData)
    {
        if (_uiActiveTheme != null)
            _uiActiveTheme.CachedButton.interactable = true;

        uiTheme.CachedButton.interactable = false;
        _uiActiveTheme = uiTheme;

        _changebleBoard.ChangeTheme(themeData);
    }

    public void ChangeTheme(ThemeData themeData)
    {
        UITheme uiTheme = 
            _availableThemeUIObjects.FirstOrDefault((tUI) => tUI.ThemeData.Id == themeData.Id);

        ChangeTheme(uiTheme, themeData);
    }

    public void UpdateThemesUI()
    {
        foreach(UITheme uiTheme in _availableThemeUIObjects)
        {
            AvailableTheme themeData = 
                _availableThemes.FirstOrDefault((td) => td.Theme.Id == uiTheme.ThemeData.Id);

            if (themeData.IsNull())
                continue;

            uiTheme.SetData(themeData.Theme, themeData.IsAvailable);
        }
    }
}


[System.Serializable]
public struct AvailableTheme
{
    [SerializeField] private bool _isAvailable;
    [SerializeField] private ThemeData _theme;

    public bool IsAvailable => _isAvailable;
    public ThemeData Theme => _theme;


    public bool IsNull()
    {
        return _theme == null;
    }
}

