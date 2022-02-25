using System;
using System.Collections.Generic;
using Cleaning;
using Managers;
using UnityEngine;
using UnityEngine.Events;

namespace UI.Cleaning
{
    public class PopupTextManager : Manager
    {
        [SerializeField] private GameObject Prefab;
        [SerializeField] private int poolSize;
        public Queue<PopupTextUI> popupTextPool = new Queue<PopupTextUI>();

        protected override void Start()
        {
            for (int i = 0; i < poolSize; i++)
            {
                popupTextPool.Enqueue(
                    Instantiate(Prefab, transform.position, Quaternion.identity, transform)
                        .GetComponent<PopupTextUI>());
            }

            CleaningManager cleaningManager = M.GetOrThrow<CleaningManager>();
            CleaningScoreManager cleaningScoreManager = M.GetOrThrow<CleaningScoreManager>();
            CleaningTimerManager cleaningTimerManager = M.GetOrThrow<CleaningTimerManager>();
            // cleaningManager.artefactRockSucceeded.AddListener(
            //     CreatePopup(Vector3.zero,
            //         $"+{cleaningScoreManager.CurrentRockScore} \n + {cleaningTimerManager.BonusTime}",
            //         Color.white, TextSize.Small));
        }

        public void CreatePopup(Vector3 pos, string input, Color color, TextSize size = TextSize.Small)
        {
            var popup = popupTextPool.Dequeue();

            popup.Setup(pos, input, color, size);
            popupTextPool.Enqueue(popup);
        }

        [ContextMenu("TEST")]
        public void TestPopup()
        {
            CreatePopup(transform.position, "TEST +111", Color.white);
        }
    }
}