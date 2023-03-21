using System.Collections.Generic;
using UnityEngine;
namespace Eiko.YaSDK.Data
{
    public class YandexPrefsParams
    {
        public bool IsAutorised { get; set; } = false;
        public bool IsInit { get; set; } = false; 
        public Dictionary<string, string> data;
        public Dictionary<string, int> score;
        public YandexPrefsParams()
        {
            data = new Dictionary<string, string>();
            score = new Dictionary<string, int>();
        }
        public void Fill(GetDataCallback data)
        {
            foreach (var item in data.data)
            {
                this.data.Add(item.key, item.value);
            }
            foreach (var item in data.score)
            {
                this.score.Add(item.key, item.value);
            }
        }
    }
    public static class YandexPrefs
    {   
        private static YandexPrefsParams param;
        private static IPrefsProvider prefs = new NoInitializePrefsProvider();
        public static InitAsyncOperation Init()
        { 
            param = new YandexPrefsParams();
            var operation = new InitAsyncOperation(param);
            YandexSDK.instance.InitData();
            return operation;
        }
        public static void SetInt(string key, int value)
        {
            Debug.Log(key + " " + value);
            prefs.SetInt(key, value);
            if (param.IsAutorised)
            {
                param.score[key] = value;
                YandexSDK.instance.SetPlayerScore(key,value);
            }
            else
            {
                PlayerPrefs.SetInt(key,value);
            }
        }
        public static void SetString(string key, string value)
        {
            Debug.Log(key + " " + value);
            prefs.SetString(key, value);
            if (param.IsAutorised)
            {

                param.data[key]=value;
                YandexSDK.instance.SetPlayerData(key,value);
            }
            else
            {
                PlayerPrefs.SetString(key, value);
            }
        }
        public static int GetInt(string key)
        {
            //Debug.Log(key + " " + value);
            return prefs.GetInt(key);
            if (param.IsAutorised)
            {
                if (param.score.TryGetValue(key, out var value))
                    return value;
                else
                    return 0;
            }
            else
            {
                return PlayerPrefs.GetInt(key, 0);
            }
        }
        public static string GetString(string key)
        {
            return prefs.GetString(key);
            if (param.IsAutorised)
            {
                if (param.data.TryGetValue(key, out var value))
                    return value;
                else
                    return "";
                }
            else
            {
                return PlayerPrefs.GetString(key, "");
            }
        }
        public class InitAsyncOperation : CustomYieldInstruction
        {
            public InitAsyncOperation(YandexPrefsParams param)
            {
                YandexSDK.instance.noAutorized += Instance_noAutorized;
                YandexSDK.instance.onDataRecived += Instance_onDataRecived; ;
                this.param = param;
            }

            private void Instance_onDataRecived(GetDataCallback obj)
            {
                param.Fill(obj);
                Callback(true);
            }

            private void Instance_noAutorized()
            {
                Callback(false);
            }

            private YandexPrefsParams param;
            public bool IsSuccess;
            public override bool keepWaiting => _keepWaiting;
            private bool _keepWaiting = true;
            private void Callback(bool success)
            {
                param.IsInit = true;
                _keepWaiting = false;
                param.IsAutorised = success;
                IsSuccess = success;

                if (success)
                    prefs = new YandexPrefsProvider(param);
                else
                    prefs = new PlayerPrefsProvider();
            }
        }
    }
    
    public interface IPrefsProvider {
        public void SetInt(string key, int value);
        public void SetString(string key, string value);
        public int GetInt(string key);
        public string GetString(string key);
    }
    public class NoInitializePrefsProvider : IPrefsProvider
    {
        private const string ErrorMessage = "YandexPrefs no init!";
        public int GetInt(string key)
        {
            Debug.LogError(ErrorMessage);
            return 0;
        }

        public string GetString(string key)
        {
            Debug.LogError(ErrorMessage);
            return "";
        }

        public void SetInt(string key, int value)
        {
            Debug.LogError(ErrorMessage);
        }

        public void SetString(string key, string value)
        {
            Debug.LogError(ErrorMessage);
        }
    }
    public class YandexPrefsProvider : IPrefsProvider
    {
        private YandexPrefsParams param;
        public YandexPrefsProvider(YandexPrefsParams param)
        {
            this.param = param;
        }
            
        public void SetInt(string key, int value)
        {
            param.score[key] = value;
            YandexSDK.instance.SetPlayerScore(key, value);
        }
        public void SetString(string key, string value)
        {
            param.data[key] = value;
            YandexSDK.instance.SetPlayerData(key, value);
        }
        public int GetInt(string key)
        {
            if (param.score.TryGetValue(key, out var value))
                return value;
            else
                return 0;
        }
        public string GetString(string key)
        {
            if (param.data.TryGetValue(key, out var value))
                return value;
            else
                return "";
        }
    }
    public class PlayerPrefsProvider: IPrefsProvider
    {
        public void SetInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);   
        }
        public void SetString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);   
        }
        public int GetInt(string key)
        {
            return PlayerPrefs.GetInt(key, 0);   
        }
        public string GetString(string key)
        {
            return PlayerPrefs.GetString(key, "");   
        }
    }

}