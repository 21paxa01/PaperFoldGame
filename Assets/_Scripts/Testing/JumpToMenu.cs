using System;
using Cysharp.Threading.Tasks;
using JetSystems;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Testing
{
    public class JumpToMenu : MonoBehaviour
    {
        [Header("Box")]
        [SerializeField] private float _height = 40 * 4;
        [SerializeField] private float _width = 300;
        [SerializeField] private float _distanceFromTop = 5;
        [SerializeField] private float _distanceFromLeft = 5;
        [SerializeField] private string _header = "Testing";
        [Header("TextField")]
        [SerializeField] private float _textFieldYPositionRelative = 5;
        [SerializeField] private string _textFieldPlaceholder = "Level";
        [SerializeField] private float _textFieldWidth = 300;
        [SerializeField] private float _textFieldHeight = 40;
        [Header("Button")]
        [SerializeField] private float _jumpButtonYPositionRelative = 5;
        [SerializeField] private float _buttonWidth = 300;
        [SerializeField] private float _buttonHeight = 40;

        [Header("ErrorLabel")] 
        [SerializeField] private float _errorLabelYPositionRelative = 5;
        [SerializeField] private float _labelWidth = 300;
        [SerializeField] private float _labelHeight = 40;
        
        [Space(10)] [SerializeField] private GUIStyle _omnistyle;

        [SerializeField] private LevelManager _level;

        private bool _inputErrorDetected;
        private string _textFieldString;

        private void Start()
        {
            _textFieldString = _textFieldPlaceholder;
        }

        private void OnGUI()
        {
            var boxPosition = new Vector2(_distanceFromLeft, _distanceFromTop - _height);
            var boxSize = new Vector2(_width, _height);
            var boxRect = new Rect(boxPosition, boxSize);
            GUI.Box(boxRect, _header, _omnistyle);

            var textFieldPosition = boxPosition - Vector2.up * _textFieldYPositionRelative;
            var textFieldSize = new Vector2(_textFieldWidth, _textFieldHeight);
            var textFieldRect = new Rect(textFieldPosition, textFieldSize);
            _textFieldString = GUI.TextField(textFieldRect, _textFieldString, _omnistyle);
            
            var buttonPosition = textFieldPosition - Vector2.up * _jumpButtonYPositionRelative;
            var buttonSize = new Vector2(_buttonWidth, _buttonHeight);
            var buttonRect = new Rect(buttonPosition, buttonSize);
            
            if (GUI.Button(buttonRect, "Jump", _omnistyle))
            {
                JumpToScene(_textFieldString);
            }

            var labelPosition = buttonPosition - Vector2.up * _errorLabelYPositionRelative;
            var labelSize = new Vector2(_labelWidth, _labelHeight);
            var labelRect = new Rect(labelPosition, labelSize);
            
            if (_inputErrorDetected)
            {
                GUI.Label(labelRect, "Invalid input", _omnistyle);
            }
        }

        private void JumpToScene(string input)
        {
            if (int.TryParse(input, out int level) == false)
            {
                _inputErrorDetected = true;
                return;
            }
            YanGamesSaveManager.SaveLevel(level - 1);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}