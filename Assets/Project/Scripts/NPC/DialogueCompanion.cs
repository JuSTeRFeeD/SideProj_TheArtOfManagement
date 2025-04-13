using System.Collections.Generic;
using System.Linq;
using Project.Scripts.Interactables;
using Project.Scripts.NodeSystem;
using Project.Scripts.NodeSystem.Dialogues;
using Project.Scripts.NodeSystem.Quests;
using Project.Scripts.Player;
using Project.Scripts.Scriptable;
using UnityEngine;

namespace Project.Scripts.NPC
{
    public class DialogueCompanion : MonoBehaviour, IInteractable
    {
        [SerializeField] private NpcData npcData;
        [SerializeField] private DialogueMarker dialogueMarker;

        public NpcData NpcData => npcData;
        private readonly List<DialogueTuple> _availableDialogues = new();
        
        public bool HasAvailableDialogues => _availableDialogues.Count > 0;
        
        public Vector3 Position() => transform.position;

        public bool CanInteract() => HasAvailableDialogues;
        
        public void Interact(PlayerInteractController playerInteractController)
        {
            if (DialogueManager.Instance.IsDialogueActive) return;
            DialogueManager.Instance.StartDialogue(this);
        }

        public void SetIsNearest(bool value)
        {
            dialogueMarker.SetCanInteract(value);
        }
        
        public void AddAvailableDialogue(DialogueTuple dialogueTuple)
        {
            _availableDialogues.Add(dialogueTuple);
            UpdateAbilityToStartDialogue();
        }
        
        public void RemoveAvailableDialogue(DialogueGraph dialogueGraph, QuestGraphProcessor questGraphProcessor)
        {
            var itemToRemove = _availableDialogues.FirstOrDefault(d => 
                d.dialogueGraph == dialogueGraph && 
                d.questGraphProcessor == questGraphProcessor);
    
            if (itemToRemove != null)
            {
                _availableDialogues.Remove(itemToRemove);
                UpdateAbilityToStartDialogue();
            }
        }

        public void UpdateAbilityToStartDialogue()
        {
            dialogueMarker.SetMarkerActive(HasAvailableDialogues);
        }

        public DialogueTuple GetDialogue()
        {
            if (_availableDialogues.Count == 0) 
                Debug.LogError("[DialogueCompanion] No dialogues available");
    
            var dialogue = _availableDialogues[0];
            _availableDialogues.RemoveAt(0);
    
            dialogueMarker.SetMarkerActive(false);
            UpdateAbilityToStartDialogue();
            return dialogue;
        }
    }
}