using UnityEngine;
using XNode;

namespace Project.Scripts.NodeSystem.Nodes
{
    [CreateNodeMenu("Dialogue/Dialogue Node")]
    [NodeWidth(300)]
    public class DialogueNode : Node
    {
        [Input] public Node input;
        [Output] public Node output;
        
        public bool speakerIsPlayer = false;
        [TextArea] public string text;

        public override object GetValue(NodePort port)
        {
            return null;
        }
    }
}