using System;
using Rooms;
using UnityEngine;

namespace Tutorial
{
    public class TutorialContainer : MonoBehaviour
    {
        [SerializeField] private Interactable container;
        [SerializeField] private SpriteRenderer sprite;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            sprite.enabled = true;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            sprite.enabled = false;
        }
    }
}