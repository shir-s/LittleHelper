using System;
using System.Collections.Generic;
using Grandma;
using Rooms;
using Scriptable_Objects;
using UnityEngine;
using Utils;

namespace Managers
{
    public class RoundManager : MonoSingleton<RoundManager>
    {
        [SerializeField] private GrandmaQuestGiver questGiver;

        private int _currentRound;

        
        private void Start()
        {
            //_currentRound = GameManager.Instance.CurrentRound;
            //StartNextRound();
        }

        private void OnEnable()
        {
            questGiver.OnItemDelivered += StartNextRound;
            GameEvents.StartQuest += StartRounds;
        }

        private void OnDisable()
        {
            questGiver.OnItemDelivered -= StartNextRound;
            GameEvents.StartQuest -= StartRounds;
        }
        
        private void StartRounds()
        {
            _currentRound = GameManager.Instance.CurrentRound;
            StartNextRound();
        }

        private void StartNextRound()
        {
            _currentRound++;
            //notify all rooms about next round (to increase difficultly)
            //foreach (var room in _rooms) room.OnRoundStarted(_currentRound);
            foreach (var room in GameManager.Instance.Rooms) room.OnRoundStarted(_currentRound);
        }

        private void RestartLevel()
        {
            _currentRound = GameManager.Instance.CurrentRound;
        }
    }
}
