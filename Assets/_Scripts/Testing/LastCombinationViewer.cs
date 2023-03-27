using System;
using UnityEngine;

namespace Testing
{
    public class LastCombinationViewer : MonoBehaviour
    {
        [SerializeField] private float _width = 1000;
        [SerializeField] private float _height = 200;
        [SerializeField] private float _offsetYFromBottom = 200;
        [SerializeField] private float _offsetXFromLeft = 50;
        [SerializeField] private GUIStyle _textStyle;
        
        private static string Combination;
        
        public static void Write(string combination)
        {
            Combination = combination;
        }
        
        private void OnGUI()
        {
            GUI.Label(
                new Rect(_offsetXFromLeft, Screen.height - _offsetYFromBottom, _width, _height), 
                Combination,
                _textStyle);
        }
    }
}