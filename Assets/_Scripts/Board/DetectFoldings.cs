using JetSystems;
using UnityEngine;
using UnityEngine.EventSystems;


public class DetectFoldings : MonoBehaviour, IPointerUpHandler, IPointerClickHandler, IPointerDownHandler
{
    [Header(" Settings ")]
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
        RaycastHit hit;
        Physics.Raycast(tapRay, out hit, 50);

        if (hit.collider == null)
            return;

        Ray ray = new Ray(hit.point, (Vector3.zero - hit.point));
        float maxDistance = Vector3.Distance(hit.point, Vector3.zero);

        Folding[] detectedFoldings = FindObjectsOfType<Folding>();

        int closestFoldingIndex = -1;
        float minDistance = 5000;

        for (int i = 0; i < detectedFoldings.Length; i++)
        {
            Folding currentFolding = detectedFoldings[i];
            Plane plane = currentFolding.GetFoldingPlane();

            float enter;
            bool foldingIntersected = plane.Raycast(ray, out enter);


            if (enter >= maxDistance || !foldingIntersected)
                continue;

            if (enter < minDistance)
            {
                minDistance = enter;
                closestFoldingIndex = i;
            }
        }

        if (closestFoldingIndex >= 0)
        {
            Paper currentPaper = detectedFoldings[closestFoldingIndex].GetComponentInParent<Paper>();
            currentPaper.TryFold(detectedFoldings[closestFoldingIndex]);
        }
    }

}
