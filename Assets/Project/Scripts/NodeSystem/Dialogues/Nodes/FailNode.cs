using UnityEngine;
using XNode;

namespace Project.Scripts.NodeSystem.Dialogues.Nodes
{
    [CreateNodeMenu("Dialogue/Fail")]
    [NodeTint("#cc4d1b")]
    public class FailNode : NodeBase
    {
        [Input]
        [SerializeField] private Node input;
    }
}