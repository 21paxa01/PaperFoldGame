using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class UIProgressBarPercent : MonoBehaviour
{
    [SerializeField] private Text _percentText;


    private Slider _cachedSlider;
    public Slider CachedSlider
    {
        get
        {
            if (_cachedSlider == null)
                _cachedSlider = GetComponent<Slider>();
            return _cachedSlider;
        }
    }


    private void Awake()
    {
        if (_percentText == null)
            Debug.LogError("_percentText value is not set!");

        _percentText.text = "0 %";
        CachedSlider.onValueChanged.AddListener(CalculatePrecentValue);
    }

    private void OnDestroy()
    {
        CachedSlider.onValueChanged.RemoveListener(CalculatePrecentValue);
    }


    public void CalculatePrecentValue(float sliderValue)
    {
        float percentValue = (100 / CachedSlider.maxValue) * sliderValue;

        _percentText.text = $"{Mathf.RoundToInt(percentValue)} %";
    }
}
