using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JetSystems;

public class UIThemeUnlockProgress : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] _unlockEffects;
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


    private void ThemeUnlockProgressUpdate(int themeUnlockProgress, int themeUnlockProgressLevelStep)
    {
        _progressBar.value = PlayerPrefsManager.GetUnlockThemeProgress();
        gameObject.SetActive(true);

        _progressBar.maxValue = themeUnlockProgressLevelStep;
        StartCoroutine(UpdateProgressBarCoroutine(themeUnlockProgress));
    }

    private void ThemeUnlock(ThemeData themeData, int themeUnlockProgressLevelStep)
    {
        _progressBar.value = PlayerPrefsManager.GetUnlockThemeProgress();
        gameObject.SetActive(true);
        _progressBar.maxValue = themeUnlockProgressLevelStep;
        StartCoroutine(UpdateProgressBarCoroutine(themeUnlockProgressLevelStep));

        if(_unlockEffects.Length > 0)
        {
            foreach(ParticleSystem particleSystem in _unlockEffects)
            {
                particleSystem.Play();
            }
        }
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
