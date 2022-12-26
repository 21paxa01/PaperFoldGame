using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TexturesPostprocessor : AssetPostprocessor
{
    private void OnPreprocessTexture()
    {
        TextureImporter importer = (TextureImporter)assetImporter;
        importer.textureCompression = TextureImporterCompression.Compressed;
        importer.maxTextureSize = 1024;
    }
}
