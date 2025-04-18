using System.Collections.Generic;
using System.Linq;
using Project.Scripts.Inventory;
using Project.Scripts.NodeSystem.Dialogues;
using Project.Scripts.NodeSystem.Quests;
using Project.Scripts.Player;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Scripts.NPC
{
    [RequireComponent(typeof(DialogueCompanion))]
    public class InternQuests : MonoBehaviour
    {
        [Tooltip("Cooldown between x-y in seconds")]
        [SerializeField] private Vector2 internQuestPopupCooldown = new(10f, 10f);
        [LabelText("Chance 0-100%")]
        [SerializeField] private float chance = 100f;
        [SerializeField] private PlayerInteractController playerInteractController;

        [Header("Quests")]
        [SerializeField] private QuestGraph finalQuestGraph;
        [Space]
        [SerializeField] private QuestGraph[] internQuestsGraphs;

        private readonly List<QuestGraph> _remainingQuests = new();
        
        private DialogueCompanion _dialogueCompanion;
        private float _timer = 0f;

        private bool _isFinished = false;

        private void Awake()
        {
            _remainingQuests.AddRange(internQuestsGraphs);
            _dialogueCompanion = GetComponent<DialogueCompanion>();
            SetTimer();
        }

        private void Update()
        {
            if (_isFinished) return;
            
            _timer -= Time.deltaTime;
            if (_timer > 0f) return;
            
            if (DialogueManager.Instance.IsDialogueActive ||
                playerInteractController.inProgress)
            {
                _timer = 2f;
                return;
            }

            if (chance >= Random.Range(0f, 100f))
            {
                TriggerInternQuest();
            }
            SetTimer();
        }

        private void SetTimer()
        {
            _timer = Random.Range(internQuestPopupCooldown.x, internQuestPopupCooldown.y);
        }

        private void TriggerInternQuest()
        {
            if (_isFinished) return;
            
            QuestGraph questGraph;
            if (_remainingQuests.Count <= 0)
            {
                questGraph = finalQuestGraph;
                _isFinished = true;
            }
            else
            {
                questGraph = _remainingQuests.First();
                _remainingQuests.RemoveAt(0);
            }

            if (!questGraph)
            {
                return;
            }
            
            var playerInventory = playerInteractController.GetComponent<PlayerInventory>();
            var processor = QuestsManager.Instance.RegisterInternQuest(questGraph);
            processor.Start(playerInventory);
            DialogueManager.Instance.StartDialogue(_dialogueCompanion);
        }
    }
}