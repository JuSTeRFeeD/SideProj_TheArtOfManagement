using Project.Scripts.Scriptable;
using Sirenix.OdinInspector;
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
        
        [HideIf("speakerIsPlayer")]
        [SerializeField] private NpcData overrideNpcSpeaker;
        
        public string Text => text;
        public bool SpeakerIsPlayer => speakerIsPlayer;
        public NpcData OverrideNpcSpeaker => overrideNpcSpeaker;

        public override object GetValue(NodePort port)
        {
            return null;
        }
    }
}