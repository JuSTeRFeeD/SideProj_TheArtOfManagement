using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Scripts.Scriptable
{
    [CreateAssetMenu(menuName = "Game/ItemData")]
    public class ItemData : ScriptableObject
    {
        [SerializeField] private bool displayItemInInventory = true;
        [SerializeField] private string title;
        [SerializeField] private string description;
        [PreviewField]
        [SerializeField] private Sprite icon;
        
        public string Title => title;
        public string Description => description;
        public Sprite Icon => icon;
        public bool DisplayItemInInventory => displayItemInInventory;
    }
}