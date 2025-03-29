using System;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace Project.Scripts.NodeSystem.Nodes
{
    [CreateNodeMenu("Dialogue/Choice Node")]
    [NodeWidth(500)]
    public class ChoiceNode : Node
    {
        [Input] 
        [SerializeField] public Node input;

        [TextArea]
        [SerializeField] private string question;
        
        [Space]
        [TextArea]
        [Output(dynamicPortList = true)] 
        [SerializeField] private List<string> choices = new();

        public string Question => question;
        public List<string> Choices => choices;

        public override object GetValue(NodePort port)
        {
            return null;
        }
    }
}