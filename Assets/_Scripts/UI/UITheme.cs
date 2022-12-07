using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UITheme : MonoBehaviour
{
    [SerializeField] private Image _themeImage;
    [SerializeField] private Text _themeText;

    private ThemeData _themeData;
    private Board _board;


    private Button _cachedButton;
    public Button CachedButton
    {
        get
        {
            if (_cachedButton == null)
                _cachedButton = GetComponent<Button>();
            return _cachedButton;
        }
    }


    public void Awake()
    {
        CachedButton.onClick.AddListener(ChangeTheme);
    }


    public void SetData(ThemeData themeData, Board board)
    {
        _themeData = themeData;
        _themeText.text = themeData.Name;
        _board = board;
    }

    public void ChangeTheme()
    {
        _board.ChangeTheme(_themeData);
    }

}
