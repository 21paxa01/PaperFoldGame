using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BaseSwitch : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool _isOn;
    [SerializeField] private Image _targetImage;
    [SerializeField] private Sprite _spriteSwitchOn;
    [SerializeField] private Sprite _spriteSwitchOff;


    private UnityEvent<bool> _valueChanged = new UnityEvent<bool>();


    public UnityEvent<bool> ValueChanged => _valueChanged;
    public bool IsOn
    {
        get { return _isOn; }
    }


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


    protected virtual void Awake()
    {
        AddEventListeners();
    }

    private void Start()
    {
        SwitchState(_isOn);
    }

    private void OnDestroy()
    {
        RemoveEventListeners();
    }


    private IEnumerator SwitchStateCoroutine()
    {
        float t = 0;
        float duration = 0.3f;
        Sprite currentSprite;

        if (_isOn)
            currentSprite = _spriteSwitchOn;
        else
            currentSprite = _spriteSwitchOff;

        Color currentColor = _targetImage.color;
        currentColor.a = 0f;
        _targetImage.color = currentColor;
        _targetImage.sprite = currentSprite;
        currentColor.a = 1f;

        while (t < duration + Time.deltaTime)
        {
            _targetImage.color = Color.Lerp(_targetImage.color, currentColor, t / duration);
            t = +Time.deltaTime;
            yield return null;
        }

        yield break;
    }


    protected virtual void AddEventListeners()
    {
        CachedButton.onClick.AddListener(SwitchState);
    }

    protected virtual void RemoveEventListeners()
    {
        CachedButton.onClick.RemoveListener(SwitchState);
    }


    public void SwitchState(bool stateOn)
    {
        _isOn = stateOn;
        _valueChanged.Invoke(_isOn);
        StartCoroutine(SwitchStateCoroutine());
    }

    public void SwitchState()
    {
        SwitchState(!_isOn);
    }
}
