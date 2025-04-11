using UnityEngine;
using XNode;

namespace Project.Scripts.NodeSystem.Dialogues.Nodes
{
    [CreateNodeMenu("Quest/End Timer Quest")]
    [NodeTint("#0f5970")]
    public class EndTimerQuestNode : NodeBase
    {
        [Input]
        [SerializeField] private Node input;
        
        [Output]
        [SerializeField] private Node output;
    }
}