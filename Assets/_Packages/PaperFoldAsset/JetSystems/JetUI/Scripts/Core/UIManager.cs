using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Eiko.YaSDK;
using Cysharp.Threading.Tasks;

namespace JetSystems
{
	public class UIManager : MonoBehaviour
	{
        public enum Orientation { Portrait, Landscape }
        public Orientation orientation;

        public enum GameState { MENU, GAME, LEVELCOMPLETE, GAMEOVER, SETTINGS, SHOP, LOADING }
        public static GameState gameState;

        #region Static Variables

        public static int COINS;
        public static UIManager instance;

        #endregion

        #region Delegates

        public delegate void SetMenuDelegate();
        public static SetMenuDelegate setMenuDelegate;

        public delegate void OnMenuSet();
        public static OnMenuSet onMenuSet;

        public delegate void OnLoadingSet();
        public static OnLoadingSet onLoadingSet;


        public delegate void SetGameDelegate();
        public static SetGameDelegate setGameDelegate;

        public delegate void OnGameSet();
        public static OnGameSet onGameSet;



        public delegate void SetLevelCompleteDelegate(int starsCount = 3);
        public static SetLevelCompleteDelegate setLevelCompleteDelegate;

        public delegate void OnLevelCompleteSet(int starsCount = 3);
        public static OnLevelCompleteSet onLevelCompleteSet;

        public delegate void WrongPaperFolded();
        public static WrongPaperFolded wrongPaperFolded;


        public delegate void SetGameoverDelegate();
        public static SetGameoverDelegate setGameoverDelegate;

        public delegate void OnGameoverSet();
        public static OnGameoverSet onGameoverSet;


        public delegate void SetSettingsDelegate();
        public static SetSettingsDelegate setSettingsDelegate;

        public delegate void OnSettingsSet();
        public static OnSettingsSet onSettingsSet;



        public delegate void UpdateProgressBarDelegate(float value);
        public static UpdateProgressBarDelegate updateProgressBarDelegate;

        public delegate UniTask OnNextLevelButtonPressed();
        public static OnNextLevelButtonPressed onNextLevelButtonPressed;

        public delegate UniTask OnNextLevelButtonPressedWithAd();
        public static OnNextLevelButtonPressedWithAd onNextLevelButtonPressedWithAd;

        public delegate void OnRetryButtonPressed();
        public static OnRetryButtonPressed onRetryButtonPressed;

        #endregion


        // Canvas Groups
        public CanvasGroup MENU;
        public CanvasGroup GAME;
        public CanvasGroup LEVELCOMPLETE;
        public CanvasGroup GAMEOVER;
        public CanvasGroup SETTINGS;
        public CanvasGroup LOADING;
        public UIThemesManager THEMES;
        public CanvasGroup[] canvases;

        // Menu UI
        public Text menuCoinsText;

        // Game UI
        public Text gameCoinsText;
        public LevelLangSwitcher levelText;

        // Shop UI
        public Text shopCoinsText;

        public Text levelCompleteEarnedCoinsTextAd;
        public Text levelCompleteEarnedCoinsText;
        public UIThemeUnlockProgress uiThemeUnlcokProgress;

        private void Awake()
        {
            if (instance == null)
                instance = this;

            // Store the canvases
            canvases = new CanvasGroup[] { MENU, GAME, LEVELCOMPLETE, GAMEOVER, SETTINGS, THEMES.CachedCanvasGroup, LOADING };

            // Get the coins amount
            
        }

        // Start is called before the first frame update
        void Start()
		{
            COINS = YanGamesSaveManager.GetCoins();
            UpdateCoins();

            // Configure the delegates
            ConfigureDelegates();
		}

        private void ConfigureDelegates()
        {
            // Basic events
            setMenuDelegate += SetMenu;
            setGameDelegate += SetGame;
            setLevelCompleteDelegate += SetLevelComplete;
            setGameoverDelegate += SetGameover;
            setSettingsDelegate += SetSettings;

            // Progress bar events
            updateProgressBarDelegate += UpdateProgressBar;
        }

        public void SetLoading()
        {
            gameState = GameState.LOADING;
            Utils.HideAllCGs(canvases, LOADING);

            onLoadingSet?.Invoke();
        }

        public void SetMenu()
        {
            gameState = GameState.MENU;
            Utils.HideAllCGs(canvases, MENU);

            // Invoke the delegate
            onMenuSet?.Invoke();
        }

        public void SetGame()
        {
            gameState = GameState.GAME;
            Utils.HideAllCGs(canvases, GAME);

            // Invoke the delegate
            onGameSet?.Invoke();

            uiThemeUnlcokProgress.gameObject.SetActive(false);

            // Update the level text
            levelText.UpdateLevelNumber(YanGamesSaveManager.GetLevel() + 1);
        }

        public void SetLevelComplete(int starsCount = 3)
        {
            gameState = GameState.LEVELCOMPLETE;
            Utils.HideAllCGs(canvases, LEVELCOMPLETE);

            // Invoke the delegate
            onLevelCompleteSet?.Invoke(starsCount);
        }

        public void SetGameover()
        {
            gameState = GameState.GAMEOVER;
            Utils.HideAllCGs(canvases, GAMEOVER);

            // Invoke the delegate
            onGameoverSet?.Invoke();
        }

        public void SetSettings()
        {
            gameState = GameState.SETTINGS;
            Utils.EnableCG(MENU);
            Utils.HideAllCGs(canvases, SETTINGS);

            // Invoke the delegate
            onSettingsSet?.Invoke();
        }

        public void SetShop()
        {
            gameState = GameState.SHOP;
            Utils.HideAllCGs(canvases, THEMES.CachedCanvasGroup);
        }


        public void CloseShop()
        {
            SetGame();
        }

        public void NextLevelButtonCallback()
        {
            // Invoke the next button delegate
            onNextLevelButtonPressed?.Invoke();
        }

        public void NextLevelButtonWithAddCallback()
        {
            YandexSDK.instance.ShowRewarded("CoinTripleBonus");
            onNextLevelButtonPressedWithAd?.Invoke();
        }

        public void RetryButtonCallback()
        {
            SetGame();

            // Invoke the retry button delegate
            onRetryButtonPressed?.Invoke();
        }

        public void CloseSettings()
        {
            SetGame();
        }

        public void UpdateProgressBar(float value)
        {
            
        }

        private void UpdateCoins()
        {
            string coinsString = COINS.ToString();

            menuCoinsText.text = coinsString;
            gameCoinsText.text = coinsString;
            shopCoinsText.text = coinsString;
        }

        public void UpdateEarnedCoins(int earnedCoins, int earnedCoinsWithAd)
        {
            if (levelCompleteEarnedCoinsText != null)
                levelCompleteEarnedCoinsText.text = $"+{earnedCoins}";

            if (levelCompleteEarnedCoinsTextAd != null)
                levelCompleteEarnedCoinsTextAd.text = $"+{earnedCoinsWithAd}";
        }

        #region Static Methods

        public static void AddCoins(int amount)
        {
            // Increase the amount of coins
            COINS += amount;

            // Update the coins
            instance.UpdateCoins();

            // Save the amount of coins
            YanGamesSaveManager.SaveCoins(COINS);
        }

        public static void RemoveCoins(int amount)
        {
            if(amount < 0 || amount > COINS)
            {
                Debug.LogError("Removed coins value is less then 0 or greater then COINS");
                return;
            }

            COINS -= amount;
            instance.UpdateCoins();
            YanGamesSaveManager.SaveCoins(COINS);
                
        }

        public static bool IsGame()
        {
            return gameState == GameState.GAME;
        }

        public static bool IsLevelComplete()
        {
            return gameState == GameState.LEVELCOMPLETE;
        }

        public static bool IsGameover()
        {
            return gameState == GameState.GAMEOVER;
        }

        #endregion
    }


}