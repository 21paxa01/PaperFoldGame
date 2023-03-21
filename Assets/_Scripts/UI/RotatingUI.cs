using System;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(RectTransform))]
    public class RotatingUI : MonoBehaviour
    {
        [SerializeField] private float _degreesPerSecond;
        
        private void Update()
        {
            transform.Rotate(Vector3.back, _degreesPerSecond * Time.deltaTime);       
        }
    }
}