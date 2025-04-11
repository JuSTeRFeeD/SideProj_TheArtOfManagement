using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Scripts.Scriptable
{
    [CreateAssetMenu(menuName ="Game/Npc Data")]
    public class NpcData : ScriptableObject
    {
        [SerializeField] private string npcName;
        
        [PreviewField]
        [SerializeField] private Sprite icon;
        
        public string NpcName => npcName;
        public Sprite Icon => icon;
    }
}