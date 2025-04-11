using Project.Scripts.NodeSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Scripts.Scriptable
{
    [CreateAssetMenu(fileName = "QuestData", menuName = "Game/New Quest")]
    public class QuestData : ScriptableObject
    {
        [Header("Main")]
        [SerializeField] private bool isMainQuest = false;
        
        
        [Header("Quest Info")]
        [SerializeField] private string description;
        
        [Tooltip("Zero - infinity")]
        [ShowIf("isMainQuest")]
        [SerializeField] private float durationSec = 0;
        
        [ShowIf("isMainQuest")]
        [SerializeField] private string questName;
        
        [Header("Target")]
        [InlineEditor]
        [ShowIf("IsNotMainQuest")]
        [SerializeField] private ItemData itemInInventory;
        [ShowIf("IsNotMainQuest")]
        [SerializeField] private DialogueGraph targetDialogue;
        
        private bool IsNotMainQuest => !isMainQuest;
        
        public string QuestName => questName;
        public string Description => description;
        public float DurationSec => durationSec;
        public ItemData ItemInInventory => itemInInventory;
        public DialogueGraph TargetDialogue => targetDialogue;
        public bool IsMainQuest => isMainQuest;
    }
}