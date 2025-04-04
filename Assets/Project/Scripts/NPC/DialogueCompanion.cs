using System.Collections.Generic;
using Project.Scripts.Interactables;
using Project.Scripts.NodeSystem;
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
        private readonly Queue<DialogueData> _availableDialogues = new();
        
        public bool HasAvailableDialogues => _availableDialogues.Count > 0;
        
        public Vector3 Position() => transform.position;

        public bool CanInteract() => HasAvailableDialogues;
        
        public void Interact(PlayerInteractController playerInteractController)
        {
            DialogueManager.Instance.StartDialogue(this);
        }

        public void SetIsNearest(bool value)
        {
            dialogueMarker.SetCanInteract(value);
        }
        
        public void AddAvailableDialogue(DialogueData dialogueStateDialogueData)
        {
            _availableDialogues.Enqueue(dialogueStateDialogueData);
        }

        public void UpdateAbilityToStartDialogue()
        {
            dialogueMarker.SetMarkerActive(HasAvailableDialogues);
        }

        public DialogueData GetDialogue()
        {
            if (_availableDialogues.Count == 0) Debug.LogError("[DialogueCompanion] No dialogues available");
            var dialogue = _availableDialogues.Dequeue();
            dialogueMarker.SetMarkerActive(false);
            return dialogue;
        }
    }
}