using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JetSystems
{

    public static class PlayerPrefsManager
    {
        private const string COINS_KEY = "COINS";
        private const string ITEM_UNLOCKED_KEY = "ITEMUNLOCKED";
        private const string SOUND_KEY = "SOUNDS";
        private const string LEVEL_KEY = "LEVEL";
        private const string USED_THEME = "USEDTHEME";
        private const string UNLOCKED_THEME = "UNLOCKED_THEME";
        private const string UNLOCK_THEME_PROGRESS = "UNLOCK_THEME_PROGRESS";



        public static int GetCoins()
        { return PlayerPrefs.GetInt(COINS_KEY); }

        public static void SaveCoins(int coinsAmount)
        { PlayerPrefs.SetInt(COINS_KEY, coinsAmount); }



        public static string GetUsedTheme()
        {
            return PlayerPrefs.GetString(USED_THEME);
        }

        public static void SetUsedTheme(string themeId)
        {
            PlayerPrefs.SetString(USED_THEME, themeId);
        }

        public static void AddUnlockedTheme(string themeId)
        {
            PlayerPrefs.SetInt($"{UNLOCKED_THEME}_{themeId}", 1);
        }

        public static bool HasUnlokedTheme(string themeId)
        {
            return PlayerPrefs.HasKey($"{UNLOCKED_THEME}_{themeId}");
        }

        public static int GetItemUnlockedState(int itemIndex)
        { return PlayerPrefs.GetInt(ITEM_UNLOCKED_KEY + itemIndex); }

        public static void SetItemUnlockedState(int itemIndex, int state)
        { PlayerPrefs.SetInt(ITEM_UNLOCKED_KEY + itemIndex, state); }




        public static int GetSoundState()
        { return PlayerPrefs.GetInt(SOUND_KEY); }

        public static void SetSoundState(int state)
        { PlayerPrefs.SetInt(SOUND_KEY, state); }


        public static int GetUnlockThemeProgress()
        {
            return PlayerPrefs.GetInt(UNLOCK_THEME_PROGRESS);
        }

        public static void SetUnlockThemeProgress(int countLevelCompleated)
        {
            PlayerPrefs.SetInt(UNLOCK_THEME_PROGRESS, countLevelCompleated);
        }




        public static int GetLevel()
        { return PlayerPrefs.GetInt(LEVEL_KEY); }

        public static void SaveLevel(int level)
        { PlayerPrefs.SetInt(LEVEL_KEY, level); }


        public static void ClearAllData()
        {
            PlayerPrefs.DeleteAll();
        }
    }
}
