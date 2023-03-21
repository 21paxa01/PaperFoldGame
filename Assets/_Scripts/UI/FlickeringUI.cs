using System;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class FlickeringUI : MonoBehaviour
    {
        private CanvasGroup _group;

        [SerializeField] private float _period;
        [SerializeField] private float _lowerBound;
        
        private void Start()
        {
            _group = GetComponent<CanvasGroup>();
        }

        private void Update()
        {
            _group.alpha = Mathf.Clamp(Mathf.Abs(Mathf.Sin(Time.time / _period)), _lowerBound, 1);
        }
    }
}