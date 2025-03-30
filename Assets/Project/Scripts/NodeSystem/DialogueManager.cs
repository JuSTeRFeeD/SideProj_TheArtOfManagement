using System.Linq;
using Project.Scripts.NodeSystem.Nodes;
using Project.Scripts.NPC;
using Project.Scripts.Scriptable;
using UnityEngine;
using XNode;

namespace Project.Scripts.NodeSystem
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }

        private DialogueData _activeDialogue;
        private DialogueGraph _currentDialogueGraph;
        private Node _currentNode;
        private bool _isWaitingChoiceSelect;
        
        public bool IsDialogueActive => _activeDialogue != null;

        private void Awake()
        {
            Instance = this;
        }

        public void StartDialogue(DialogueCompanion dialogueCompanion)
        {
            _activeDialogue = dialogueCompanion.GetDialogue();
            if (!_activeDialogue) return;

            _currentDialogueGraph = _activeDialogue.Dialogue;
            
            foreach (var node in _currentDialogueGraph.nodes)
            {
                if (node is StartNode)
                {
                    _currentNode = node;
                    break;
                }
            }
            UIDialogue.Instance.StartDialogue(dialogueCompanion.EmployerData);
            ProcessNode();
        }

        private void Update()
        {
            if (_activeDialogue == null) return;
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
                case DialogueNode dialogueNode:
                    ShowDialogue(dialogueNode);
                    break;
                case ChoiceNode choiceNode:
                    ShowChoices(choiceNode);
                    break;
                case QuestStartNode questNode:
                    StartQuest(questNode);
                    break;
                case StartNode:
                    var port = _currentNode.GetOutputPort("output");
                    if (port.IsConnected)
                    {
                        _currentNode = port.Connection.node;
                        ProcessNode();
                    }
                    else EndDialogue();
                    break;
                case FameNode fameNode:
                    HandleFameNode(fameNode);
                    break;
                default:
                    Debug.Log("process unknown node");
                    break;
            }
        }

        private void HandleFameNode(FameNode fameNode)
        {
            // TODO: change fame
            var port = _currentNode.GetOutputPort("output");
            if (port.IsConnected)
            {
                _currentNode = port.Connection.node;
                ProcessNode();
            }
            else EndDialogue();
        }

        private void ShowDialogue(DialogueNode node)
        {
            UIDialogue.Instance.ShowDialogue(node.text, node.speakerIsPlayer);
            var port = node.GetOutputPort("output");
            if (port.IsConnected)
            {
                _currentNode = port.Connection.node;
            }
            else
            {
                _currentNode = null;
            }
        }

        private void ShowChoices(ChoiceNode node)
        {
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

        private void StartQuest(QuestStartNode node)
        {
            // QuestManager.Instance.StartQuest(node.quest);
            var port = node.GetOutputPort("output");
            if (port.IsConnected)
            {
                _currentNode = port.Connection.node;
                ProcessNode();
            }
            else
            {
                _currentNode = null;
            }
        }

        private void EndDialogue()
        {
            if (!_activeDialogue) return;
            
            GameProgressManager.Instance.SetDialogueCompleted(_activeDialogue);
            _activeDialogue = null;
            _currentDialogueGraph = null;
            _currentNode = null;
            UIDialogue.Instance.EndDialogue();
        }
    }
}