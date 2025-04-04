using Project.Scripts.NodeSystem;
using UnityEditor;
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
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (dialogue == null)
            {
                dialogue = CreateInstance<DialogueGraph>();
                dialogue.name = "DialogueGraph";
            
                // Сохраняем как вложенный объект
                AssetDatabase.AddObjectToAsset(dialogue, this);
                AssetDatabase.SaveAssets();
            }
        }
#endif
    }
}