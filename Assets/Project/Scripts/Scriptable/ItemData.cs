using UnityEngine;

namespace Project.Scripts.Scriptable
{
    [CreateAssetMenu(menuName = "Game/ItemData")]
    public class ItemData : ScriptableObject
    {
        [SerializeField] private int id;
        [SerializeField] private string title;
        [SerializeField] private string description;
        
        public int Id => id;
        public string Title => title;
        public string Description => description;
    }
}