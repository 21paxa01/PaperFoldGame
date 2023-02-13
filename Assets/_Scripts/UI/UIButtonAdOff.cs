using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Eiko.YaSDK;
using JetSystems;

[RequireComponent(typeof(Button))]
public class UIButtonAdOff : MonoBehaviour
{
    private Button _cachedButton;

    public Button CachedButton
    {
        get
        {
            if (_cachedButton == null)
                _cachedButton = GetComponent<Button>();
            return _cachedButton;
        }
    }

    public void Awake()
    {
        CachedButton.onClick.AddListener(OnButtonClick);
    }

    public void Start()
    {
        bool isAdOff = YanGamesSaveManager.GetAdOff();
        if (isAdOff)
        {
            gameObject.SetActive(false);
            return;
        }

        YandexSDK.instance.onPurchaseSuccess += AdsOff;
    }

    public void OnDestroy()
    {
        CachedButton.onClick.RemoveAllListeners();
    }

    public void OnButtonClick()
    {
        YandexSDK.instance.ProcessPurchizeAdsDisabled();
    }

    public void AdsOff(Purchase purchase)
    {
        if(purchase.productID == YandexSDK.instance.adsPurchize)
        {
            gameObject.SetActive(false);
            YandexSDK.instance.AdsOff();
            YanGamesSaveManager.SetAdOff(true);
        }
    }
}
