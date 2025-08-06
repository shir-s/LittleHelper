using System;
using DG.Tweening;
using UnityEngine;

namespace Item
{
    public class ItemAnimation
    {
        private readonly float _revealTime = 0.5f;
        private readonly float _collectTime = 0.2f;
        private readonly float _returnTime = 0.5f;
        private readonly float _wrongShakeDur = 0.3f;
        private readonly float _wrongShakeStr = .1f;
        private readonly float _jumpPower = 1f;
        private readonly int _jumpCount = 1;

        public Action<GameObject> OnItemRevealed;
        public Action<GameObject> OnCorrectItemAnimation;
        public Action<GameObject> OnWrongItemAnimation;

        public void RevealItem(GameObject item, Vector3 scale)
        {
            item.transform.localScale = Vector3.zero;
            item.transform
                .DOScale(scale, _revealTime)
                .OnComplete(() => OnItemRevealed?.Invoke(item));
        }


        public void GrabItem(GameObject item, Vector3 target)
        {
            item.transform
                .DOJump(target, _jumpPower, _jumpCount, _collectTime)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => OnCorrectItemAnimation?.Invoke(item));
        }

        public void ShakeItemAndReturn(GameObject item, Vector3 itemSpawnPoint)
        {
            //shake item
            item.transform
                .DOShakePosition(_wrongShakeDur, _wrongShakeStr)
                .OnComplete(() =>
                {
                    //return it to the box
                    item.transform
                        .DOMove(itemSpawnPoint, _returnTime)
                        .SetEase(Ease.InOutQuad)
                        .OnComplete(() => OnWrongItemAnimation?.Invoke(item));
                });
        }
    }
}