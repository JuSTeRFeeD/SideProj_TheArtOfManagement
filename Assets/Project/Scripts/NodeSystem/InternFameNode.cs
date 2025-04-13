using UnityEngine;
using XNode;

namespace Project.Scripts.NodeSystem
{
    [CreateNodeMenu("Intern/Intern Fame Node")]
    [NodeTint("#702b59")]
    public class InternFameNode : NodeBase
    {
        [Input] 
        [SerializeField] private Node input;

        [Output] 
        [SerializeField] private Node output;
        
        [SerializeField] private int fameAmount;

        public int FameAmount => fameAmount;
    }
}