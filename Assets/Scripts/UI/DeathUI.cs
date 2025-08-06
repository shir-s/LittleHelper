using System;
using System.Collections;
using Managers;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI
{
    public class DeathUI : MonoBehaviour
    {
        [SerializeField] private Image background;
        [SerializeField] private Image topText;
        [SerializeField] private Image bottomText;
        
        [Header("Timings")]
        [SerializeField] private float delayBeforeShow = 3f;
        [SerializeField] private float backgroundFadeDuration = 1f;
        [SerializeField] private float textFadeDuration       = 0.5f;



        private void OnEnable()
        {
            GameEvents.SetUpDeath += ShowDeathUI;
            GameEvents.RestartLevel += ResetUI;
        }
        private void OnDisable()
        {
            GameEvents.SetUpDeath -= ShowDeathUI;
            GameEvents.SetUpDeath -= ResetUI;
        }

        private void ResetUI()
        {
            SetAlpha(background,   0f);
            SetAlpha(topText,      0f);
            SetAlpha(bottomText,   0f);
        }

        private void ShowDeathUI()
        {
            SetAlpha(background,   0f);
            SetAlpha(topText,      0f);
            SetAlpha(bottomText,   0f);

            StartCoroutine(ShowSequence());
        }
        
        private IEnumerator ShowSequence()
        {
            // 1) wait before showing anything
            yield return new WaitForSeconds(delayBeforeShow);

            // 2) fade in background
            yield return FadeImage(background, 0f, 0.95f, backgroundFadeDuration);

            // 3) fade in top text
            yield return FadeImage(topText, 0f, 1f, textFadeDuration);

            // 4) fade in bottom text
            yield return FadeImage(bottomText, 0f, 1f, textFadeDuration);
            
            //yield return new WaitForSeconds(2f);
            GameEvents.PlayerDied?.Invoke();
        }

        private IEnumerator FadeImage(Image img, float from, float to, float duration)
        {
            float elapsed = 0f;
            SetAlpha(img, from);
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                SetAlpha(img, Mathf.Lerp(from, to, t));
                yield return null;
            }
            SetAlpha(img, to);
        }

        private void SetAlpha(Image img, float a)
        {
            var c = img.color;
            c.a = a;
            img.color = c;
        }
    }
}