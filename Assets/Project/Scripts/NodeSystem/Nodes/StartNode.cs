using UnityEngine;
using XNode;

namespace Project.Scripts.NodeSystem.Nodes
{
    [CreateNodeMenu("Start Node")]
    public class StartNode : Node
    {
        [Output] 
        [SerializeField] private Node output;

        public override object GetValue(NodePort port)
        {
            return null;
        }
    }
}