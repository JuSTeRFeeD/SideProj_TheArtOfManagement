using Project.Scripts.Scriptable;
using UnityEngine;
using XNode;

namespace Project.Scripts.NodeSystem
{
    [CreateNodeMenu("Items/Give")]
    public class GiveItemNode : NodeBase
    {
        [Input]
        [SerializeField] private Node input;
        
        [Output]
        [SerializeField] private Node output;
        
        [SerializeField] private ItemData item;
        public ItemData Item => item;
    }
}