using UnityEngine;
using XNode;

namespace Project.Scripts.NodeSystem
{
    [CreateNodeMenu("Start Node")]
    [NodeTint("#b98aba")]
    public class StartNode : NodeBase
    {
        [Output] 
        [SerializeField] private Node output;

        public override object GetValue(NodePort port)
        {
            return null;
        }
    }
}