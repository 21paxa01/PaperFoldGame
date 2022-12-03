using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JetSystems;

public class Raycaster : MonoBehaviour
{
    [Header(" Settings ")]
    [SerializeField] private bool playTesting;
    Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(playTesting || UIManager.IsGame())
                DetectClosestFolding();
        }
    }

    private void DetectClosestFolding()
    {
        Ray tapRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Physics.Raycast(tapRay, out hit, 50);

        if (hit.collider != null)
        {
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


                if (enter >= maxDistance || !foldingIntersected)
                    continue;

                if(enter < minDistance)
                {
                    minDistance = enter;
                    closestFoldingIndex = i;
                }
            }

            if(closestFoldingIndex >= 0)
            {
                Paper currentPaper = detectedFoldings[closestFoldingIndex].GetComponentInParent<Paper>();
                currentPaper.TryFold(detectedFoldings[closestFoldingIndex]);
            }

            
            /*
            if (enter >= maxDistance)
                foldingIntersected = false;

            if (foldingIntersected)
            {
                Paper currentPaper = currentFolding.GetComponentInParent<Paper>();
                currentPaper.TryFold(currentFolding);
            }*/


            //Debug.Log("Folding intersected : " + foldingIntersected + " -> enter = " + enter);
        }
    }

    /*
    private void DetectClosestFolding()
    {
        Ray tapRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Physics.Raycast(tapRay, out hit, 50);

        if (hit.collider != null)
        {
            Folding currentFolding = FindObjectOfType<Folding>();
            Plane plane = currentFolding.GetFoldingPlane();

            Ray ray = new Ray(hit.point, (Vector3.zero - hit.point));
            float maxDistance = Vector3.Distance(hit.point, Vector3.zero);

            float enter;
            bool foldingIntersected = plane.Raycast(ray, out enter);

            if (enter >= maxDistance)
                foldingIntersected = false;

            if(foldingIntersected)
            {
                Paper currentPaper = currentFolding.GetComponentInParent<Paper>();
                currentPaper.TryFold(currentFolding);
            }


            Debug.Log("Folding intersected : " + foldingIntersected + " -> enter = " + enter);
        }
    }
    */
}
