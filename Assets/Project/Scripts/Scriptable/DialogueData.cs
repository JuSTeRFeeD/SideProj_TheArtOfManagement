using Project.Scripts.NodeSystem;
using UnityEngine;

namespace Project.Scripts.Scriptable
{
    [CreateAssetMenu(menuName = "Game/Dialogue Data")]
    public class DialogueData : ScriptableObject
    {
        [SerializeField] private DialogueGraph dialogue;
        [SerializeField] private DialogueData[] completeDialoguesToUnlock;
        [SerializeField] private ItemData[] bringItemsToUnlockDialogue;
        
        public DialogueGraph Dialogue => dialogue;
        public DialogueData[] CompleteDialoguesToUnlock => completeDialoguesToUnlock;
        public ItemData[] BringItemsToUnlockDialogue => bringItemsToUnlockDialogue;
    }
}