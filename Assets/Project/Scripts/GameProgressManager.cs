using System.Collections.Generic;
using Project.Scripts.NodeSystem;
using Project.Scripts.NPC;
using Project.Scripts.Scriptable;
using UnityEngine;

namespace Project.Scripts
{
    public class DialogueState
    {
        public readonly DialogueData dialogueData;
        public readonly EmployerData linkedEmployer;
        public bool isAvailable;
        public bool isCompleted;

        public DialogueState(DialogueData data, EmployerData linkedToLinkedEmployer)
        {
            linkedEmployer = linkedToLinkedEmployer;
            dialogueData = data;
            isAvailable = false;
            isCompleted = false;
        }
    }
    
    public class GameProgressManager : MonoBehaviour
    {
        public static GameProgressManager Instance { get; private set; }
        
        private readonly Dictionary<EmployerData, DialogueCompanion> _dialogueCompanions = new();
        private readonly Dictionary<DialogueGraph, DialogueState> _dialogueStateByGraph = new();
        private readonly HashSet<DialogueData> _completedDialogues = new();

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            var employersData = Resources.LoadAll<EmployerData>("Employers");
            foreach (var employerData in employersData)
            {
                foreach (var employerDataDialogue in employerData.Dialogues)
                {
                    _dialogueStateByGraph.Add(employerDataDialogue.Dialogue, new DialogueState(employerDataDialogue, employerData));
                }
            }
            
            var dialogueCompanions = FindObjectsByType<DialogueCompanion>(FindObjectsSortMode.None);
            foreach (var dialogueCompanion in dialogueCompanions)
            {
                _dialogueCompanions.Add(dialogueCompanion.EmployerData, dialogueCompanion);
            }

            RefreshAvailableDialogues();
        }

        private void RefreshAvailableDialogues()
        {
            foreach (var dialogueState in _dialogueStateByGraph.Values)
            {
                if (dialogueState.isCompleted || dialogueState.isAvailable) continue;

                var isAvailable = true;
                foreach (var dialogueData in dialogueState.dialogueData.CompleteDialoguesToUnlock)
                {
                    if (_completedDialogues.Contains(dialogueData)) continue;
                    isAvailable = false;
                    break;
                }

                if (isAvailable)
                {
                    dialogueState.isAvailable = true;
                    
                    if (_dialogueCompanions.ContainsKey(dialogueState.linkedEmployer))
                    {
                        _dialogueCompanions[dialogueState.linkedEmployer].AddAvailableDialogue(dialogueState.dialogueData);
                    }
                    else
                    {
                        Debug.LogError("NPC для диалога не найден!");
                    }
                }
            }
            
            foreach (var dialogueCompanion in _dialogueCompanions.Values)
            {
                dialogueCompanion.UpdateAbilityToStartDialogue();
            }
        }

        public void SetDialogueCompleted(DialogueData dialogueData)
        {
            _dialogueStateByGraph[dialogueData.Dialogue].isCompleted = true;
            _completedDialogues.Add(dialogueData);
            RefreshAvailableDialogues();
        }
    }
}
