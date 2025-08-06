using System.Collections;
using Scriptable_Objects;
using UnityEngine;
using Grandma;
using Item;
using Managers;
using Player;
using Utils;

namespace Rooms
{
    public class Interactable : MonoBehaviour
    {
        private static readonly int Shake = Animator.StringToHash("Shake");

        [Header("sprites")]
        [SerializeField] private Sprite closedSprite;
        [SerializeField] private Sprite openSprite;
        [SerializeField] private Animator animator;
        private bool _hasAnimator;
        private SpriteRenderer _sr;
        
        [Header("Item spawn point")]
        [SerializeField] private Transform itemSpawnPoint;
        private GrandmaQuestGiver _questGiver;
        private PlayerInventory _inventory;
        
        // [Header("Tutorial Containers")]
        [SerializeField] private bool isFirstTutorialContainer;
        
        
        private ItemAnimation _itemAnimation = new();
        
        private ItemDefinition _storedItem;
        public bool IsEmpty => _storedItem == null;
        
        private bool _isPlayerInRange;
        public bool IsPlayerInRange => _isPlayerInRange;
        private bool _isOpen;
        
        
        
        private void Awake()
        {
            _questGiver = FindFirstObjectByType<GrandmaQuestGiver>();
            _inventory = FindFirstObjectByType<PlayerInventory>();
            _sr   = GetComponentInParent<SpriteRenderer>();
            _sr.sprite = closedSprite;
            //animator = GetComponentInParent<Animator>();
            _hasAnimator = animator != null;
        }

        private void OnEnable()
        {
            _itemAnimation.OnItemRevealed += CheckItemRevealed;
            _itemAnimation.OnCorrectItemAnimation += PickUpItem;
            _itemAnimation.OnWrongItemAnimation += ReturnItem;
        }

        private void Update()
        {
            if (_isPlayerInRange && !_isOpen && Input.GetKeyDown(KeyCode.F))
            {
                Open();
            }
        }
        

        public void AddItem(ItemDefinition item) {_storedItem = item;}
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            _isPlayerInRange = true;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player")) _isPlayerInRange = false;
        }

        private void Open()
        {
            if (isFirstTutorialContainer)
            {
                GameEvents.TutorialContainerOpened?.Invoke();
            }
            print("box opened");
            SoundManager.Instance.PlayOpenContainer();
            _sr.sprite = openSprite;
            if (_hasAnimator) animator.SetTrigger(Shake);
            // Todo: add some kind of feedback that the object is empty
            _isOpen = true;
            if (_storedItem == null)
            {
                print("box is empty");
                StartCoroutine(CloseAfterDelay());
                return;
            }
            RevealItem();
        }

        private IEnumerator CloseAfterDelay()
        {
            yield return new WaitForSeconds(1f);
            _isOpen = false;
            _sr.sprite = closedSprite;
        }

        private void RevealItem()
        {
            var item = Instantiate(_storedItem.prefab, itemSpawnPoint.position, Quaternion.identity);
            var scale = item.transform.localScale;
            _itemAnimation.RevealItem(item,scale);
        }
 
        private void CheckItemRevealed(GameObject item)
        {
            if (_questGiver.IsCurrentItem(_storedItem))
            {
                Vector3 target = _inventory.transform.position;
                _itemAnimation.GrabItem(item,target);
                SoundManager.Instance.PlayCollectItem(); // sound of item pickup
            }
            else _itemAnimation.ShakeItemAndReturn(item,itemSpawnPoint.position);
        }

        private void PickUpItem(GameObject item)
        {
            bool gotIt = _inventory.PickUp(_storedItem);
            if (!gotIt) Debug.LogWarning("Couldn't pick up — inventory full?");
            Destroy(item);
            _storedItem = null;
            _isOpen = false;
            _sr.sprite = closedSprite;
        }

        private void ReturnItem(GameObject item)
        {
            // allow box to be opened again
            _isOpen = false;
            Destroy(item);
            _sr.sprite = closedSprite;
        }
    }
}