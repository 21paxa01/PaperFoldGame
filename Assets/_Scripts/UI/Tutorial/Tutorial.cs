using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Eiko.YaSDK.Data;
using ModestTree;
using UnityEngine;

namespace UI
{
    public class Tutorial : MonoBehaviour
    {
        [SerializeField] private float _distanceFromCenter;
        [SerializeField] private TutorialAnimation _animationPrefab;
        [SerializeField] private float _animationFrequency;
        [SerializeField] private float _animationTimeSpacing;

        private Dictionary<Folding, TutorialAnimation> _created;
        
        private Paper _paper;
        private Coroutine _showOverTime;

        private void Awake()
        {
            if (IsTutorialShownOnce())
                return;
            
            MarkThatTutorialWereAlreadyShown();
            LevelManager.onPaperInstantiated += OnPaperInstantiated;
            _showOverTime = StartCoroutine(ShowAnimationOverTime());
        }

        private bool IsTutorialShownOnce()
        {
            return YandexPrefs.GetInt("ShownOnce") == 1;
        }

        private void MarkThatTutorialWereAlreadyShown()
        {
            YandexPrefs.SetInt("ShownOnce", 1);
        }

        private void OnPaperInstantiated(Paper paper)
        {
            DestroyOld();
            _paper = paper;
            CreateAnimationInstances();
        }

        private IEnumerator ShowAnimationOverTime()
        {
            while (true)
            {
                if (_paper == null)
                {
                    yield return null;
                    continue;
                }
                foreach (PossibleCombination combination in _paper.possibleCombinations)
                {
                    foreach (Folding folding in combination.GetFoldings())
                    {
                        yield return new WaitForSeconds(_animationTimeSpacing);
                        _created[folding].Show();
                    }
                    yield return new WaitForSeconds(_animationFrequency);
                }

                yield return null;
            }
        }

        private void DestroyOld()
        {
            if (_created == null || _created.IsEmpty()) return;
            
            foreach (KeyValuePair<Folding, TutorialAnimation> keyValuePair in _created)
            {
                Destroy(keyValuePair.Value.gameObject);
                _created.Remove(keyValuePair.Key);
            }
        }

        private void CreateAnimationInstances()
        {
            Folding[] foldings = _paper.Foldings.ToArray();
            _created = new Dictionary<Folding, TutorialAnimation>(foldings.Length);
            
            for (var i = 0; i < foldings.Length; i++)
            {
                Vector3 position = Camera.main.WorldToScreenPoint(GetTutorialClickPoint(foldings[i]));
                TutorialAnimation instantiated = Instantiate(_animationPrefab, position, Quaternion.identity, transform);
                _created.Add(foldings[i], instantiated);
            }
        }

        private Vector3 GetTutorialClickPoint(Folding folding)
        {
            return GetPointFromCenterBehindFolding(folding) * _distanceFromCenter;
        }
        
        private Vector3 GetPointFromCenterBehindFolding(Folding folding)
        {
            Vector3 center = _paper.GetFrontRenderer().bounds.center;
            return GetClosestPointToPaperCenter(folding) - center;
        }

        private Vector3 GetClosestPointToPaperCenter(Folding folding)
        {
            Vector3 center = _paper.GetFrontRenderer().bounds.center;
            Vector3 closest = folding.GetFoldingPlane().ClosestPointOnPlane(center);
            return closest;
        }

        private void OnDisable()
        {
            LevelManager.onPaperInstantiated -= OnPaperInstantiated;
            if(_showOverTime != null)StopCoroutine(_showOverTime);
        }
    }
}