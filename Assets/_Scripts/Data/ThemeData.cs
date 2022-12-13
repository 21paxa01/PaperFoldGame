using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Theme", menuName = "PaperFoldGame/Theme/Create Theme Data")]
public class ThemeData : ScriptableObject
{
    [SerializeField] private string _id;
    [SerializeField] private string _name;
    [SerializeField] private Sprite _sprite;
    [SerializeField] private int _price;
    [SerializeField] private Sprite _boardTheme;

    public string Id => _id;
    public string Name => _name;
    public Sprite Sprite => _sprite;
    public int Price => _price;
    public Sprite Texture => _boardTheme;

    private void OnValidate()
    {
        if (_id == null || _id == string.Empty)
            _id = System.Guid.NewGuid().ToString();
    }
}
