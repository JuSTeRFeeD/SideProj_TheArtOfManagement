using UnityEngine;
using XNode;

namespace Project.Scripts.NodeSystem.Quests.Nodes
{
    [CreateNodeMenu("Quest/Quest End Node")]
    [NodeTint("#780212")]
    public class QuestEndNode : NodeBase
    {
        [Input] 
        [SerializeField] private Node input;
    }
}