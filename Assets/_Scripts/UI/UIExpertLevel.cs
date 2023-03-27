using System;
using JetSystems;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIExpertLevel : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private Button _playButton;
        [SerializeField] private Image _expertStarNearLevel;

        private void Start()
        {
            Hide();
        }

        public void UpdateLevel(int level)
        {
            if (IsExpertLevel(level))
                Show();
            else Hide();
        }

        private bool IsExpertLevel(int level)
        {
            return LevelLangSwitcher.IsCurrentLevelExpert(level);
        }

        private void OnPlaying()
        {
            _playButton.onClick.RemoveAllListeners();
            Hide();
        }

        private void Show()
        {
            _expertStarNearLevel.gameObject.SetActive(true);
            _group.alpha = 1;
            _group.interactable = true;
            _group.blocksRaycasts = true;
            _playButton.onClick.AddListener(OnPlaying);
        }

        private void Hide()
        {
            _expertStarNearLevel.gameObject.SetActive(false);
            _group.alpha = 0;
            _group.interactable = false;
            _group.blocksRaycasts = false;
        }
    }
}