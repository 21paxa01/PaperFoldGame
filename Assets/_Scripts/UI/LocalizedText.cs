using System;
using Eiko.YaSDK;
using TMPro;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class LocalizedText : MonoBehaviour
    {
        [SerializeField] private string _en;
        [SerializeField] private string _ru;

        private void Start()
        {
            GetComponent<TMP_Text>().text =
                YandexSDK.instance.Lang == "ru" ? _ru : _en;
        }
    }
}