#if UNITY_EDITOR
using Eiko.YaSDK.Editor;
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Eiko.YaSDK
{
    public class YandexSDK : MonoBehaviour {

    #if UNITY_EDITOR
        [HideInInspector]
        public CanvasAddEditor editorCanvas;
    #endif
        public const int ReloadAdsSeconds = 30;
        public const string key = "AddsOff";
        public bool AdsEnabled { get; private set; }

        public static YandexSDK instance;
        [DllImport("__Internal")]
        private static extern void GetUserData();
        [DllImport("__Internal")]
        private static extern void ShowFullscreenAd();
        [DllImport("__Internal")]
        private static extern void SetData(string key,string value);
        [DllImport("__Internal")]
        private static extern void SetScore(string key, int value);
        [DllImport("__Internal")]
        private static extern void InitPlayerData();

        /// <summary>
        /// Returns an int value which is sent to index.html
        /// </summary>
        /// <param name="placement"></param>
        /// <returns></returns>
        [DllImport("__Internal")]
        private static extern int ShowRewardedAd(string placement);
        //[DllImport("__Internal")]
        //private static extern void GerReward();
        [DllImport("__Internal")]
        private static extern void AuthenticateUser();
        [DllImport("__Internal")]
        private static extern void InitPurchases();
        [DllImport("__Internal")]
        private static extern void Purchase(string id);
        [DllImport("__Internal")]
        private static extern string GetLang();

        [DllImport("__Internal")]
        private static extern void Review();
        [DllImport("__Internal")]
        private static extern void GetPurchases();

        public event Action addsOnReloaded;
        public event Action onUserDataReceived;

        public event Action onInterstitialShown;
        public event Action<string> onInterstitialFailed;
        /// <summary>
        /// Пользователь открыл рекламу
        /// </summary>
        public event Action<int> onRewardedAdOpened;
        /// <summary>
        /// Пользователь должен получить награду за просмотр рекламы
        /// </summary>
        public event Action<string> onRewardedAdReward;
        /// <summary>
        /// Пользователь закрыл рекламу
        /// </summary>
        public event Action<int> onRewardedAdClosed;
        /// <summary>
        /// Вызов/просмотр рекламы повлёк за собой ошибку
        /// </summary>
        public event Action<string> onRewardedAdError;
        /// <summary>
        /// Покупка успешно совершена
        /// </summary>
        public event Action<Purchase> onPurchaseSuccess;
        /// <summary>
        /// Покупка не удалась: в консоли разработчика не добавлен товар с таким id,
        /// пользователь не авторизовался, передумал и закрыл окно оплаты,
        /// истекло отведенное на покупку время, не хватило денег и т. д.
        /// </summary>
        public event Action<string> onPurchaseFailed;

        public event Action onClose;
        public event Action onPurchaseInitialize;
        public event Action onPurchaseInitializeFailed;
        public Queue<int> rewardedAdPlacementsAsInt = new Queue<int>();
        public Queue<string> rewardedAdsPlacements = new Queue<string>();
        private Action<ReviewCallback> actionReview;
        public bool addsAvailable;
        private bool IsReviewed = false;
        public UserData user;
        public string adsPurchize="AddOff";
        public string Lang = "ru";

        private void Awake() {
            if (instance == null) {
                instance = this;
                DontDestroyOnLoad(gameObject);
#if !UNITY_EDITOR && UNITY_WEBGL
                Lang = GetLang();
#endif
            }
            else {
                Destroy(gameObject);
            }
            StartCoroutine(WaitAddReload());
#if UNITY_EDITOR
            editorCanvas =  Instantiate(editorCanvas);
#endif
            AdsEnabled = 0 == PlayerPrefs.GetInt(key, 0);
            onPurchaseSuccess += PurchizeCallbackAds;
        }

        /// <summary>
        /// Call this to ask user to authenticate
        /// </summary>
        public void Authenticate() {
#if !UNITY_EDITOR && UNITY_WEBGL
            AuthenticateUser();
#endif
        }

        /// <summary>
        /// Call this to show interstitial ad. Don't call frequently. There is a 3 minute delay after each show.
        /// </summary>
        public void ShowInterstitial() {
            if(addsAvailable)
            {
                StartCoroutine(WaitAddReload());
#if !UNITY_EDITOR && UNITY_WEBGL
                ShowFullscreenAd();
#else
                editorCanvas.OpenFullScreen();
#endif
                Time.timeScale = 0;
            }
            else
            {
                Debug.LogWarning("Ad not ready!");
            }
        }

        /// <summary>
        /// Call this to show rewarded ad
        /// </summary>
        /// <param name="placement"></param>
        public void ShowRewarded(string placement) {
#if !UNITY_EDITOR && UNITY_WEBGL
            int placemantId = ShowRewardedAd(placement);
#else
            int placemantId = 0;
#endif
            rewardedAdPlacementsAsInt.Enqueue(placemantId);
            rewardedAdsPlacements.Enqueue(placement);
            Time.timeScale = 0;
            AudioListener.pause = true;
#if UNITY_EDITOR
            editorCanvas.OpenReward(placemantId);
            
#endif
        }

        public void AdsOff()
        {
            PlayerPrefs.SetInt(key, 1);
            AdsEnabled = false;
            StopAllCoroutines();
            Debug.Log("AdsOff");
            addsAvailable = false;
        }
        public void ProcessPurchizeAdsDisabled(Action action= null)
        {
            this.action = action;
            ProcessPurchase(adsPurchize);
        }
        private Action action;
        private void PurchizeCallbackAds(Purchase purchase)
        {

            if (purchase.productID == adsPurchize)
            {
                AdsOff();
                onPurchaseSuccess -= PurchizeCallbackAds;
                action?.Invoke();
            }
        }
        /// <summary>
        /// Call this to receive user data
        /// </summary>
        public void RequestUserData() {
#if !UNITY_EDITOR && UNITY_WEBGL
            GetUserData();
#endif
        }
    
        public void InitializePurchases() {
#if !UNITY_EDITOR && UNITY_WEBGL
            InitPurchases();
#endif
        }

        public void ProcessPurchase(string id) {
#if !UNITY_EDITOR && UNITY_WEBGL
            Purchase(id);
#else
            OnPurchaseSuccess(id);
#endif
        }
    
        public void StoreUserData(string data) {
            user = JsonUtility.FromJson<UserData>(data);
            onUserDataReceived?.Invoke();
        }

        /// <summary>
        /// Callback from index.html
        /// </summary>
        public void OnInterstitialShown() {
            Time.timeScale = 1;
            onInterstitialShown?.Invoke();
        }

        /// <summary>
        /// Callback from index.html
        /// </summary>
        /// <param name="error"></param>
        public void OnInterstitialError(string error) {
            Time.timeScale = 1;
            onInterstitialFailed?.Invoke(error);
        }

        /// <summary>
        /// Callback from index.html
        /// </summary>
        /// <param name="placement"></param>
        public void OnRewardedOpen(int placement) {
            onRewardedAdOpened?.Invoke(placement);
        }

        /// <summary>
        /// Callback from index.html
        /// </summary>
        /// <param name="placement"></param>
        public void OnRewarded(int placement) {
            Time.timeScale = 1;
            if (placement == rewardedAdPlacementsAsInt.Dequeue()) {
                onRewardedAdReward?.Invoke(rewardedAdsPlacements.Dequeue());
            }

        }

        /// <summary>
        /// Callback from index.html
        /// </summary>
        /// <param name="placement"></param>
        public void OnRewardedClose(int placement) {
            AudioListener.pause = false;
            Time.timeScale = 1;
            onRewardedAdClosed?.Invoke(placement);
        }

        /// <summary>
        /// Callback from index.html
        /// </summary>
        /// <param name="placement"></param>
        public void OnRewardedError(string placement) {
            Time.timeScale = 1;
            AudioListener.pause = false;
            onRewardedAdError?.Invoke(placement);
            rewardedAdsPlacements.Clear();
            rewardedAdPlacementsAsInt.Clear();
        }

        public void OnPurchaseSuccess(string json)
        {
            var purchase = JsonUtility.FromJson<Purchase>(json);
            onPurchaseSuccess?.Invoke(purchase);
        }

        /// <summary>
        /// Callback from index.html
        /// </summary>
        /// <param name="error"></param>
        public void OnPurchaseFailed(string error) {
            onPurchaseFailed?.Invoke(error);
        }
    
        /// <summary>
        /// Browser tab has been closed
        /// </summary>
        /// <param name="error"></param>
        public void OnClose() {
            onClose?.Invoke();
        }
        public event Action<GetPurchasesCallback> GettedPurchase;
        public void TryGetPurchases()
        {
            GetPurchases();
        }
        public IEnumerator WaitAddReload()
        {
            addsAvailable = false;
            yield return new WaitForSecondsRealtime(ReloadAdsSeconds);
            addsAvailable = true;
            addsOnReloaded?.Invoke();
        }
        public void ShowReview(Action<ReviewCallback> action = null)
        {
            actionReview = action;
            if (IsReviewed)
            {
                OnReview(JsonUtility.ToJson(
                new ReviewCallback()
                {
                    CanReview = false,
                    FeedbackSent = false,
                    Reason = IsReviewed ? "GAME_RATED" : "Success"
                }));
                
                return;
            }
#if !UNITY_EDITOR && UNITY_WEBGL
            Review();
#else
            editorCanvas.ShowReview();
#endif
        }
        public void OnReview(string callback)
        {
            ReviewCallback review = JsonUtility.FromJson<ReviewCallback>(callback);
            if(review.FeedbackSent)
            {
                IsReviewed = true;
            }
            actionReview?.Invoke(review);            
        }
        public event Action<GetDataCallback> onDataRecived;
        public event Action noAutorized;
        public void OnGetData(string json)
        {
            GetDataCallback callback;
            if (!string.IsNullOrEmpty(json))
            {
                callback = JsonUtility.FromJson<GetDataCallback>(json); 
                if(callback.data==null)
                {
                    callback.data = new KeyValuePairStringCallback[0];
                }
                if(callback.score == null)
                {
                    callback.score = new KeyValuePairIntCallback[0];
                }
            }
            else
            {
                callback = new GetDataCallback();
                callback.data = new KeyValuePairStringCallback[0];
                callback.score = new KeyValuePairIntCallback[0];
            }
            onDataRecived?.Invoke(callback);
        }
        public void NoAutorized()
        {
            noAutorized?.Invoke();
        }
        
        public void SetPlayerData(string key, string value)
        {
#if !UNITY_EDITOR
            SetData(key, value);
#endif
        }
        
        public void SetPlayerScore(string key, int value)
        {
#if !UNITY_EDITOR
            SetScore(key, value);
#endif
        }

        public void InitData()
        {
#if !UNITY_EDITOR
            InitPlayerData();
#else
            NoAutorized();
#endif
        }
        public void OnPurchaseInitialize()
        {
            onPurchaseInitialize?.Invoke();
        }
        [ContextMenu("Test")]
        public void Test()
        {
            string a = "{\"purchases\":[{\"productID\":\"BuyAll\",\"purchaseTime\":0,\"purchaseToken\":\"957ff7fa-938a-4c14-9454-d8a4e990347a\"},{\"productID\":\"Skin11Unlocked\",\"purchaseTime\":0,\"purchaseToken\":\"111ab5c1-33ba-4835-92f3-acb7135ead2f\"},{\"productID\":\"BuyAll\",\"purchaseTime\":0,\"purchaseToken\":\"4fe915c5-59b9-43ef-9993-e96488c3fbd0\"},{\"productID\":\"Skin12Unlocked\",\"purchaseTime\":0,\"purchaseToken\":\"bf1cd480-cd97-44cb-9938-5f81db24c0e6\"},{\"productID\":\"AddOff\",\"purchaseTime\":0,\"purchaseToken\":\"044f9005-9a8e-4a16-9256-eaa513468d1f\"},{\"productID\":\"BuyAll\",\"purchaseTime\":0,\"purchaseToken\":\"a140dc49-2d57-403f-9352-7be76ba039e2\"},{\"productID\":\"AddOff\",\"purchaseTime\":0,\"purchaseToken\":\"a6894793-fc17-43c8-a03e-fb2afd1369e7\"}]}";
            var b = JsonUtility.FromJson<GetPurchasesCallback>(a);
            foreach (var item in b.purchases)
            {
                Debug.Log(item.productID);
            }
        }

        public void OnPurchaseInitializeFailed()
        {
            onPurchaseInitializeFailed?.Invoke();
        }
        public event Action onGetPurchaseFailed;
        public void OnGetPurchaseFailed()
        {
            onGetPurchaseFailed?.Invoke();
        }
    }
    [Serializable]
    public class GetDataCallback
    {
        public KeyValuePairStringCallback[] data;
        public KeyValuePairIntCallback[] score;
    }
    [Serializable]
    public class KeyValuePairStringCallback
    { 
        public string key;
        public string value;
    }
    [Serializable]
    public class KeyValuePairIntCallback
    {
        public string key;
        public int value;
    }

    public struct ReviewCallback
    {
        public bool CanReview;
        public string Reason;
        public bool FeedbackSent;
    }

    public struct UserData {
        public string id;
        public string name;
        public string avatarUrlSmall;
        public string avatarUrlMedium;
        public string avatarUrlLarge;
    }
    [Serializable]
    public class GetPurchasesCallback
    {
        public Purchase[] purchases;
        public string signature;
    }
    [Serializable]
    public class Purchase
    {
        public string productID;
        public string purchaseToken;
        public string developerPayload;
        public string signature;
    }
}