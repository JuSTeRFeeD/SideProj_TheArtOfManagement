using System.Collections.Generic;
using Project.Scripts.Scriptable;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Scripts.NPC
{
    public class DialogueCompanion : MonoBehaviour
    {
        [SerializeField] private EmployerData employerData;
        [SerializeField] private DialogueMarker dialogueMarker;

        public EmployerData EmployerData => employerData;
        private readonly Queue<DialogueData> _availableDialogues = new();
        
        public bool HasAvailableDialogues => _availableDialogues.Count > 0;
        public DialogueMarker DialogueMarker => dialogueMarker;

        [Button]
        public void TestLogAvailableDialogues()
        {
            Debug.LogError($"{name} has {_availableDialogues.Count} dialogues available: {HasAvailableDialogues}");
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