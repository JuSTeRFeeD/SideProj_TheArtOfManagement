using System.Collections.Generic;
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
        private readonly Queue<DialogueTuple> _availableDialogues = new();
        
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
        
        public void AddAvailableDialogue(DialogueGraph dialogueStateDialogueData, QuestGraphProcessor linkedQuestProcessor)
        {
            Debug.Log($"Added dialogue `{dialogueStateDialogueData.name}` to `{name}`");
            _availableDialogues.Enqueue(new DialogueTuple
            {
                dialogueGraph = dialogueStateDialogueData,
                questGraphProcessor = linkedQuestProcessor
            });
            UpdateAbilityToStartDialogue();
        }

        public void UpdateAbilityToStartDialogue()
        {
            dialogueMarker.SetMarkerActive(HasAvailableDialogues);
        }

        public DialogueTuple GetDialogue()
        {
            if (_availableDialogues.Count == 0) Debug.LogError("[DialogueCompanion] No dialogues available");
            var dialogue = _availableDialogues.Dequeue();
            dialogueMarker.SetMarkerActive(false);
            UpdateAbilityToStartDialogue();
            return dialogue;
        }
    }
}