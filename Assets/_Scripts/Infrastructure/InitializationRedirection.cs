using Eiko.YaSDK;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Infrastructure
{
    public class InitializationRedirection : MonoBehaviour
    {
        private void Awake()
        {
            if (InitializationRequired())
            {
                SceneManager.LoadScene(0);
            }
        }
        
        private bool InitializationRequired()
        {
            return YandexSDK.instance == null;
        }
    }
}