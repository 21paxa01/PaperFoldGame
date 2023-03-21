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
            if ((YanGamesSaveManager.GetLevel() - 1) % 10 == 0)
            {
                Show();
            }
            else
            {
                Hide();
            }
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