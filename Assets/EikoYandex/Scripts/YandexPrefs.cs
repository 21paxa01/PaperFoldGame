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
            Debug.Log(key+" " +value);
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
        public override bool keepWaiting =>_keepWaiting;
        private bool _keepWaiting = true;
        private void Callback(bool success)
        {
            param.IsInit = true;
            _keepWaiting = false;
            param.IsAutorised = success;
            IsSuccess = success;
        }
    }

}