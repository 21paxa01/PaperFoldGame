using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] MeshRenderer _meshRenderer;
    
    public void ChangeTheme(ThemeData themeData)
    {
        _meshRenderer.materials[0].mainTexture = themeData.Texture;
    }
}
