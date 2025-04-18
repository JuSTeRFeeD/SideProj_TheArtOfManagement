using System;
using System.Collections;
using System.Linq;
using Project.Scripts.Fame;
using Project.Scripts.Inventory;
using Project.Scripts.NodeSystem.Dialogues.Nodes;
using Project.Scripts.NodeSystem.Quests;
using Project.Scripts.NPC;
using UnityEngine;
using XNode;

namespace Project.Scripts.NodeSystem.Dialogues
{
    public class DialogueManager : MonoBehaviour
    {
        [SerializeField] private PlayerInventory playerInventory;
        
        public static DialogueManager Instance { get; private set; }

        private DialogueTuple _dialogueTuple;
        private DialogueGraph _currentDialogueGraph;
        private Node _currentNode;
        private bool _isWaitingChoiceSelect;
        
        public bool IsDialogueActive => _currentDialogueGraph != null;

        private void Awake()
        {
            Instance = this;
        }

        public void StartDialogue(DialogueCompanion dialogueCompanion)
        {
            _dialogueTuple = dialogueCompanion.GetDialogue();
            if (_dialogueTuple == null) return;
            
            _currentDialogueGraph = _dialogueTuple.dialogueGraph;
            if (!_currentDialogueGraph) return;

            Debug.Log($"Start dialogue {_currentDialogueGraph.name}");
            
            foreach (var node in _currentDialogueGraph.nodes.Where(node => node is StartNode))
            {
                _currentNode = node;
                break;
            }
#if UNITY_EDITOR
            if (!_currentNode)
            {
                Debug.LogError($"Dialogue graph: {_currentDialogueGraph.name} doesn't have a StartNode!");
            }
#endif
            _dialogueTuple.questGraphProcessor.OnStartDialogue(_dialogueTuple);
            UIDialogue.Instance.StartDialogue(dialogueCompanion.NpcData);
            ProcessNode();
        }

        private void Update()
        {
            if (!_currentDialogueGraph) return;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // Need to click on choice variant to continue
                if (UIDialogue.Instance.IsAnimationPlaying || _isWaitingChoiceSelect) return;
                ProcessNode();
            }
        }

        private void ProcessNode()
        {
            if (!_currentNode) EndDialogue();

            switch (_currentNode)
            {
                case StartNode startNode:
                    NextOrEndDialogue(startNode);
                    break;
                case DialogueNode dialogueNode:
                    ShowDialogue(dialogueNode);
                    break;
                case ChoiceNode choiceNode:
                    ShowChoices(choiceNode);
                    break;
                case TakeItemNode takeItemNode:
                    HandleTakeItem(takeItemNode);
                    break;
                case GiveItemNode giveItemNode:
                    HandleGiveItem(giveItemNode);
                    break;
                case FameNode fameNode:
                    HandleFameNode(fameNode);
                    break;
                case InternFameNode internFameNode:
                    HandleInternFameNode(internFameNode);
                    break;
                case CheckInternFameNode checkInternFameNode:
                    HandleCheckInternFameNode(checkInternFameNode);
                    break;
                case SuccessNode successNode:
                    _dialogueTuple.questGraphProcessor.HandleSuccessDialogue(_currentDialogueGraph);
                    NextOrEndDialogue(successNode);
                    break;
                case FailNode failNode:
                    _dialogueTuple.questGraphProcessor.HandleFailDialogue(_currentDialogueGraph);
                    NextOrEndDialogue(failNode);
                    break;
                default:
                    Debug.Log("process unknown node");
                    break;
            }
        }

        private void HandleGiveItem(GiveItemNode giveItemNode)
        {
            playerInventory.AddItem(giveItemNode.Item);
            NextOrEndDialogue(giveItemNode);
        }

        private void HandleTakeItem(TakeItemNode takeItemNode)
        {
            playerInventory.TakeItem(takeItemNode.Item);
            NextOrEndDialogue(takeItemNode);
        }

        private void HandleCheckInternFameNode(CheckInternFameNode checkInternFameNode)
        {
            var internFame = PlayerFame.Instance.internFame.Value;
            for (var index = 0; index < checkInternFameNode.Variants.Count; index++)
            {
                var checkVariable = checkInternFameNode.Variants[index];
                // как сделать переход на один из вариантов?
                var conditionMet = checkVariable.Operation switch
                {
                    VariableCheckOperation.Equal => internFame == checkVariable.FameAmount,
                    VariableCheckOperation.Less => internFame < checkVariable.FameAmount,
                    VariableCheckOperation.Greater => internFame > checkVariable.FameAmount,
                    VariableCheckOperation.LessOrEqual => internFame <= checkVariable.FameAmount,
                    VariableCheckOperation.GreaterOrEqual => internFame >= checkVariable.FameAmount,
                    _ => throw new ArgumentOutOfRangeException()
                };
                
                if (!conditionMet) continue;
                var port = checkInternFameNode.GetOutputPort($"variants {index}");
                if (port.ConnectionCount <= 0) continue;
                var connection = port.Connection;
                _currentNode = connection.node;
                ProcessNode();
                return;
            }

            Debug.LogError("Not found any variants of intern fame condition check!");
        }

        private void HandleInternFameNode(InternFameNode internFameNode)
        {
            PlayerFame.Instance.internFame.Add(internFameNode.FameAmount);
            StartCoroutine(WaitDialogueAnimationToNext(internFameNode));
        }

        private void HandleFameNode(FameNode fameNode)
        {
            PlayerFame.Instance.mainFame.Add(fameNode.FameAmount);
            StartCoroutine(WaitDialogueAnimationToNext(fameNode));
        }

        private IEnumerator WaitDialogueAnimationToNext(Node node)
        {
            while (UIDialogue.Instance.IsAnimationPlaying)
            {
                yield return null;
            }
            NextOrEndDialogue(node);
        }

        private void ShowDialogue(DialogueNode node)
        {
            UIDialogue.Instance.ShowDialogue(node.Text, node.SpeakerIsPlayer, node.OverrideNpcSpeaker);
            var port = node.GetOutputPort("output");
            _currentNode = port.IsConnected ? port.Connection.node : null;
        }

        private void ShowChoices(ChoiceNode node)
        {
            Debug.Log("Show choices");
            _isWaitingChoiceSelect = true;
            var choiceTexts = node.Choices
                .Select(choice => choice)
                .ToList();
            UIDialogue.Instance.ShowChoices(node.Question, choiceTexts, callbackIndex => 
            {
                var port = node.GetPort($"choices {callbackIndex}");
                if (!port.IsConnected) return;
                _currentNode = port.Connection.node;
                _isWaitingChoiceSelect = false;
                ProcessNode();
            });
        }

        private void NextOrEndDialogue(Node node)
        {
            var port = node.GetOutputPort("output");
            if (port is { IsConnected: true })
            {
                _currentNode = port.Connection.node;
                ProcessNode();
            }
            else EndDialogue();
        }

        private void EndDialogue()
        {
            if (!_currentDialogueGraph) return;
            
            _currentDialogueGraph = null;
            _currentNode = null;
            UIDialogue.Instance.EndDialogue();
        }
    }
}