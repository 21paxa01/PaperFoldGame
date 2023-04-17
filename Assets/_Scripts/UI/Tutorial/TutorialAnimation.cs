using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Image))]
    public class TutorialAnimation : MonoBehaviour
    {
        [SerializeField] private float _maxExpanding;
        [SerializeField] private float _time;
        [SerializeField] private AnimationCurve _sizeCurve;
        
        private Image _image;
        private Coroutine _showing;

        private void OnValidate()
        {
            PreprocessCurve();
        }

        private void Start()
        {
            _image = GetComponent<Image>();
            _image.preserveAspect = true;
            _image.raycastTarget = false;
            _image.enabled = false;
        }

        public void Show()
        {
            if(_showing != null)
                StopCoroutine(_showing);
            
            _image.enabled = true;
            _showing = StartCoroutine(ShowAnimation());
        }

        private IEnumerator ShowAnimation()
        {
            float startTime = Time.time;
            float endTime = Time.time + _time;

            float CompletionPercent()
            {
                return Mathf.InverseLerp(startTime, endTime, Time.time);
            }

            while (CompletionPercent() <= 1)
            {
                float normalizedSize = EvaluateCurveTimeNormalized(CompletionPercent());
                _image.rectTransform.sizeDelta = Vector2.one * (normalizedSize * _maxExpanding);
                yield return null;
            }

            _showing = null;
            _image.enabled = false;
        }

        private float EvaluateCurveTimeNormalized(float time)
        {
            var last = _sizeCurve.keys[_sizeCurve.keys.Length - 1];
            return _sizeCurve.Evaluate(last.time * time);
        }

        private void PreprocessCurve()
        {
            for (int i = 0; i < _sizeCurve.keys.Length; i++)
            {
                ClampKey(min: 0, max: 1, index: i);
            }
        }

        private void ClampKey(float min, float max, int index)
        {
            float value = _sizeCurve.keys[index].value;
            float time = _sizeCurve.keys[index].time;
            float clamped = Mathf.Clamp(value, min, max);
            _sizeCurve.MoveKey(index, new Keyframe(time, clamped));
        }

        public void Stop()
        {
            if(_showing != null)
                StopCoroutine(_showing);
            _image.enabled = false;
        }
    }
}