using Eiko.YaSDK;
using JetSystems;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UITheme : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private Image _themeImage;
    [SerializeField] private LangSwitcher _themeText;
    [SerializeField] private Color _errorColor;
    [Header("Buy Buttons")]
    [SerializeField] private Text _defaultPriceText;
    [SerializeField] private Button _buyDefault;
    [Space]
    [SerializeField] private Text _inAppPriceText;
    [SerializeField] private Button _buyInApp;


    private readonly UnityEvent<UITheme, ThemeData> _themeAvailableClicked = new UnityEvent<UITheme, ThemeData>();

    private ThemeData _themeData;
    private Button _cachedThemeButton;
    private bool _isAvaialble;
    private bool _isBlockClick = false;


    public UnityEvent<UITheme, ThemeData> ThemeAvailableClicked => _themeAvailableClicked;
    public ThemeData ThemeData => _themeData;
    public Button CachedThemeButton => _cachedThemeButton;


    private void Awake()
    {
        _cachedThemeButton = GetComponent<Button>();
        _buyDefault.onClick.AddListener(OnBuyDefault);
        _buyInApp.onClick.AddListener(OnBuyInApp);
        _cachedThemeButton.onClick.AddListener(OnThemeClick);
    }

    private void Start()
    {
        YandexSDK.instance.onPurchaseSuccess += OnThemePurchaseDone;
    }

    private void OnDestroy()
    {
        _buyDefault.onClick.RemoveListener(OnBuyDefault);
        _buyInApp.onClick.RemoveListener(OnBuyInApp);
        _cachedThemeButton.onClick.RemoveListener(OnThemeClick);
        ThemeAvailableClicked.RemoveAllListeners();
    }

    private void OnThemeClick()
    {
        if (_isBlockClick)
            return;

        if (_isAvaialble)
        {
            ThemeAvailableClicked.Invoke(this, _themeData);
            return;
        }
    }


    private void OnBuyDefault()
    {
        if (_isBlockClick)
            return;

        if (UIManager.COINS < _themeData.Price)
        {
            _isBlockClick = true;

            Color defaultColor = _buyDefault.image.color;
            LeanTween.color(_buyDefault.image.rectTransform, _errorColor, 0.2f)
                .setEaseLinear()
                .setOnComplete(() =>
                {
                    LeanTween.color(_buyDefault.image.rectTransform, defaultColor, 0.2f)
                    .setEaseLinear()
                    .setOnComplete(() => _isBlockClick = false);
                });
        }
        else
        {
            UIManager.RemoveCoins(_themeData.Price);
            SetAvailable(true);
            YanGamesSaveManager.AddUnlockedTheme(_themeData.Id);
            ThemeAvailableClicked.Invoke(this, _themeData);
            AppMetricaWeb.Event("buyTheme300Gold");
        }
    }

    private void OnBuyInApp()
    {
        if (_isBlockClick)
            return;
        YandexSDK.instance.ProcessPurchase(_themeData.PurchaseId);
    }

    public void SetData(ThemeData themeData, bool isAvailable)
    {
        if(themeData.Sprite != null)
        {
            _themeText.gameObject.SetActive(false);
            _themeImage.sprite = themeData.Sprite;
            _themeImage.gameObject.SetActive(true);
        }
        else
        {
            _themeImage.gameObject.SetActive(false);

            _themeText.en = themeData.Name_EN;
            _themeText.ru = themeData.Name_RU;

            _themeText.gameObject.SetActive(true);
        }

        _defaultPriceText.text = themeData.Price.ToString();
        _inAppPriceText.text = themeData.InAppPrice.ToString();

        if (YanGamesSaveManager.HasUnlokedTheme(themeData.Id))
            isAvailable = true;

        SetAvailable(isAvailable);

        _themeData = themeData;
    }

    public void SetAvailable(bool available)
    {
        _buyDefault.gameObject.SetActive(!available);
        _buyInApp.gameObject.SetActive(!available);
        _buyDefault.transform.parent.gameObject.SetActive(!available);
        _isAvaialble = available;
    }

    private void OnThemePurchaseDone(Purchase purchase)
    {
        if(purchase.productID == _themeData.PurchaseId)
        {
            SetAvailable(true);
            YanGamesSaveManager.AddUnlockedTheme(_themeData.Id);
            ThemeAvailableClicked.Invoke(this, _themeData);
            AppMetricaWeb.Event("buyTheme2Yn");
        }
    }
}


