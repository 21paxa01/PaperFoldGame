using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace JetSystems
{
    public class AnimatedElement : MonoBehaviour, IPointerDownHandler
    {
        public LeanTweenType tweenType;
        public float duration;

        private bool _buttonDowned = false;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_buttonDowned)
                return;

            _buttonDowned = true;
            LeanTween.scale(gameObject, Vector3.one * 0.9f, duration).setEase(tweenType).setOnComplete(() =>
            {
                _buttonDowned = false;
            });
        }
    }
}