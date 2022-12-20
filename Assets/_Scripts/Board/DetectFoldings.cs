using JetSystems;
using UnityEngine;
using UnityEngine.EventSystems;


public class DetectFoldings : MonoBehaviour, IPointerUpHandler, IPointerClickHandler, IPointerDownHandler
{
    [Header(" Settings ")]
    [SerializeField] private LayerMask detectLayers;
    [SerializeField] private bool playTesting;
    Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (playTesting || UIManager.IsGame())
            DetectClosestFolding();
    }

    private void DetectClosestFolding()
    {
        Ray tapRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(tapRay, out RaycastHit hit, 50, detectLayers);

        if (hit.collider == null)
            return;

        Folding[] detectedFoldings = FindObjectsOfType<Folding>(true);

        int closestFoldingIndex = -1;
        float minDistance = 5000;

        for (int i = 0; i < detectedFoldings.Length; i++)
        {
            Folding currentFolding = detectedFoldings[i];

            float t = Vector3.Distance(hit.point, currentFolding.GetFoldingPosition());

            if(t < minDistance)
            {
                minDistance = t;
                closestFoldingIndex = i;
            }
        }

        if (closestFoldingIndex >= 0)
        {
            Folding colsestFolding = detectedFoldings[closestFoldingIndex];

            if (!colsestFolding.gameObject.activeInHierarchy)
                return;

            Paper currentPaper = colsestFolding.GetComponentInParent<Paper>();
            currentPaper.TryFold(colsestFolding);
        }
    }

}
