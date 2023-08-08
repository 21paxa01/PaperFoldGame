using System;
using System.Net;
using Agava.WebUtility;
using JetSystems;
using UnityEngine;
using Random = UnityEngine.Random;
using Eiko.YaSDK;

public class SoundsManager : MonoBehaviour
{
    [Header(" Sounds ")]
    [SerializeField] private AudioSource paperSoundFold;
    [SerializeField] public AudioSource main;
    [SerializeField] private AudioSource paperSoundUnfold;
    [SerializeField] private AudioSource paperSoundStickerEffect;
    [SerializeField] private AudioSource levelCompleteSound;
    Paper currentPaper;
    
    void Start()
    {
        //UIManager.onGameSet += StoreCurrentPaper;
        UIManager.onLevelCompleteSet += ResetCurrentPaper;
        LevelManager.onPaperInstantiated += StoreCurrentPaper;
        YandexSDK.instance.audio = main;
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        AudioListener.volume = pauseStatus ? 0 : 1;
        AudioListener.pause = pauseStatus;
    }

    private void OnDestroy()
    {
        //UIManager.onGameSet -= StoreCurrentPaper;
        UIManager.onLevelCompleteSet -= ResetCurrentPaper;
        LevelManager.onPaperInstantiated -= StoreCurrentPaper;

    }

    private void StoreCurrentPaper(Paper paper)
    {
        currentPaper = paper;
        currentPaper.paperStartFold += PlayPaperFoldSound;
        currentPaper.paperStartUnfold += PlayPaperUnfoldSound;
        currentPaper.onPaperShowEffect += PlayPaperShowStickerEffectSound;
    }

    /*
    private void StoreCurrentPaper()
    {
        currentPaper = FindObjectOfType<Paper>();
        currentPaper.paperStartFold += PlayPaperFoldSound;
        currentPaper.paperStartUnfold += PlayPaperUnfoldSound;
        currentPaper.onPaperShowEffect += PlayPaperShowStickerEffectSound;
    }
    */

    private void ResetCurrentPaper(int none)
    {
        //currentPaper.onPaperEvolving -= PlayPaperSound;

        currentPaper.paperStartFold -= PlayPaperFoldSound;
        currentPaper.paperStartUnfold -= PlayPaperUnfoldSound;
        currentPaper.onPaperShowEffect -= PlayPaperShowStickerEffectSound;
        currentPaper = null;

        levelCompleteSound.Play();
    }


    private void PlayPaperFoldSound()
    {
        paperSoundFold.pitch = Random.Range(0.9f, 1.2f);
        paperSoundFold.Play();
    }

    private void PlayPaperUnfoldSound()
    {
        paperSoundUnfold.pitch = Random.Range(0.9f, 1.2f);
        paperSoundUnfold.Play();
    }

    private void PlayPaperShowStickerEffectSound()
    {
        paperSoundStickerEffect.Play();
    }
}
