using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Scripts.Scriptable
{
    [CreateAssetMenu(fileName = "QuestData", menuName = "Game/New Quest")]
    public class QuestData : ScriptableObject
    {
        [Header("Quest Info")]
        [SerializeField] private string questName;
        [SerializeField] private string description;
        [Tooltip("Zero - infinity")]
        [SerializeField] private float durationSec = 0;
        [Header("Target")]
        [InlineEditor]
        [SerializeField] private DialogueData targetDialogue;
        [InlineEditor]
        [SerializeField] private ItemData bringItem;
        
        public string QuestName => questName;
        public string Description => description;
        public float DurationSec => durationSec;
        public DialogueData TargetDialogue => targetDialogue;
        public ItemData BringItem => bringItem;
    }
}