using System;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.Interactables;
using Project.Scripts.Inventory;
using Project.Scripts.NPC;
using Project.Scripts.Scriptable;
using UnityEngine;

namespace Project.Scripts.NodeSystem.Quests
{
    public class QuestsManager : MonoBehaviour
    {
        [SerializeField] private PlayerInventory playerInventory;
        
        public readonly Dictionary<ItemData, InteractableGetItem> interactablesToGetItem = new();
        public readonly Dictionary<NpcData, DialogueCompanion> dialogueCompanionByNpc = new();
        public List<QuestGraphProcessor> processors = new();

        public event Action OnQuestsChange;
        public event Action<QuestGraphProcessor> OnTimedQuestUpdate;
        
        private void Start()
        {
            var interactables = FindObjectsByType<InteractableGetItem>(FindObjectsSortMode.None).ToList();
            foreach (var interactable in interactables)
            {
                interactablesToGetItem.Add(interactable.GiveItemAfterInteract, interactable);
            }
            
            var dialogueCompanions = FindObjectsByType<DialogueCompanion>(FindObjectsSortMode.None).ToList();
            foreach (var dialogueCompanion in dialogueCompanions)
            {
                dialogueCompanionByNpc.Add(dialogueCompanion.NpcData, dialogueCompanion);
            }
            
            var questGraphs = Resources.LoadAll<QuestGraph>("").ToList();
            processors = new List<QuestGraphProcessor>(questGraphs.Count);
            foreach (var questGraph in questGraphs)
            {
                if (!questGraph.isQuestActive) continue;
                var processor = new QuestGraphProcessor(questGraph, this);
                processor.OnQuestUpdate += HandleProcessorQuestUpdate;
                processor.OnTimedQuestUpdate += HandleProcessorTimedQuestUpdate;
                processors.Add(processor);
            }
            
            // start quests
            foreach (var processor in processors)
            {
                processor.Start(playerInventory);
            }
            
            HandleProcessorQuestUpdate();
        }

        private void HandleProcessorTimedQuestUpdate(QuestGraphProcessor processor)
        {
            OnTimedQuestUpdate?.Invoke(processor);
        }

        private void HandleProcessorQuestUpdate()
        {
            // Set interactables availability
            foreach (var processor in processors)
            {
                if (!processor.IsActive) continue;
                if (processor.TryGetRequiredQuestItem(out var item))
                {
                    if (interactablesToGetItem.TryGetValue(item, out var interactable))
                    {
                        interactable.SetCanInteract(true);
                    }
                }
            }
            
            OnQuestsChange?.Invoke();
        }
    }
}