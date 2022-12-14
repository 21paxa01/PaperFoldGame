using JetSystems;
using UnityEngine;

public class SoundsManager : MonoBehaviour
{
    [Header(" Sounds ")]
    [SerializeField] private AudioSource paperSoundFold;
    [SerializeField] private AudioSource paperSoundUnfold;
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
        //UIManager.onGameSet -= StoreCurrentPaper;
        UIManager.onLevelCompleteSet -= ResetCurrentPaper;
        LevelManager.onPaperInstantiated -= StoreCurrentPaper;

    }

    private void StoreCurrentPaper(Paper paper)
    {
        currentPaper = paper;
        currentPaper.paperStartFold += PlayPaperFoldSound;
        currentPaper.paperStartUnfold += PlayPaperUnfoldSound;
    }

    private void StoreCurrentPaper()
    {
        currentPaper = FindObjectOfType<Paper>();
        currentPaper.paperStartFold += PlayPaperFoldSound;
        currentPaper.paperStartUnfold += PlayPaperUnfoldSound;
    }

    private void ResetCurrentPaper(int none)
    {
        //currentPaper.onPaperEvolving -= PlayPaperSound;

        currentPaper.paperStartFold -= PlayPaperFoldSound;
        currentPaper.paperStartUnfold -= PlayPaperUnfoldSound;
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
}
