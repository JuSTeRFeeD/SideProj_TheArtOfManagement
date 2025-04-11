using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Project.Scripts.Fame;
using Project.Scripts.Inventory;
using Project.Scripts.NodeSystem.Quests.Nodes;
using Project.Scripts.Scriptable;
using UnityEngine;
using XNode;

namespace Project.Scripts.NodeSystem.Quests
{
    public class QuestTuple
    {
        public QuestData questData;
        public bool isDialogueEnded;
        public bool isItemInInventory;
    }
    
    public class QuestGraphProcessor
    {
        private readonly QuestsManager _questsManager;
        private readonly QuestGraph _questGraph;
        private Node _currentNode;
        
        private readonly List<QuestTuple> _quests = new();
        public event Action OnQuestUpdate;
        
        public bool IsActive { get; private set; } = false;
        public bool IsQuestEnded { get; private set; } = false;

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
            return itemData != null;
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
                    sb.Append(index + 1);
                    sb.Append(". ");
                    sb.Append(questTuple.questData.QuestName);
                    sb.Append("\n");
                    continue;
                }

                sb.Append("- ");
                sb.Append(questTuple.questData.Description);
                if (
                    (questTuple.isDialogueEnded) || // dialogue ended
                    (questTuple.isItemInInventory && questTuple.questData.ItemInInventory) // item in inventory
                ) {
                    sb.Append(" [+]");
                }
                sb.Append("\n");
            }
            return sb.ToString();
        }

        public QuestGraphProcessor(QuestGraph questGraph, QuestsManager questsManager)
        {
            _questsManager = questsManager;
            _questGraph = questGraph;
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
            if (_currentNode == null) return;
            
            Debug.Log($"Processing node {_currentNode.name}");
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
            }
        }

        private void ProcessQuestEndNode(QuestEndNode questEndNode)
        {
            _currentNode = null;
            IsQuestEnded = true;
            OnQuestUpdate?.Invoke();
        }

        private void ProcessQuestStartNode(QuestStartNode questStartNode)
        {
            Debug.Log($"Started quest {questStartNode.QuestData.name} | {questStartNode.QuestData.QuestName}-{questStartNode.QuestData.Description}");
            _quests.Add(new QuestTuple
            {
                questData = questStartNode.QuestData
            });
            _currentNode = questStartNode;
            OnQuestUpdate?.Invoke();

            // First quest
            if (_quests.Count == 1) SkipQuestRequirements();
            // Only talk quest
            else if (questStartNode.QuestData.TargetDialogue && !questStartNode.QuestData.ItemInInventory) SkipQuestRequirements();
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
            PlayerFame.Instance.AddPoints(fameNode.FameAmount);
            Next(fameNode);
        }

        private void ProcessUnlockDialogueNode(UnlockDialogueNode unlockDialogueNode)
        {
            _currentNode = unlockDialogueNode;
            if (_questsManager.dialogueCompanionByNpc.TryGetValue(unlockDialogueNode.NpcData,
                    out var dialogueCompanion))
            {
                dialogueCompanion.AddAvailableDialogue(unlockDialogueNode.DialogueGraph, this);
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
            if (_currentNode == null) return;
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
            if (_currentNode == null) return;
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

        public void OnStartDialogue(DialogueGraph currentDialogueGraph)
        {
            foreach (var questTuple in _quests)
            {
                if (questTuple.questData.TargetDialogue == currentDialogueGraph)
                {
                    questTuple.isDialogueEnded = true;
                }
            }
            OnQuestUpdate?.Invoke();
        }
    }
}