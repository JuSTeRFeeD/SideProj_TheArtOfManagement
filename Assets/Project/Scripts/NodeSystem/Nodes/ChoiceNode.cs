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
        [Input] public Node input;

        [TextArea] public string question;
        
        [Space]
        [TextArea]
        [Output(dynamicPortList = true)] public List<string> choices = new();

        public override object GetValue(NodePort port)
        {
            return null;
        }
    }
}