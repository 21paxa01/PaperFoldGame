using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JetSystems;

public class FoldLinesGenerator : MonoBehaviour
{
    [Header(" Components ")]
    [SerializeField] private LineRenderer linePrefab;
    private Paper currentPaper;

    private void Awake()
    {
        LevelManager.onPaperInstantiated += DrawFoldingLines;
        UIManager.onLevelCompleteSet += ClearPaperDelegates;
    }

    private void OnDestroy()
    {
        LevelManager.onPaperInstantiated -= DrawFoldingLines;
    }

    private void ClearPaperDelegates(int none)
    {
        currentPaper.onPaperStateChanged -= UpdateFoldingLines;
        currentPaper = null;

        UpdateFoldingLines();
    }

    private void DrawFoldingLines(Paper paper)
    {
        currentPaper = paper;
        currentPaper.onPaperStateChanged += UpdateFoldingLines;

        UpdateFoldingLines();
    }

    private void UpdateFoldingLines()
    {
        transform.Clear();

        if (currentPaper == null) return;

        Folding[] foldings = currentPaper.GetFoldings(false);

        foreach (Folding folding in foldings)
            if (!folding.IsFolded())
                DrawFolding(folding);
    }

    private void DrawFolding(Folding folding)
    {
        LineRenderer line = Instantiate(linePrefab, transform);

        Vector3[] foldingDirection = folding.GetRotationAxis().AsLine();

        Vector3 p0 = foldingDirection[0].With(y: currentPaper.transform.position.y + -0.05f);
        Vector3 p1 = foldingDirection[1].With(y: currentPaper.transform.position.y + -0.05f);

        line.SetPosition(0, p0);
        line.SetPosition(1, p1);
    }
}
