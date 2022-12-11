using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JetSystems;

public class UIThemeUnlockProgress : MonoBehaviour
{
    [SerializeField] private Slider _progressBar;
    [SerializeField] private Image _themeImage;
    [SerializeField] private Text _themeName;


    private void Awake()
    {
        LevelManager.themeUnlockProgressUpdated += ThemeUnlockProgressUpdate;
        LevelManager.themeUnlocked += ThemeUnlock;
    }

    private void OnDestroy()
    {
        LevelManager.themeUnlockProgressUpdated -= ThemeUnlockProgressUpdate;
        LevelManager.themeUnlocked -= ThemeUnlock;
    }


    private void ThemeUnlockProgressUpdate(ThemeData themeData, int themeUnlockProgress, int themeUnlockProgressLevelStep)
    {

        _progressBar.value = PlayerPrefsManager.GetUnlockThemeProgress();
        gameObject.SetActive(true);

        if(themeData.Sprite == null)
        {
            _themeImage.gameObject.SetActive(false);
            _themeName.text = themeData.Name;
            _themeName.gameObject.SetActive(true);
        }
        else
        {
            _themeName.gameObject.SetActive(false);
            _themeImage.sprite = themeData.Sprite;
            _themeImage.gameObject.SetActive(false);
        }

        _progressBar.maxValue = themeUnlockProgressLevelStep;
        StartCoroutine(UpdateProgressBarCoroutine(themeUnlockProgress));
    }

    private void ThemeUnlock(ThemeData themeData, int themeUnlockProgressLevelStep)
    {
        gameObject.SetActive(true);

        if (themeData.Sprite == null)
        {
            _themeImage.gameObject.SetActive(false);
            _themeName.text = themeData.Name;
            _themeName.gameObject.SetActive(true);
        }
        else
        {
            _themeName.gameObject.SetActive(false);
            _themeImage.sprite = themeData.Sprite;
            _themeImage.gameObject.SetActive(false);
        }

        _progressBar.maxValue = themeUnlockProgressLevelStep;
        StartCoroutine(UpdateProgressBarCoroutine(themeUnlockProgressLevelStep));
    }

    private IEnumerator UpdateProgressBarCoroutine(int unlockProgress)
    {
        float startProgress = _progressBar.value;
        float t = 0f;

        while (_progressBar.value < unlockProgress)
        {
            t += 2f * Time.deltaTime;
            float lerpProgess = Mathf.Lerp(startProgress, unlockProgress, t);
            _progressBar.value = lerpProgess;
            yield return null;
        }

        yield break;
    }
}
