using Eiko.YaSDK;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PurchaseProcess : MonoBehaviour
{

    private static PurchaseProcess instance;
    public List<Purchase> purchases;
    private string signature;
    private void Awake()
    {
        if(instance)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        InitPurchases();
    }
    public void InitPurchases()
    {
        YandexSDK.instance.onPurchaseSuccess += Instance_onPurchaseSuccess;
        YandexSDK.instance.GettedPurchase += Instance_GettedPurchase;
        YandexSDK.instance.onPurchaseInitialize += Instance_onPurchaseInitialize;
        YandexSDK.instance.InitializePurchases();
        
    }

    private void Instance_onPurchaseInitialize()
    {
        GetPurchases();
    }

    private void Instance_GettedPurchase(GetPurchasesCallback obj)
    {
        purchases = obj.purchases.ToList();
        signature = obj.signature;
    }

    private void Instance_onPurchaseSuccess(Purchase obj)
    {
        purchases.Add(obj);
    }

    private void GetPurchases()
    {
        YandexSDK.instance.TryGetPurchases();

    }
    public static bool Has(string id)
    {
        return null != instance.purchases.FirstOrDefault(x=>x.productID==id);
    }
}
