using UnityEngine;
using XNode;

namespace Project.Scripts.NodeSystem.Dialogues.Nodes
{
    [CreateNodeMenu("Dialogue/Dialogue Node")]
    [NodeWidth(300)]
    public class DialogueNode : NodeBase
    {
        [Input]
        [SerializeField] private Node input;
        
        [Output]
        [SerializeField] private Node output;
        
        [SerializeField] private bool speakerIsPlayer = false;
        [TextArea] 
        [SerializeField] private string text;
        
        public string Text => text;
        public bool SpeakerIsPlayer => speakerIsPlayer;

        public override object GetValue(NodePort port)
        {
            return null;
        }
    }
}