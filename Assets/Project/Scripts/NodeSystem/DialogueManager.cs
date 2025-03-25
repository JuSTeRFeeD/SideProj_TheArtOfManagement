using System.Linq;
using Project.Scripts.NodeSystem.Nodes;
using Project.Scripts.NPC;
using UnityEngine;
using XNode;

namespace Project.Scripts.NodeSystem
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }
        
        [SerializeField] private DialogueGraph currentDialogueGraph;
        private Node _currentNode;
        private bool _isWaitingChoiceSelect;

        private void Awake()
        {
            Instance = this;
        }

        public void StartDialogue(DialogueCompanion dialogueCompanion)
        {
            Debug.Log("Start Dialogue");
            currentDialogueGraph = dialogueCompanion.Dialogue;
            foreach (var node in currentDialogueGraph.nodes)
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
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // Need to click on choice variant to continue
                if (_isWaitingChoiceSelect) return; 
                ProcessNode();
            }
        }

        private void ProcessNode()
        {
            Debug.Log("process node");
            switch (_currentNode)
            {
                case DialogueNode dialogueNode:
                    Debug.Log("process node as dialogue");
                    ShowDialogue(dialogueNode);
                    break;
                case ChoiceNode choiceNode:
                    Debug.Log("process node as choice");
                    ShowChoices(choiceNode);
                    break;
                case QuestStartNode questNode:
                    Debug.Log("process node as quest");
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
                default:
                    Debug.Log("process unknown node");
                    break;
            }
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
                EndDialogue();
            }
        }

        private void ShowChoices(ChoiceNode node)
        {
            _isWaitingChoiceSelect = true;
            var choiceTexts = node.choices
                .Select(choice => choice)
                .ToList();
            UIDialogue.Instance.ShowChoices(node.question, choiceTexts, callbackIndex => 
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
                EndDialogue();
            }
        }

        private void EndDialogue()
        {
            currentDialogueGraph = null;
            _currentNode = null;
            UIDialogue.Instance.EndDialogue();
        }
    }
}