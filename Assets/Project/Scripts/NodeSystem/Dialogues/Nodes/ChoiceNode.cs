using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace Project.Scripts.NodeSystem.Dialogues.Nodes
{
    [CreateNodeMenu("Dialogue/Choice Node")]
    [NodeWidth(500)]
    public class ChoiceNode : NodeBase
    {
        [Input] 
        [SerializeField] public Node input;

        [TextArea]
        [SerializeField] private string question;
        
        [Space]
        [TextArea]
        [Output(dynamicPortList = true)] 
        [SerializeField] private List<string> choices = new();

        [SerializeField] private bool cylcledAndRemoveSelected = false;
        
        public string Question => question;
        public List<string> Choices => choices;
        public bool CycledAndRemoveSelected => cylcledAndRemoveSelected;

        public override object GetValue(NodePort port)
        {
            return null;
        }
    }
}