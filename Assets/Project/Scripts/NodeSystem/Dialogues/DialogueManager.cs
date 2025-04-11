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
        [SerializeField] private QuestsManager questsManager;
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
            if (_currentDialogueGraph == null) return;
            if (Input.GetKeyDown(KeyCode.F))
            {
                // Need to click on choice variant to continue
                if (_isWaitingChoiceSelect) return;
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

        private void HandleFameNode(FameNode fameNode)
        {
            PlayerFame.Instance.AddPoints(fameNode.FameAmount);
            NextOrEndDialogue(fameNode);
        }

        private void ShowDialogue(DialogueNode node)
        {
            UIDialogue.Instance.ShowDialogue(node.Text, node.SpeakerIsPlayer);
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