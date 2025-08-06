using UnityEngine;
using Managers;
using Utils;

public class CheatCodes : MonoBehaviour
{
    [SerializeField] private GameObject upgradePanel;
    private bool isPanelOpen = false;

    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.U)) 
        // {
        //     if (upgradePanel != null)
        //     {
        //         upgradePanel.SetActive(true);
        //     }
        // }
        
        if (Input.GetKeyDown(KeyCode.U))
        {
            isPanelOpen = !isPanelOpen;
            upgradePanel.SetActive(isPanelOpen);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            AddMoneyCheat(500);
        }
    }

    private void AddMoneyCheat(int amount)
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.AddMoney(amount);
            Debug.Log($"Cheat Activated: Added {amount} coins!");
        }
        else
        {
            Debug.LogWarning("CurrencyManager not found!");
        }
    }
}