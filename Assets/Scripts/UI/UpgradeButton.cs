using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Managers;
using Player;
using UnityEngine.EventSystems;
using Utils;
using DG.Tweening;

namespace UI
{
    public class UpgradeButton : MonoBehaviour
    {
        [SerializeField] private Upgrade upgradeData;
        [SerializeField] private RectTransform targetToShake;
        
        
        private Button buyButton;
        private UpgradeManager upgradeManager;

        private void Start()
        {
            buyButton = GetComponent<Button>();
            upgradeManager = FindFirstObjectByType<UpgradeManager>();

            targetToShake = buyButton.GetComponent<RectTransform>();

            buyButton.onClick.AddListener(BuyUpgrade);
            UpdateButtonState();
        }
        
        private void BuyUpgrade()
        {
            Debug.Log(">>> BuyUpgrade called!");

            if (!buyButton.interactable)
            {
                Debug.Log("Buy button is not interactable");
                return;
            }

            if (upgradeData == null)
            {
                Debug.LogWarning("No upgrade data assigned!");
                return;
            }

            bool success = upgradeManager.TryBuyUpgrade(upgradeData);

            if (success)
            {
                Debug.Log($"Purchased upgrade: {upgradeData.upgradeName}");
                UpdateButtonState();
                // SoundManager.Instance.PlayBuyUpgrade(); //sound of purchase
                SoundManager.Instance.PlaySFX(SoundManager.Instance.buyUpgrade, 0.4f); // sound of purchase

            }
            else
            {
                Debug.Log("Could not purchase upgrade – not enough money?");
                StartCoroutine(ShakeWithDelay());
            }
        }
        private IEnumerator ShakeWithDelay()
        {
            yield return null;

            if (targetToShake != null && targetToShake.gameObject.activeInHierarchy)
            {
                targetToShake.DOKill();
                targetToShake.DOShakePosition(0.4f, new Vector3(10f, 0f, 0f), vibrato: 10)
                    .SetUpdate(true);
            }
        }

        
        private void UpdateButtonState()
        {
            if (upgradeManager.HasReachedLimit(upgradeData.type))
            {
                buyButton.interactable = false;

                var colors = buyButton.colors;
                colors.disabledColor = new Color(0.3f, 0.3f, 0.3f);
                buyButton.colors = colors;
            }
            else
            {
                // Refresh the state to trigger hover again if needed
                buyButton.interactable = true;
                EventSystem.current.SetSelectedGameObject(null);
            }
        }


        
    }
}
