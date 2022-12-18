using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
public class DecalMaster : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private DecalObject[] _decalObjects;

    [Header("Single Paper Mode Settings")]
    [SerializeField] private SpriteRenderer decalRenderer;
    [SerializeField] private MeshRenderer targetObjectRenderer;
    [SerializeField] private Transform papersParent;
    [SerializeField] private Texture2D[] coloredPapers;
    
    
    private Texture2D newTargetObjectTexture;
    private bool projectingActive;
    private Coroutine currentProjectingCoroutine = null;


    public bool ProjectingActive => projectingActive;


    private IEnumerator ProjectPapersCoroutine()
    {
        if (_decalObjects == null || _decalObjects.Length < 0)
        {
            Debug.LogWarning("Not found decal objects!");
            yield break;
        }

        for(int i = 0; i < _decalObjects.Length; i++)
        {
            if (_decalObjects[i].gameObject.activeInHierarchy)
                _decalObjects[i].gameObject.SetActive(false);
        }


        projectingActive = true;

        for (int i = 0; i < _decalObjects.Length; i++)
        {
            _decalObjects[i].gameObject.SetActive(true);
            yield return currentProjectingCoroutine = StartCoroutine(_decalObjects[i].Project());
            _decalObjects[i].gameObject.SetActive(false);
            Debug.LogWarning($"Finished Printing");
            yield return null;
        }

        projectingActive = false;

        yield break;
    }

    private IEnumerator ProjectSinglePaperCoroutine()
    {
        projectingActive = true;
        yield return currentProjectingCoroutine = StartCoroutine(Project());
        projectingActive = false;
    }

    private IEnumerator Project()
    {
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

    private void ConfigureChild(int paperIndex)
    {
        Paper currentPaper = papersParent.GetChild(paperIndex).GetComponent<Paper>();
        targetObjectRenderer = currentPaper.GetFrontRenderer();

        currentPaper.GetFrontRenderer().sharedMaterial.mainTexture = GetRandomColoredPaper();
    }

    private Texture2D GetRandomColoredPaper()
    {
        return coloredPapers[Random.Range(0, coloredPapers.Length)];
    }


    public void EnableNextPaper()
    {
        int currentActivePaperIndex = 0;

        for (int i = 0; i < papersParent.childCount; i++)
        {
            if (papersParent.GetChild(i).gameObject.activeSelf)
            {
                currentActivePaperIndex = i;
                break;
            }
        }

        currentActivePaperIndex++;
        if (currentActivePaperIndex >= papersParent.childCount)
            currentActivePaperIndex = 0;

        for (int i = 0; i < papersParent.childCount; i++)
        {
            if (i == currentActivePaperIndex)
                papersParent.GetChild(i).gameObject.SetActive(true);
            else
                papersParent.GetChild(i).gameObject.SetActive(false);
        }

        ConfigureChild(currentActivePaperIndex);
    }

    public void ProjectSinglePaper()
    {
        currentProjectingCoroutine = StartCoroutine(ProjectSinglePaperCoroutine());
    }

    public void ProjectAllPapers()
    {
        currentProjectingCoroutine = StartCoroutine(ProjectPapersCoroutine());
    }

    public void StopCurrentProjecting()
    {
        StopAllCoroutines();
        currentProjectingCoroutine = null;
        projectingActive = false;
    }
}
#endif