using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JetSystems;

public class Board : MonoBehaviour
{
    [SerializeField] MeshRenderer _meshRenderer;
    
    public void ChangeTheme(ThemeData themeData)
    {
        string changedThemeId = themeData.Id;

        if (changedThemeId != PlayerPrefsManager.GetUsedTheme())
            PlayerPrefsManager.SetUsedTheme(changedThemeId);

        _meshRenderer.materials[0].mainTexture = themeData.Texture;
    }
}
