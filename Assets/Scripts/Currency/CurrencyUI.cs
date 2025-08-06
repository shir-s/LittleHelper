using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Managers;

public class CurrencyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private Image jarImage;

    [Header("Jar Sprites")]
    [SerializeField] private Sprite jarEmpty;
    [SerializeField] private Sprite jar25;
    [SerializeField] private Sprite jar50;
    [SerializeField] private Sprite jar75;
    [SerializeField] private Sprite jarFull;

    public void UpdateMoneyUI(int money)
    {
        currencyText.text = money.ToString();

        if (money >= 400)
            jarImage.sprite = jarFull;
        else if (money >= 300)
            jarImage.sprite = jar75;
        else if (money >= 200)
            jarImage.sprite = jar50;
        else if (money >= 100)
            jarImage.sprite = jar25;
        else
            jarImage.sprite = jarEmpty;
    }
}