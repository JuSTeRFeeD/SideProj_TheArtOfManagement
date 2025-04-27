using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Project.Scripts.Fame;
using Project.Scripts.Inventory;
using Project.Scripts.NodeSystem.Dialogues.Nodes;
using Project.Scripts.NodeSystem.Quests.Nodes;
using Project.Scripts.Scriptable;
using UnityEngine;
using XNode;

namespace Project.Scripts.NodeSystem.Quests
{
    public class QuestTuple
    {
        public UnlockDialogueNode byUnlockDialogueNode;
        public QuestData questData;
        public bool isDialogueEnded;
        public bool isItemInInventory;
        public bool isFailed;
    }

    internal struct UnlockedDialogues
    {
        public DialogueGraph dialogueGraph;
        public NpcData npcData;
    }
    
    public class QuestGraphProcessor
    {
        private readonly QuestsManager _questsManager;
        private readonly QuestGraph _questGraph;
        private Node _currentNode;
        
        private readonly List<QuestTuple> _quests = new();
        public event Action OnQuestUpdate;
        public event Action<QuestGraphProcessor> OnQuestComplete;
        public event Action<QuestGraphProcessor> OnTimedQuestUpdate;
        
        public bool IsActive { get; private set; } = false;
        public bool IsQuestEnded { get; private set; } = false;
        public readonly bool isTimedQuest; 

        private float _timer;
        private readonly List<UnlockedDialogues> _unlockedDialogues = new();
        
        private Coroutine _timedQuestCoroutine;

        public QuestGraphProcessor(QuestGraph questGraph, QuestsManager questsManager)
        {
            _questsManager = questsManager;
            _questGraph = questGraph;
            
            isTimedQuest = false;
            foreach (var _ in _questGraph.nodes.FindAll(x => x is QuestStartNode).Select(questStartNode => questStartNode as QuestStartNode).Where(node => node.QuestData.DurationSec > 0))
            {
                isTimedQuest = true;
                break;
            }
        }

        public bool TryGetRequiredQuestItem(out ItemData itemData)
        {
            itemData = default;
            foreach (var quest in _quests)
            {
                if (quest.isDialogueEnded) continue;
                if (!quest.questData.ItemInInventory) continue;
                if (quest.isItemInInventory) continue;
                itemData = quest.questData.ItemInInventory;
            }
            return itemData;
        }
        
        public bool TryGetFailedQuestItem(out ItemData itemData)
        {
            itemData = default;
            foreach (var quest in _quests)
            {
                if (quest.isDialogueEnded) continue;
                if (!quest.questData.ItemInInventory) continue;
                if (quest.isFailed)
                {
                    itemData = quest.questData.ItemInInventory;
                }
            }
            return itemData;
        }

        public string GetQuestDescription()
        {
            if (IsQuestEnded) return string.Empty;
            
            var sb = new StringBuilder();
            for (var index = 0; index < _quests.Count; index++)
            {
                var questTuple = _quests[index];
                if (index == 0)
                {
                    sb.Append("[*] ");
                    sb.Append(questTuple.questData.QuestName);

                    if (questTuple.questData.DurationSec > 0)
                    {
                        var time = TimeSpan.FromSeconds(Mathf.Max(0, _timer));
                        if (_timer < 10f) sb.Append("<color=red>");
                        sb.Append(" (");
                        sb.Append($"{time.Minutes:D2}");
                        sb.Append(":");
                        sb.Append($"{time.Seconds:D2}");
                        sb.Append(")");
                        if (_timer < 10f) sb.Append("</color>");
                    }
                        
                    sb.Append("\n");
                    continue;
                }

                sb.Append("- ");
                var done =
                    (questTuple.isDialogueEnded) || // dialogue ended
                    (questTuple.isItemInInventory && questTuple.questData.ItemInInventory) // item in inventory
                    ;
                var failed = questTuple.isFailed;
                if (failed) sb.Append("<color=red>");
                else if (done) sb.Append("<color=green>");
                sb.Append(questTuple.questData.Description);
                if (done || failed) sb.Append("</color>");
                sb.Append("\n");
            }
            return sb.ToString();
        }

        public void Start(PlayerInventory playerInventory)
        {
            IsActive = true;
            playerInventory.OnInventoryChangeEvent += OnInventoryChangeEvent;
            foreach (var node in _questGraph.nodes.Where(node => node is StartNode))
            {
                _currentNode = node;
                break;
            }
            ProcessNode();
        }

        private void OnInventoryChangeEvent(List<ItemData> item)
        {
            foreach (var questTuple in _quests)
            {
                // Нужен предмет по квесту
                if (questTuple.isItemInInventory) continue;
                if (_currentNode is not QuestStartNode questStartNode) continue;
                if (questStartNode.QuestData != questTuple.questData) continue;
                if (item.Contains(questTuple.questData.ItemInInventory))
                {
                    questTuple.isItemInInventory = true;
                    OnQuestUpdate?.Invoke();
                    SkipQuestRequirements();
                    break;
                }
            }
        }

        private void ProcessNode()
        {
            if (!_currentNode) return;
            
            switch (_currentNode)
            {
                case StartNode startNode:
                    Next(startNode);
                    break;
                case UnlockDialogueNode unlockDialogueNode:
                    ProcessUnlockDialogueNode(unlockDialogueNode);
                    break;
                case FameNode fameNode:
                    ProcessFameNode(fameNode);
                    break;
                case QuestStartNode questStartNode:
                    ProcessQuestStartNode(questStartNode);
                    break;
                case QuestEndNode questEndNode:
                    ProcessQuestEndNode(questEndNode);
                    break;
                case EndTimerQuestNode endTimerQuestNode:
                    ProcessEndTimerQuestNode(endTimerQuestNode);
                    break;
                case TeleportNpcNode teleportNpcNode:
                    ProcessTeleportNpcNode(teleportNpcNode);
                    break;
            }
        }

        private void ProcessTeleportNpcNode(TeleportNpcNode teleportNpcNode)
        {
            if (!_questsManager.dialogueCompanionByNpc.TryGetValue(teleportNpcNode.NpcData, out var dialogueCompanion))
            {
                throw new Exception($"Dialogue companion `{teleportNpcNode.NpcData.name}` not found!");
            }
            dialogueCompanion.transform.position = teleportNpcNode.TpToPosition;
            
            Next(teleportNpcNode);
        }

        private void ProcessQuestEndNode(QuestEndNode questEndNode)
        {
            Debug.Log("Processed quest end node");
            if (_timedQuestCoroutine != null)
            {
                _questsManager.StopCoroutine(_timedQuestCoroutine);
                _timedQuestCoroutine = null;
            }
            _currentNode = null;
            IsQuestEnded = true;
            OnQuestUpdate?.Invoke();
            OnQuestComplete?.Invoke(this);
        }

        private void ProcessQuestStartNode(QuestStartNode questStartNode)
        {
            Debug.Log($"Started quest {questStartNode.QuestData.name} | {questStartNode.QuestData.QuestName}-{questStartNode.QuestData.Description}");
            _quests.Add(new QuestTuple
            {
                questData = questStartNode.QuestData,
            });
            _currentNode = questStartNode;
            OnQuestUpdate?.Invoke();

            // Main quest title
            if (_quests.Count == 1) SkipQuestRequirements();
            // Only talk quest
            else if (questStartNode.QuestData.TargetDialogue && !questStartNode.QuestData.ItemInInventory) SkipQuestRequirements();

            // START TIMED QUEST
            if (questStartNode.QuestData.DurationSec > 0)
            {
                _timedQuestCoroutine = _questsManager.StartCoroutine(TimedQuestCoroutine(questStartNode, questStartNode.QuestData.DurationSec));
            }
        }

        // Timed Quest Coroutine
        private IEnumerator TimedQuestCoroutine(Node questStartNode, float duration)
        {
            Debug.Log($"Started timed quest {duration}");
            _timer = duration;
            while (_timer > 0)
            {
                _timer -= Time.deltaTime;
                OnTimedQuestUpdate?.Invoke(this);
                yield return null;
            }
            
            // Quest failed
            
            // Lock unlocked dialogues
            foreach (var questTuple in _quests)
            {
                questTuple.isFailed = true;
            }
            foreach (var unlockedDialogues in _unlockedDialogues)
            {
                if (_questsManager.dialogueCompanionByNpc.TryGetValue(unlockedDialogues.npcData,
                        out var dialogueCompanion))
                {
                    dialogueCompanion.RemoveAvailableDialogue(unlockedDialogues.dialogueGraph, this);
                }
            }
            
            OnQuestUpdate?.Invoke();
            
            // To fail connection
            var outputPort = questStartNode.GetOutputPort("outputFail");
            if (outputPort.IsConnected)
            {
                _currentNode = outputPort.Connection.node;
                ProcessNode();
            }
        }

        // Timed Quest Completed
        private void ProcessEndTimerQuestNode(EndTimerQuestNode endTimerQuestNode)
        {
            if (_timedQuestCoroutine != null)
            {
                _questsManager.StopCoroutine(_timedQuestCoroutine);
                _timedQuestCoroutine = null;
            }
            Next(endTimerQuestNode);
        }

        private void SkipQuestRequirements()
        {
            var outputPort = _currentNode.GetOutputPort("outputSuccess");
            if (outputPort == null) return;
            if (!outputPort.IsConnected) return;
            _currentNode = outputPort.Connection.node;
            ProcessNode();
        }

        private void ProcessFameNode(FameNode fameNode)
        {
            PlayerFame.Instance.mainFame.Add(fameNode.FameAmount);
            Next(fameNode);
        }

        private void ProcessUnlockDialogueNode(UnlockDialogueNode unlockDialogueNode)
        {
            _currentNode = unlockDialogueNode;
            if (_questsManager.dialogueCompanionByNpc.TryGetValue(unlockDialogueNode.NpcData,
                    out var dialogueCompanion))
            {
                _unlockedDialogues.Add(new UnlockedDialogues
                {
                    dialogueGraph = unlockDialogueNode.DialogueGraph,
                    npcData = unlockDialogueNode.NpcData
                });
                var tuple = new DialogueTuple(unlockDialogueNode.DialogueGraph, this);
                dialogueCompanion.AddAvailableDialogue(tuple);
            }
            else
            {
                Debug.LogError($"Dialogue companion for `{unlockDialogueNode.NpcData.name}` not found!");
            }
        }

        private void Next(Node node)
        {
            var outputPort = node.GetOutputPort("output");
            if (outputPort == null) return;
            if (!outputPort.IsConnected) return;
            _currentNode = outputPort.Connection.node;
            ProcessNode();
        }

        public void HandleFailDialogue(DialogueGraph dialogueGraph)
        {
            if (!_currentNode) return;
            if (_currentNode is not UnlockDialogueNode unlockDialogueNode) return;
            if (unlockDialogueNode.DialogueGraph != dialogueGraph) return;
            
            Debug.Log("Handle Fail Dialogue");
            var outputPort = _currentNode.GetOutputPort("outputFail");
            if (outputPort == null) return;
            if (!outputPort.IsConnected) return;
            _currentNode = outputPort.Connection.node;
            ProcessNode();
        }

        public void HandleSuccessDialogue(DialogueGraph dialogueGraph)
        {
            if (!_currentNode) return;
            if (_currentNode is not UnlockDialogueNode unlockDialogueNode) return;
            if (unlockDialogueNode.DialogueGraph != dialogueGraph) return;
            
            Debug.Log("Handle Success Dialogue");
            var outputPort = _currentNode.GetOutputPort("outputSuccess");
            if (outputPort == null) return;
            if (!outputPort.IsConnected) return;
            Debug.Log("Success dialogue ok");
            _currentNode = outputPort.Connection.node;
            ProcessNode();
        }

        public void OnStartDialogue(DialogueTuple dialogueTuple)
        {
            CheckTimedQuestCompleteOnDialogueEnd();
            
            foreach (var questTuple in _quests)
            {
                if (questTuple.questData.TargetDialogue == dialogueTuple.dialogueGraph)
                {
                    questTuple.isDialogueEnded = true;
                }
            }
            OnQuestUpdate?.Invoke();
        }

        // Check if dialogue end completes timed quest to prevent fail timed quest
        private void CheckTimedQuestCompleteOnDialogueEnd()
        {
            if (_timedQuestCoroutine == null)
            {
                return;
            }
            
            var outputPort = _currentNode.GetOutputPort("outputSuccess");
            if (outputPort is { IsConnected: true })
            {
                if (outputPort.Connection.node is EndTimerQuestNode)
                {
                    _questsManager.StopCoroutine(_timedQuestCoroutine);
                    _timedQuestCoroutine = null;
                }
            }
        }
    }
}