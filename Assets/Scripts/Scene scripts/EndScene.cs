using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class EndScene : MonoBehaviour
    {
        [SerializeField] private Button againButton;
        [SerializeField] private Button exitButton;
        

        private void Start()
        {
            againButton.onClick.AddListener(LoadStartScene);
            exitButton.onClick.AddListener(ExitGame);
        }

        private void ExitGame()
        {
            Application.Quit();
        }

        private void LoadStartScene()
        {
            SceneManager.LoadScene("Start");
        }
    }
}