using UnityEngine;

namespace Project.Scripts.NodeSystem.Dialogues.Nodes
{
    [CreateNodeMenu("ShowQuestTitle")]
    public class ShowQuestTitleNode : NodeBase
    {
        [Input]
        [SerializeField] private NodeBase input;
        
        [Output]
        [SerializeField] private NodeBase output;
        
        [TextArea]
        [SerializeField] private string questTitle;

        public string QuestTitle => questTitle;
    }
}