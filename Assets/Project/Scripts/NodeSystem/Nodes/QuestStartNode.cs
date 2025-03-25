using XNode;

namespace Project.Scripts.NodeSystem.Nodes
{
    [CreateNodeMenu("Quest/Start Quest Node")]
    public class QuestStartNode : Node
    {
        [Input] public Node input;
        [Output] public Node output;
        // public Quest quest;
    }
}