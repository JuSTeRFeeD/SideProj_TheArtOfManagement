using UnityEngine;
using XNode;

namespace Project.Scripts.NodeSystem.Dialogues.Nodes
{
    [CreateNodeMenu("Dialogue/Success")]
    [NodeTint("#2acc1b")]
    public class SuccessNode : NodeBase
    {
        [Input]
        [SerializeField] private Node input;
    }
}