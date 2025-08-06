using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Managers;
using Player;
using Rooms;
using Scriptable_Objects;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Utils;

namespace Grandma
{
    public class GrandmaQuestGiver : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private PlayerInventory playerInventory;
        
        [SerializeField] private WorldConfig worldConfig;
        
        [SerializeField] private Transform givenItemContainer;
        
        [SerializeField] private int    hintBlinkCount   = 3;
        [SerializeField] private float  hintDuration   = 2f;
        
        [SerializeField] private SpriteRenderer[] itemUI;
        private SpriteRenderer _activeSprite;
        
        private List<ItemDefinition> _remainingItems;
        [Header("Tutorial stuff")]
        [SerializeField] private ItemDefinition book;
        [SerializeField] private SpriteRenderer placedBook;
        [SerializeField] private SpriteRenderer bookSprite;
        [SerializeField] private SpriteRenderer freezerSprite;
        [SerializeField] private SpriteRenderer gardenSprite;
        [SerializeField] private SpriteRenderer basementSprite;
        [SerializeField] private Interactable firstTutorialContainer;
        [SerializeField] private GameObject door1;
        [SerializeField] private SpriteRenderer closedDoor1;
        [SerializeField] private GameObject door2;
        [SerializeField] private SpriteRenderer closedDoor2;
        [SerializeField] private SpriteRenderer fButton;
        
        
        private bool isTutorial = true;
        private ItemDefinition _currentItem;
        private bool _isPlayerInRange;
        private int _currentItemIndex = 0;
        
        public event Action OnItemDelivered;


        private void Start()
        {
            firstTutorialContainer.AddItem(book);
            door1.SetActive(false);
            door2.SetActive(false);
            placedBook.enabled = false;
        }

        private void OnEnable()
        {
            GameEvents.StartQuest += StartQuest;
            GameEvents.RestartLevel += ResetQuest;
            GameEvents.TutorialContainerOpened += EnableF;
        }

        private void EnableF()
        {
            if (isTutorial)
            {
                bookSprite.enabled = false;
                fButton.enabled = true;
            }
        }

        private void DisableF()
        {
            fButton.enabled = false;
        }

        private void ResetQuest()
        {
            _currentItemIndex = 0;
            SetNextItemGoal();
        }

        private void OnDisable()
        {
            GameEvents.StartQuest -= StartQuest;
            GameEvents.RestartLevel -= ResetQuest;
            GameEvents.TutorialContainerOpened -= EnableF;
        }
        
        private void StartQuest()
        {
            if (isTutorial)
            {
                SetTutorialItem();
                return;
            }
            _currentItemIndex = 0;
            SetNextItemGoal();
        }

        private void SetTutorialItem()
        {
            _currentItem = book;
        }

        private void Update()
        {
            if (!_isPlayerInRange || !Input.GetKeyDown(KeyCode.F)) return; 
            if(playerInventory.CurrentItem == _currentItem) ItemDelivered(playerInventory.CurrentItem);
        }


        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            _isPlayerInRange = true;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player")) _isPlayerInRange = false;
        }

        private void ItemDelivered(ItemDefinition itemDelivered)
        {
            if (itemDelivered != _currentItem) return;
            if (isTutorial)
            {
                print("finish tutorial");
                UnlockRoom();
                isTutorial = false;
                placedBook.enabled = true;
                DisableF();
            }
            else
            {
                // only for the _real_ game loop do we advance the round  
                OnItemDelivered?.Invoke();
            }
            
            playerInventory.DropItem();
            
           // OnItemDelivered?.Invoke();
            
            var item = Instantiate(itemDelivered.prefab, playerInventory.transform.position, Quaternion.identity, givenItemContainer);
            item.transform.localScale = itemDelivered.scale;
            item.transform.localRotation = itemDelivered.rotation;
            item.transform
                .DOJump(itemDelivered.deliveryPosition, 3, 1, 2)
                .SetEase(Ease.OutQuad);
            
            if(_currentItemIndex != worldConfig.recipeItems.Count) SetNextItemGoal();
            else
            {
                print("All Items Delivered");
                GameEvents.PlayerWon?.Invoke();
            }
        }

        private void UnlockRoom()
        {
            door1.SetActive(true);
            door2.SetActive(true);
            closedDoor1.enabled = false;
            closedDoor2.enabled = false;
        }


        private void SetNextItemGoal()
        {
            _currentItem = worldConfig.recipeItems[_currentItemIndex++];
            UpdateSpeechBubble();
            print($"current Item is {_currentItem.itemName}");
        }

        private void UpdateSpeechBubble()
        {
            if (_activeSprite != null)
            {
                _activeSprite.enabled = false;
            }

            if (isTutorial)
            {
                _activeSprite = bookSprite;
                return;
            }
            foreach (var sprite in itemUI)
            {
                if (sprite.name != _currentItem.itemName) continue;
                sprite.enabled = true;
                _activeSprite = sprite;
                ShowLocation();
            }
        }

        private void ShowLocation()
        {
            //disable the item sprite, then show the location sprite, then shaw the item sprite again, repeat 3 times
            if (_currentItemIndex > 3) return;
            switch (_currentItem.roomType)
            {
                case RoomType.Freezer:
                    StartCoroutine(ShowLocationRoutine(freezerSprite));
                    break;
                case RoomType.Garden:
                    StartCoroutine(ShowLocationRoutine(gardenSprite));
                    break;
                case RoomType.Pantry:
                    StartCoroutine(ShowLocationRoutine(basementSprite));
                    break;
                default:
                    return;
            }
        }
        
        private IEnumerator ShowLocationRoutine(SpriteRenderer locSprite)
        {
            for (int i = 0; i < hintBlinkCount; i++)
            {
                locSprite.enabled   = false;
                _activeSprite.enabled = true;
                yield return new WaitForSeconds(hintDuration);
                
                _activeSprite.enabled = false;
                locSprite.enabled   = true;
                yield return new WaitForSeconds(hintDuration);
            }
            locSprite.enabled   = false;
            _activeSprite.enabled = true;
        }

        public bool IsCurrentItem(ItemDefinition item)
        {
            print("your item is  " + item.itemName);
            print("current item is " + _currentItem.itemName);
            return _currentItem == item;
        }
    }
}