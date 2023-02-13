using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Eiko.YaSDK;
using Eiko.YaSDK.Data;

namespace JetSystems
{
    public static class PrefsKeys
    {
        public const string COINS_KEY = "COINS";
        public const string ITEM_UNLOCKED_KEY = "ITEMUNLOCKED";
        public const string SOUND_KEY = "SOUNDS";
        public const string LEVEL_KEY = "LEVEL";
        public const string USED_THEME = "USEDTHEME";
        public const string UNLOCKED_THEME = "UNLOCKED_THEME";
        public const string UNLOCK_THEME_PROGRESS = "UNLOCK_THEME_PROGRESS";
        public const string AD_OFF = "AD_OFF";
    }



    //public static class PlayerPrefsManager
    //{
    //    public static int GetCoins()
    //    { return PlayerPrefs.GetInt(PrefsKeys.COINS_KEY); }

    //    public static void SaveCoins(int coinsAmount)
    //    { PlayerPrefs.SetInt(PrefsKeys.COINS_KEY, coinsAmount); }

    //    public static string GetUsedTheme()
    //    {
    //        return PlayerPrefs.GetString(PrefsKeys.USED_THEME);
    //    }

    //    public static void SetUsedTheme(string themeId)
    //    {
    //        PlayerPrefs.SetString(PrefsKeys.USED_THEME, themeId);
    //    }

    //    public static void AddUnlockedTheme(string themeId)
    //    {
    //        PlayerPrefs.SetInt($"{PrefsKeys.UNLOCKED_THEME}_{themeId}", 1);
    //    }

    //    public static bool HasUnlokedTheme(string themeId)
    //    {
    //        return PlayerPrefs.HasKey($"{PrefsKeys.UNLOCKED_THEME}_{themeId}");
    //    }

    //    public static int GetItemUnlockedState(int itemIndex)
    //    { return PlayerPrefs.GetInt(PrefsKeys.ITEM_UNLOCKED_KEY + itemIndex); }

    //    public static void SetItemUnlockedState(int itemIndex, int state)
    //    { PlayerPrefs.SetInt(PrefsKeys.ITEM_UNLOCKED_KEY + itemIndex, state); }




    //    public static int GetSoundState()
    //    { return PlayerPrefs.GetInt(PrefsKeys.SOUND_KEY); }

    //    public static void SetSoundState(int state)
    //    { PlayerPrefs.SetInt(PrefsKeys.SOUND_KEY, state); }


    //    public static int GetUnlockThemeProgress()
    //    {
    //        return PlayerPrefs.GetInt(PrefsKeys.UNLOCK_THEME_PROGRESS);
    //    }

    //    public static void SetUnlockThemeProgress(int countLevelCompleated)
    //    {
    //        PlayerPrefs.SetInt(PrefsKeys.UNLOCK_THEME_PROGRESS, countLevelCompleated);
    //    }

    //    public static int GetLevel()
    //    { return PlayerPrefs.GetInt(PrefsKeys.LEVEL_KEY); }

    //    public static void SaveLevel(int level)
    //    { PlayerPrefs.SetInt(PrefsKeys.LEVEL_KEY, level); }


    //    public static void ClearAllData()
    //    {
    //        PlayerPrefs.DeleteAll();
    //    }
    //}

    public static class YanGamesSaveManager
    {
        public static int GetCoins()
        {
            return YandexPrefs.GetInt(PrefsKeys.COINS_KEY);
        }

        public static void SaveCoins(int coins)
        {
            YandexPrefs.SetInt(PrefsKeys.COINS_KEY, coins);
        }

        public static string GetUsedTheme()
        {
            return YandexPrefs.GetString(PrefsKeys.USED_THEME);
        }

        public static void SetUsedTheme(string themeId)
        {
            YandexPrefs.SetString(PrefsKeys.USED_THEME, themeId);
        }

        public static bool HasUnlokedTheme(string themeId)
        {
            int unlockedThemeState = YandexPrefs.GetInt($"{PrefsKeys.UNLOCKED_THEME}_{themeId}");
            return true && unlockedThemeState > 0;
        }

        public static void AddUnlockedTheme(string themeId)
        {
            YandexPrefs.SetInt($"{PrefsKeys.UNLOCKED_THEME}_{themeId}", 1);
        }

        public static int GetItemUnlockedState(int itemIndex)
        { 
            return YandexPrefs.GetInt(PrefsKeys.ITEM_UNLOCKED_KEY + itemIndex); 
        }

        public static void SetItemUnlockedState(int itemIndex, int state)
        {
            YandexPrefs.SetInt(PrefsKeys.ITEM_UNLOCKED_KEY + itemIndex, state); 
        }




        public static int GetSoundState()
        { 
            return YandexPrefs.GetInt(PrefsKeys.SOUND_KEY); 
        }

        public static void SetSoundState(int state)
        {
            YandexPrefs.SetInt(PrefsKeys.SOUND_KEY, state); 
        }


        public static int GetUnlockThemeProgress()
        {
            return YandexPrefs.GetInt(PrefsKeys.UNLOCK_THEME_PROGRESS);
        }

        public static void SetUnlockThemeProgress(int countLevelCompleated)
        {
            YandexPrefs.SetInt(PrefsKeys.UNLOCK_THEME_PROGRESS, countLevelCompleated);
        }

        public static int GetLevel()
        { 
            return YandexPrefs.GetInt(PrefsKeys.LEVEL_KEY); 
        }

        public static void SaveLevel(int level)
        {
            YandexPrefs.SetInt(PrefsKeys.LEVEL_KEY, level); 
        }

        public static void SetAdOff(bool value)
        {
            int adOffValue = value ? 1 : 0;

            YandexPrefs.SetInt(PrefsKeys.AD_OFF, adOffValue);
        }

        public static bool GetAdOff()
        {
            int adOffValue = YandexPrefs.GetInt(PrefsKeys.AD_OFF);
            return adOffValue >= 1;
        }
    }
}
