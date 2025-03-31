using UnityEngine;
using XNode;

namespace Project.Scripts.NodeSystem.Nodes
{
    [CreateNodeMenu("Fame Node")]
    public class FameNode : Node
    {
        [Input] 
        [SerializeField] private Node input;

        [Output] 
        [SerializeField] private Node output;
        
        [SerializeField] private int fameAmount;

        public int FameAmount => fameAmount;
    }
}