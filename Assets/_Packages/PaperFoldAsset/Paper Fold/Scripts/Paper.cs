using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Packages.PaperFoldAsset.Paper_Fold.Scripts;
using _Packages.PaperFoldAsset.Paper_Fold.Scripts.RegexCombinationBuilder;
using UnityEngine;
using JetSystems;
using Eiko.YaSDK;
using Testing;

public class Paper : MonoBehaviour
{
    [HideInInspector] public PossibleCombinationBuilder Builder;
    [HideInInspector] public RegexCombinationAnalyzer RegexAnalyzer;

    public delegate void OnPaperStateChanged();

    public OnPaperStateChanged onPaperStateChanged;

    public delegate void OnPaperEvolving();

    public OnPaperEvolving onPaperEvolving;

    public delegate void OnPaperShowEffect();

    public OnPaperShowEffect onPaperShowEffect;

    public delegate void PaperStartFold();

    public PaperStartFold paperStartFold;

    public delegate void PaperStartUnfold();

    public PaperStartUnfold paperStartUnfold;

    [Header(" Settings ")] 
    [SerializeField] private bool _test;
    [SerializeField] private bool _decalPaper;
    public Transform foldingsParent;
    private readonly List<Folding> _foldedFolding = new List<Folding>();
    private Folding[] _folding;
    private bool _canFold = true;
    [SerializeField] private Texture2D paperTexture;
    [SerializeField] private PaperEffect effect;

    [Header(" Rendering ")] 
    [SerializeField] private MeshRenderer paperBackRenderer;
    [SerializeField] private MeshRenderer paperFrontRenderer;
    Folding currentFolding;

    [Header(" Mesh Manipulation ")] MeshFilter backFilter;
    private MeshFilter _frontFilter;
    [SerializeField] private float verticesElevationStep;
    [SerializeField] private float foldingDuration;

    [Header(" Solution ")] 
    public PossibleCombination[] possibleCombinations;

    public IEnumerable<Folding> Folding => foldingsParent.Cast<Transform>().Select(x => x.GetComponent<Folding>());

    private void Awake()
    {
        backFilter = paperBackRenderer.GetComponent<MeshFilter>();
        _frontFilter = paperFrontRenderer.GetComponent<MeshFilter>();

        // Анимация появления
        float toPositionZ = transform.position.z;
        Vector3 startPosition = transform.position;
        startPosition.z = 10;
        transform.position = startPosition;

        LoadingState.IsLoading = true;
        transform.LeanMoveZ(toPositionZ, 1f)
                 .setOnComplete(() => LoadingState.IsLoading = false);
    }

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR
        if(!FindObjectOfType<DecalMaster>())
#endif
        paperFrontRenderer.material.mainTexture = paperTexture;

        _folding = GetFoldings();
    }

    public void TryFold(Folding tappedFolding)
    {
        if (!_canFold) return;
        _canFold = false;

        currentFolding = tappedFolding;

        if (!tappedFolding.IsFolded())
        {
            _foldedFolding.Add(currentFolding);
            Fold();
        }
        else
        {
            // It is folded, unfold it
            StartCoroutine(UnfoldingProcedureCoroutine());
        }
    }

    private void Fold()
    {
        // We need to get the vertices that need to be folded first
        // These are the ones inside the folding's tapping zone
        StartCoroutine(FoldCoroutine(true));

        //StartCoroutine("FoldingCoroutine");
        currentFolding.SetFoldedState(true);

        Taptic.Light();
        onPaperEvolving?.Invoke();
    }

    private int[] GetVerticesToMove(MeshFilter filter, Folding folding)
    {
        List<int> verticesInsideTappingZone = new List<int>();

        Vector3[] vertices = filter.mesh.vertices;

        Plane foldingPlane = folding.GetFoldingPlane();

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertexWorldPos = filter.transform.TransformPoint(vertices[i]);
            if (foldingPlane.GetSide(vertexWorldPos))
                verticesInsideTappingZone.Add(i);
        }

        return verticesInsideTappingZone.ToArray();
    }

    private bool IsInsideTappingZone(Vector3 vertexLocalPos, BoxCollider tappingZone)
    {
        Vector3 vertexWorldPos = paperBackRenderer.transform.position + vertexLocalPos;

        return tappingZone.bounds.Contains(vertexWorldPos);
    }

    private Vector3 GetRotatedPoint(Vector3 point, float angle, RotationAxis rotationAxis)
    {
        Vector3 pivot = rotationAxis.position;
        Quaternion q = Quaternion.AngleAxis(angle, rotationAxis.AsVector());
        return q * (point - pivot) + pivot;
    }

    private void Unfold()
    {
        StartCoroutine(FoldCoroutine(false));

        currentFolding.SetFoldedState(false);
        currentFolding.ClearFoldedVertices();
    }

    public void StartUnfolding()
    {
        if (!_canFold) return;
        _canFold = false;

        StartCoroutine(UnfoldingProcedureCoroutine());
    }

    IEnumerator UnfoldingProcedureCoroutine()
    {
        // 1. Check if there is any folding before this one
        for (int i = _foldedFolding.Count - 1; i >= 0; i--)
        {
            Folding folding = _foldedFolding[i];
            if (folding == currentFolding)
            {
                //Unfold();
                Taptic.Light();
                onPaperEvolving?.Invoke();

                yield return StartCoroutine(FoldCoroutine(false));

                currentFolding.SetFoldedState(false);
                currentFolding.ClearFoldedVertices();
                _foldedFolding.Remove(currentFolding);

                break;
            }
            else
            {
                currentFolding = folding;

                Taptic.Light();
                onPaperEvolving?.Invoke();

                yield return StartCoroutine(FoldCoroutine(false));

                currentFolding.SetFoldedState(false);
                currentFolding.ClearFoldedVertices();

                _foldedFolding.Remove(currentFolding);
            }
        }

        _canFold = true;

        onPaperStateChanged?.Invoke();

        yield return null;
    }

    IEnumerator FoldCoroutine(bool folding)
    {
        if (folding)
            paperStartFold?.Invoke();
        else
            paperStartUnfold?.Invoke();


        int[] backVerticesToMove = folding
            ? GetVerticesToMove(backFilter, currentFolding)
            : currentFolding.GetBackFoldedVerticesIndices();
        int[] frontVerticesToMove = folding
            ? GetVerticesToMove(_frontFilter, currentFolding)
            : currentFolding.GetFrontFoldedVerticesIndices();

        if (folding)
        {
            currentFolding.SetBackFoldedVerticesIndices(backVerticesToMove);
            currentFolding.SetFrontFoldedVerticesIndices(frontVerticesToMove);
        }

        Vector3[] initialBackVertices = backFilter.mesh.vertices;
        Vector3[] backVertices = backFilter.mesh.vertices;

        Vector3[] initialFrontVertices = _frontFilter.mesh.vertices;
        Vector3[] frontVertices = _frontFilter.mesh.vertices;

        if (folding)
        {
            for (int i = 0; i < frontVerticesToMove.Length; i++)
                initialFrontVertices[frontVerticesToMove[i]].y -= verticesElevationStep * (1 + _foldedFolding.Count);
        }

        float angleMultiplier = folding ? 1 : -1;
        float targetAngle = currentFolding.GetRotationAngle() * angleMultiplier;
        float timer = 0;

        while (timer <= foldingDuration + Time.deltaTime)
        {
            float angle = Mathf.Clamp01((timer / foldingDuration)) * targetAngle;

            for (int i = 0; i < backVerticesToMove.Length; i++)
                backVertices[backVerticesToMove[i]] =
                    GetRotatedLocalVertex(initialBackVertices[backVerticesToMove[i]], angle);


            for (int i = 0; i < frontVerticesToMove.Length; i++)
                frontVertices[frontVerticesToMove[i]] =
                    GetRotatedLocalVertex(initialFrontVertices[frontVerticesToMove[i]], angle);


            if (!folding)
            {
                for (int i = 0; i < frontVerticesToMove.Length; i++)
                    frontVertices[frontVerticesToMove[i]].y += verticesElevationStep * (1 + _foldedFolding.Count);
            }

            backFilter.mesh.vertices = backVertices;
            _frontFilter.mesh.vertices = frontVertices;

            timer += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < backVerticesToMove.Length; i++)
            backVertices[backVerticesToMove[i]] =
                GetRotatedLocalVertex(initialBackVertices[backVerticesToMove[i]], targetAngle);


        for (int i = 0; i < frontVerticesToMove.Length; i++)
            frontVertices[frontVerticesToMove[i]] =
                GetRotatedLocalVertex(initialFrontVertices[frontVerticesToMove[i]], targetAngle);


        if (!folding)
        {
            for (int i = 0; i < frontVerticesToMove.Length; i++)
                frontVertices[frontVerticesToMove[i]].y += verticesElevationStep * (1 + _foldedFolding.Count);
        }

        backFilter.mesh.vertices = backVertices;
        _frontFilter.mesh.vertices = frontVertices;

        _frontFilter.GetComponent<MeshCollider>().sharedMesh = _frontFilter.mesh;

        if (folding)
            _canFold = true;

        if (AvailableFoldingsEnded() && folding)
        {
            yield return new WaitForSeconds(0.1f);
            CheckForLevelComplete();
        }

        onPaperStateChanged?.Invoke();
    }

    private void CheckForLevelComplete()
    {
        WriteTestingInfo();
        if (Builder != null && Builder.Used && Builder.IsCorrect(_foldedFolding))
        {
            StartCoroutine(SetLevelComplete());
            return;
        }

        if (RegexAnalyzer != null && RegexAnalyzer.IsCorrect(_foldedFolding))
        {
            StartCoroutine(SetLevelComplete());
            return;
        }

        for (int i = 0; i < possibleCombinations.Length; i++)
        {
            if (MatchPossibleCombination(possibleCombinations[i]))
            {
                StartCoroutine(SetLevelComplete());
                return;
            }
        }

        StartCoroutine(SetWrongFoldPaper());
    }

    private void WriteTestingInfo()
    {
        var message = _foldedFolding
            .Select(x => x.name)
            .Aggregate((report, element) => report + "|" + element);
        LastCombinationViewer.Write(name + ":" + message);
    }

    private bool MatchPossibleCombination(PossibleCombination foldingsCombination)
    {
        if (_foldedFolding.Count != foldingsCombination.GetFoldings().Length)
            return false;

        for (int i = 0; i < _foldedFolding.Count; i++)
            if (_foldedFolding[i] != foldingsCombination.GetFoldings()[i])
                return false;

        return true;
    }


    private IEnumerator SetLevelComplete()
    {
        Debug.Log("Level Complete");

        if (_decalPaper || _test)
            yield break;


        _canFold = false;

        if (YandexSDK.instance != null)
        {
            YandexSDK.instance.ShowInterstitial();
        }

        LeanTween.moveZ(gameObject, -20f, 0.7f).setEaseInBack().setOnComplete(() => { });

        if (effect.StickerEffect != null)
        {
            StickerEffect stickerEffect = Instantiate(effect.StickerEffect);
            stickerEffect.CachedTransform.position = transform.position;

            stickerEffect.CachedSpriteRenderer.flipX = effect.FlipX;
            stickerEffect.CachedSpriteRenderer.flipY = effect.FlipY;
            onPaperShowEffect?.Invoke();
            yield return stickerEffect.ShowEffect(effect.SpriteImage, effect.SpriteSize, effect.SpriteRotate);
            yield return new WaitForSeconds(0.5f);
            Destroy(stickerEffect.gameObject);
        }

        UIManager.setLevelCompleteDelegate?.Invoke();
        yield break;
    }

    private IEnumerator SetWrongFoldPaper()
    {
        bool animationComplete = false;

        Debug.Log("Wrong!");

        LeanTween.moveLocalX(gameObject, transform.position.x + 0.1f, 0.1f).setEaseShake().setOnComplete(() =>
        {
            LeanTween.moveLocalX(gameObject, transform.position.x - 0.1f, 0.1f).setEaseShake()
                .setOnComplete(() => { animationComplete = true; });
        });

        yield return new WaitUntil(() => animationComplete);
        animationComplete = false;

        if (_decalPaper || _test)
            yield break;

        UnfoldAllFoldings();

        if (YandexSDK.instance != null)
            YandexSDK.instance.ShowInterstitial();

        UIManager.wrongPaperFolded?.Invoke();

        yield break;
    }

    private Vector3 GetRotatedLocalVertex(Vector3 localVertexToMove, float angle)
    {
        Vector3 vertexWorldPos = paperBackRenderer.transform.TransformPoint(localVertexToMove);
        Vector3 rotatedWorldVertex = GetRotatedPoint(vertexWorldPos, angle, currentFolding.GetRotationAxis());
        Vector3 rotatedLocalVertex = paperBackRenderer.transform.InverseTransformPoint(rotatedWorldVertex);

        return rotatedLocalVertex;
    }

    public Folding[] GetFoldings(bool includeInactive = true)
    {
        return foldingsParent.GetComponentsInChildren<Folding>(includeInactive);
    }

    public MeshRenderer GetFrontRenderer()
    {
        return paperFrontRenderer;
    }

    public bool AvailableFoldingsEnded()
    {
        return _foldedFolding.Count >= _folding.Length;
    }

    public void UnfoldAllFoldings()
    {
        if (_foldedFolding.Count <= 0 || !_canFold)
            return;

        currentFolding = _foldedFolding[0];

        _canFold = false;

        StartCoroutine(UnfoldingProcedureCoroutine());
    }
}

[System.Serializable]
public struct PossibleCombination
{
    [SerializeField] private Folding[] foldings;

    public Folding[] GetFoldings()
    {
        return foldings;
    }
}

[System.Serializable]
public struct PaperEffect
{
    [SerializeField] private StickerEffect _stickerEffect;
    [SerializeField] private Sprite _spriteImage;
    [SerializeField] private Vector3 _spriteSize;
    [SerializeField] private Quaternion _spriteRotate;
    [SerializeField] private bool _flipX;
    [SerializeField] private bool _flipY;

    public StickerEffect StickerEffect => _stickerEffect;
    public Sprite SpriteImage => _spriteImage;
    public Vector3 SpriteSize => _spriteSize;
    public Quaternion SpriteRotate => _spriteRotate;
    public bool FlipX => _flipX;
    public bool FlipY => _flipY;
}