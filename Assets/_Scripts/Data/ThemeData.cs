using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Theme", menuName = "PaperFoldGame/Theme/Create Theme Data")]
public class ThemeData : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private Sprite _sprite;
    [SerializeField] private int _price;
    [SerializeField] private Texture _texture;


    public string Name => _name;
    public Sprite Sprite => _sprite;
    public int Price => _price;
    public Texture Texture => _texture;
}
