using Eiko.YaSDK;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LangSwitcher : MonoBehaviour
{
    public TextMeshProUGUI textTmp;
    public Text text;
    public string Text { get => text ? text.text : textTmp.text; set
        {
            if (text)
            {
                text.text = value;
            }
            else
            {
                textTmp.text = value;
            }
        }
}
    public string ru;
    public string en;
    private void OnValidate()
    {
        if(text == null && textTmp == null)
        {
            if (gameObject.TryGetComponent<TextMeshProUGUI>(out var tmp))
            {
                textTmp=tmp;
            }
            else if (gameObject.TryGetComponent<Text>(out var text))
            {
                this.text = text; 
            }
            else
            {
                Debug.LogError("GameObject must have a text component!");
                DestroyImmediate(this); 
            }
        }
        
    }
    public void Start()
    {
        if(YandexSDK.instance.Lang=="en")
        {
            Text= en;
        }
        else
        {
            Text = ru;
        }
        
    }
}
