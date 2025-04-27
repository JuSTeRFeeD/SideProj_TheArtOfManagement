using System;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.Interactables;
using Project.Scripts.Inventory;
using Project.Scripts.NPC;
using Project.Scripts.Scriptable;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Project.Scripts.NodeSystem.Quests
{
    public class QuestsManager : MonoBehaviour
    {
        [SerializeField] private bool debugMode = false;
        
        [SerializeField] private PlayerInventory playerInventory;
        
        public readonly Dictionary<ItemData, InteractableGetItem> interactablesToGetItem = new();
        public readonly Dictionary<ItemData, InteractDance> interactableDance = new();
        public readonly Dictionary<NpcData, DialogueCompanion> dialogueCompanionByNpc = new();
        public List<QuestGraphProcessor> processors = new();

        public event Action OnQuestsChange;
        public event Action<QuestGraphProcessor> OnTimedQuestUpdate;

        public static QuestsManager Instance { get; private set; }

        [Header("Quest Settings")]
        [SerializeField] private int maxActiveQuestsCount = 3;
        [SerializeField] private float checkIntervalSeconds = 10f;
        private float _currentInterval = 0f;
        private int _activeQuestsCount = 0;
        private int _timedQuestsCount = 0;
        private List<QuestGraphProcessor> _availableQuests = new();
        
        public event Action AllQuestsCompleted;
        
        private void Awake()
        {
            Instance = this;
#if UNITY_EDITOR
#else
            debugMode = false;
#endif
        }

        private void Start()
        {
            var interactables = FindObjectsByType<InteractableGetItem>(FindObjectsSortMode.None).ToList();
            foreach (var interactable in interactables)
            {
                interactablesToGetItem.Add(interactable.GiveItemAfterInteract, interactable);
            }
            var interactDance = FindObjectsByType<InteractDance>(FindObjectsSortMode.None).ToList();
            foreach (var dance in interactDance)
            {
                interactableDance.Add(dance.GiveItemAfterInteract, dance);
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
#if UNITY_EDITOR
                if (!questGraph.isQuestActive) continue;
#endif
                
                var processor = new QuestGraphProcessor(questGraph, this);
                processor.OnQuestUpdate += HandleProcessorQuestUpdate;
                processor.OnTimedQuestUpdate += HandleProcessorTimedQuestUpdate;
                processors.Add(processor);
                
                _availableQuests.Add(processor);
                processor.OnQuestComplete += OnQuestCompleted;
                
                if (debugMode) processor.Start(playerInventory);
            }
            
            HandleProcessorQuestUpdate();
        }

        private void OnQuestCompleted(QuestGraphProcessor processor)
        {
            _activeQuestsCount--;
            if (processor.isTimedQuest) _timedQuestsCount--;
            processors.Remove(processor);
            
            if (_availableQuests.Count == 0 && _activeQuestsCount <= 0)
                AllQuestsCompleted?.Invoke();
        }

        [Button]
        [InfoBox("Нужно чтобы был взят 1 timed квест чтобы прокнуло")]
        private void FastFinish()
        {
            var buff = _availableQuests[0];
            _availableQuests.Clear();
            _activeQuestsCount = 1;
            OnQuestCompleted(buff);
        }
        
        private void Update()
        {
            if (debugMode) return;
            
            // Instantly unlock 1 quest
            if (_activeQuestsCount == 0)
            {
                UnlockRandomQuest();
                _currentInterval = 0f;
            }
            
            // Interval to unlock quests
            _currentInterval += Time.deltaTime;
            if (_currentInterval < checkIntervalSeconds) return;
            _currentInterval = 0f;
            
            if (_activeQuestsCount < maxActiveQuestsCount)
            {
                UnlockRandomQuest();
            }
        }

        private void UnlockRandomQuest()
        {
            if (_activeQuestsCount >= maxActiveQuestsCount) return;

            while (_availableQuests.Count > 0)
            {
                var index = Random.Range(0, _availableQuests.Count);
                var processor = _availableQuests[index];
                
                // Prevent multiple timed quests
                if (_timedQuestsCount > 0 && processor.isTimedQuest)
                {
                    if (_availableQuests.Count == 1) return;
                    continue;
                }
                
                processor.Start(playerInventory);
                if (processor.isTimedQuest) _timedQuestsCount++;
                _activeQuestsCount++;
                _availableQuests.RemoveAt(index);
                return;
            }
        }

        public QuestGraphProcessor RegisterInternQuest(QuestGraph questGraph)
        {
            var processor = new QuestGraphProcessor(questGraph, this);
            processor.OnQuestUpdate += HandleProcessorQuestUpdate;
            processor.OnTimedQuestUpdate += HandleProcessorTimedQuestUpdate;
            processors.Add(processor);
            return processor;
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
                
                // Required
                if (processor.TryGetRequiredQuestItem(out var item))
                {
                    if (interactablesToGetItem.TryGetValue(item, out var interactable))
                    {
                        interactable.SetCanInteract(true);
                    }
                    if (interactableDance.TryGetValue(item, out var dance))
                    {
                        dance.SetCanInteract(true);
                    }
                }
                
                // Failed
                if (processor.TryGetFailedQuestItem(out var failedItem))
                {
                    if (interactablesToGetItem.TryGetValue(failedItem, out var interactable))
                    {
                        interactable.SetCanInteract(false);
                    }
                    if (interactableDance.TryGetValue(failedItem, out var dance))
                    {
                        dance.SetCanInteract(false);
                    }
                }
            }
            
            OnQuestsChange?.Invoke();
        }
    }
}