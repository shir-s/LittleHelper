using System;
using System.Collections;
using System.Collections.Generic;
using Item;
using Rooms;
using Scriptable_Objects;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;
using Player;

namespace Managers
{
    public class GameManager : MonoSingleton<GameManager>
    {
        [Header("Run Setup")] 
        [SerializeField] private int rounds = 7;
        [SerializeField] private WorldGenerator worldGenerator;
        [SerializeField] internal GameObject playerObject;
        [SerializeField] private CurrencyManager currencyManager;
        [SerializeField] private GameObject upgradePanel;
        [SerializeField] private GameObject winPanel;
        private bool _isUpgradePanelOpen = false;
        private bool _isWinPanelOpen = false;

        public List<Room> Rooms { get; private set;}
        
        [SerializeField] private WorldConfig worldConfig;
        
        public GameObject PlayerObject => playerObject; 
    
        private int _currentRound;

        public int CurrentRound => _currentRound;
        
        public Dictionary<UpgradeType, int> PurchasedUpgrades { get; private set; } = new();
        
       
        private bool isTutorialCompleted = true; //TODO: change the default to false after creating tutorial

        private void OnEnable()
        {
            GameEvents.PlayerDied += HandlePlayerDied;
            GameEvents.RestartLevel += RestartLevel;
            GameEvents.PlayerWon += PlayerWon;
        }
        private void OnDisable()
        {
            GameEvents.PlayerDied -= HandlePlayerDied;
            GameEvents.RestartLevel -= RestartLevel;
            GameEvents.PlayerWon -= PlayerWon;
        }

        // private void Start()
        // {
        //     if (SceneManager.GetActiveScene().name == "SandBox")
        //     {
        //         StartRun();
        //     }
        // }
        
        private void Start()
        {
            string currentScene = SceneManager.GetActiveScene().name;

            if (currentScene == "Start" || currentScene == "Win Scene")
            {
                SoundManager.Instance.PlayStartOrWinMusic();
            }
            else if (currentScene == "SandBox")
            {
                SoundManager.Instance.PlayBackgroundMusic();
                StartRun();
            }
        }

        
        // private void Update()
        // {
        //     if (Input.GetKeyDown(KeyCode.R))
        //     {
        //         GameEvents.RestartLevel.Invoke();
        //         //RestartLevel();
        //     }
        //     if(Input.GetKeyDown((KeyCode.Q)))
        //     {
        //         GameEvents.PlayerWon.Invoke();
        //         //PlayerWon();
        //     }
        // }

        private void RestartLevel()
        {
            if (_isUpgradePanelOpen)
            {
                upgradePanel.SetActive(false);
                _isUpgradePanelOpen = false;
            }
            //delete current rooms 
            foreach (Room room in Rooms)
            {
                if (room != null)
                    Destroy(room.gameObject);
            }
            Rooms.Clear();
            Physics2D.SyncTransforms();
            StartRun();
            Time.timeScale = 1;
        }
        
        private void ResetEntireGame()
        {
            if(_isWinPanelOpen)
                winPanel.SetActive(false);
            //TODO: ADD RESET TO THE UPGRADES
            currencyManager.ResetMoney();
            RestartLevel();
        }
        
        private void StartRun()
        {
            _currentRound = 0;
            print("generating world");
            GenerateWorld();
            GameEvents.StartQuest.Invoke();
            StartNextRound();
        }
        
        private void GenerateWorld()
        { 
            Rooms = worldGenerator.GenerateWorld();
            ItemPlacer.PopulateContainers(worldConfig.recipeItems,worldConfig.trashItems, Rooms);
            //ItemPlacer.PopulateContainers(worldConfig.trashItems, Rooms);
            //currencyManager.GetCoinSpawner().SetRooms(Rooms);
            currencyManager.GetCoinSpawner().InitialSpawn();
        }
        
        private void StartNextRound()
        {
            _currentRound++;
            foreach (var room in Rooms)
            {
                room.OnRoundStarted(_currentRound);
            }
        }
        
        public void HandlePlayerDied()
        {
            Debug.Log("Player Failed!");
            Time.timeScale = 0;
            upgradePanel.SetActive(true);
            _isUpgradePanelOpen = true;
            //SoundManager.Instance.PlayGameOver();
        }
        
        private void PlayerWon()
        {
            // Time.timeScale = 0;
            // winPanel.SetActive(true);
            // _isWinPanelOpen = true;
            // Debug.Log("Player Won!");
            
            // //CurrencyManager.Instance.ResetMoney();
            
            //TODO: ADD TIMER OR SMTH SO IT WONT GO STRAIGHT TO END SCENE
            StartCoroutine(ResetAfterDelay());
            
        }
        
        private IEnumerator ResetAfterDelay()
        {
            yield return new WaitForSecondsRealtime(3f); // מחכה 2 שניות אמיתיות, לא לפי Time.timeScale
            //ResetEntireGame();
            Time.timeScale = 1;
            //SceneManager.LoadScene("Start");
            SceneManager.LoadScene("Win Scene");
        }
        
        public void RegisterUpgrade(UpgradeType type)
        {
            if (!PurchasedUpgrades.ContainsKey(type))
            {
                PurchasedUpgrades[type] = 0;
            }
            PurchasedUpgrades[type]++;
        }
        
        public void OnStartGameButtonPressed()
        {
            if (isTutorialCompleted)
            {
                SoundManager.Instance.StopMusic();
                SceneManager.LoadScene("SandBox");
            }
        }
        
    }
}
