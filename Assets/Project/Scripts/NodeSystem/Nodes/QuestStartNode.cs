using UnityEngine;
using XNode;

namespace Project.Scripts.NodeSystem.Nodes
{
    [CreateNodeMenu("Quest/Start Quest Node")]
    public class QuestStartNode : Node
    {
        [Input] 
        [SerializeField] private Node input;
        [Output] 
        [SerializeField] private Node output;
        
        // public Quest quest;
    }
}