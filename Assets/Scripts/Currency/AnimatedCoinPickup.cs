using UnityEngine;
using DG.Tweening;

public class AnimatedCoinPickup : MonoBehaviour
{
    [SerializeField] private GameObject flyingCoinPrefab;
    [SerializeField] private RectTransform jarTargetUI;
    [SerializeField] private Canvas canvas;

    public void AnimateCoin(Vector3 worldPosition)
    {
        GameObject coin = Instantiate(flyingCoinPrefab, canvas.transform);
        
        Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPosition);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPos,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out Vector2 anchoredPos
        );

        RectTransform coinRect = coin.GetComponent<RectTransform>();
        coinRect.anchoredPosition = anchoredPos;

        // coinRect.DOMove(jarTargetUI.position, 0.6f)
        coinRect.DOMove(jarTargetUI.transform.position, 0.6f)
            .SetEase(Ease.OutBack)
            .OnComplete(() => Destroy(coin));
    }
}