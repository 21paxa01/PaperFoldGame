using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JetSystems;

public class SoundsManager : MonoBehaviour
{
    [Header(" Sounds ")]
    [SerializeField] private AudioSource paperSound;
    [SerializeField] private AudioSource levelCompleteSound;
    Paper currentPaper;

    // Start is called before the first frame update
    void Start()
    {
        UIManager.onGameSet += StoreCurrentPaper;
        UIManager.onLevelCompleteSet += ResetCurrentPaper;
        LevelManager.onPaperInstantiated += StoreCurrentPaper;
    }

    private void OnDestroy()
    {
        UIManager.onGameSet -= StoreCurrentPaper;
        UIManager.onLevelCompleteSet -= ResetCurrentPaper;
        LevelManager.onPaperInstantiated -= StoreCurrentPaper;

    }

    private void StoreCurrentPaper(Paper paper)
    {
        currentPaper = paper;// FindObjectOfType<Paper>();
        currentPaper.onPaperEvolving += PlayPaperSound;
    }

    private void StoreCurrentPaper()
    {
        currentPaper = FindObjectOfType<Paper>();
        currentPaper.onPaperEvolving += PlayPaperSound;
    }

    private void ResetCurrentPaper(int none)
    {
        currentPaper.onPaperEvolving -= PlayPaperSound;
        currentPaper = null;

        levelCompleteSound.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void PlayPaperSound()
    {
        paperSound.pitch = Random.Range(0.9f, 1.2f);
        paperSound.Play();
    }
}
