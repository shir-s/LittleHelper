using Managers;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Utils;

namespace UI
{
    public class FreezingRoomUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private Image uiFreeze;
        private float _maxTime;


        private void Awake()
        {
            if (timerText != null)
                timerText.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            GameEvents.OnFreezeStarted += t => _maxTime = t;
            GameEvents.OnTimerUpdated += UpdateTimer;
            GameEvents.OnTimerVisibilityChanged += ToggleTimerUI;
            GameEvents.OnTimerColorChanged += ChangeTimerColor;
        }

        private void OnDisable()
        {
            GameEvents.OnFreezeStarted -= t => _maxTime = t;
            GameEvents.OnTimerUpdated -= UpdateTimer;
            GameEvents.OnTimerVisibilityChanged -= ToggleTimerUI;
            GameEvents.OnTimerColorChanged -= ChangeTimerColor;
        }

        private void UpdateTimer(float time)
        {
            UpdateFreezeUI(time);
            bool isNegative = time < 0f;
            float displayTime = Mathf.Abs(time);
        
            int minutes = Mathf.FloorToInt(displayTime / 60f);
            int seconds = Mathf.FloorToInt(displayTime % 60f);
            int hundredths = Mathf.FloorToInt((displayTime * 100f) % 100f);
        
            string prefix = isNegative ? "-" : "";
            timerText.text = $"{prefix}{minutes:00}:{seconds:00}:{hundredths:00}";
            /*if (isNegative)
            {
                timerText.color = Color.red;
            }
            else
            {
                timerText.color = Color.white; 
            }*/
        }
        
        private void ChangeTimerColor(Color color)
        {
            timerText.color = color;
        }

        private void ToggleTimerUI(bool isVisible)
        {
            if (timerText != null)
                timerText.gameObject.SetActive(isVisible);
            SetTransparency(0f);
        }

        private void UpdateFreezeUI(float time)
        {
            float normalized = Mathf.Clamp01(time / _maxTime);
            float alpha      = 1f - normalized;
            SetTransparency(alpha);
        }
        
        private void SetTransparency(float alpha)
        {
            var c = uiFreeze.color;
            c.a = Mathf.Clamp01(alpha);
            uiFreeze.color = c;
        }
    }
}