using UnityEngine;
using Agava.WebUtility;

namespace Infrastructure
{
    public class Initializer : MonoBehaviour
    {
        private void Start()
        {
            WebApplication.InBackgroundChangeEvent += OnApplicationFocusChanged;
        }
        
        private void OnApplicationFocusChanged(bool hasFocus)
        {
            AudioListener.pause = !hasFocus;
            AudioListener.volume = hasFocus ? 1 : 0;
        }
    }
}