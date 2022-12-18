using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecalObject : MonoBehaviour
{
    [SerializeField] private SpriteRenderer decalRenderer;
    [SerializeField] private Paper _paper;
    [SerializeField] private Texture2D paperTexture;

    private MeshRenderer targetObjectRenderer;
    private Texture2D newTargetObjectTexture;


    private void OnValidate()
    {
        if (_paper == null)
           _paper = GetComponentInChildren<Paper>(true);
    }

    private void Start()
    {
        _paper.GetFrontRenderer().material.mainTexture = paperTexture;
    }


    public IEnumerator Project()
    {
        targetObjectRenderer = _paper.GetFrontRenderer();

        Texture2D decalTexture = decalRenderer.sprite.texture;

        newTargetObjectTexture = new Texture2D(targetObjectRenderer.material.mainTexture.width, targetObjectRenderer.material.mainTexture.height, TextureFormat.RGBA32, false);
        Graphics.CopyTexture(targetObjectRenderer.material.mainTexture, newTargetObjectTexture);
        targetObjectRenderer.material.mainTexture = newTargetObjectTexture;

        int width = decalTexture.width;
        int height = decalTexture.height;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (decalTexture.GetPixel(x, y).a <= 0.1f)
                {
                    continue;
                }
                ProjectPixel(x, y, decalTexture);
            }

            Debug.Log("Pixel printed");
            yield return null;
        }

        newTargetObjectTexture.Apply();
        
        SaveTexture();

        yield return new WaitForSeconds(10f);
        yield return null;
        yield break;
    }

    private void ProjectPixel(int x, int y, Texture2D decalTexture)
    {
        RaycastHit hit;

        Vector3 rayOrigin = GetRayOrigin(x, y, decalTexture);
        Vector3 rayDirection = decalRenderer.transform.forward;
        Ray ray = new Ray(rayOrigin, rayDirection);

        if (!Physics.Raycast(ray, out hit))
            return;

        Renderer rend = hit.transform.GetComponent<Renderer>();
        MeshCollider meshCollider = hit.collider as MeshCollider;

        if (rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null || meshCollider == null)
            return;

        if (rend != targetObjectRenderer)
            return;

        Texture2D tex = rend.material.mainTexture as Texture2D;
        Vector2 pixelUV = hit.textureCoord;
        pixelUV.x *= tex.width;
        pixelUV.y *= tex.height;

        tex.SetPixel((int)pixelUV.x, (int)pixelUV.y, decalTexture.GetPixel(x, y));
        tex.Apply();
    }

    private Vector3 GetRayOrigin(int x, int y, Texture2D decalTexture)
    {
        float rightPercent = (float)x / decalTexture.width;
        float upPercent = (float)y / decalTexture.height;

        Vector3 startPosition = decalRenderer.transform.position;
        startPosition += decalRenderer.transform.right * (rightPercent - 0.5f) * decalRenderer.transform.localScale.x;
        startPosition += decalRenderer.transform.up * (upPercent - 0.5f) * decalRenderer.transform.localScale.y;

        return startPosition;
    }

    private void SaveTexture()
    {
        JetSystems.Utils.SaveTexture(newTargetObjectTexture, "/DecalToTexture/Created Textures/");
    }

}
