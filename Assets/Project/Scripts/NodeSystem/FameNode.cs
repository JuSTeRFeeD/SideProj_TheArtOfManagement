using UnityEngine;
using XNode;

namespace Project.Scripts.NodeSystem
{
    [CreateNodeMenu("Fame Node")]
    [NodeTint("#5b2b70")]
    public class FameNode : NodeBase
    {
        [Input] 
        [SerializeField] private Node input;

        [Output] 
        [SerializeField] private Node output;
        
        [SerializeField] private int fameAmount;

        public int FameAmount => fameAmount;
    }
}