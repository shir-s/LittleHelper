using Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class StartScreen:MonoBehaviour
    {
        [SerializeField] Button startButton;
        
        
        void Start()
        {
            startButton.onClick.AddListener(LoadGameScene);
        }

        private void LoadGameScene()
        {
            SceneManager.LoadScene("SandBox");
            //GameManager.Instance.OnStartGameButtonPressed();
        }
        
    }
}