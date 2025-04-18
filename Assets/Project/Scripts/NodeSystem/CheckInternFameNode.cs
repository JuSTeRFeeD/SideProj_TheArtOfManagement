using System;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace Project.Scripts.NodeSystem
{
    [CreateNodeMenu("Intern/Check Intern Fame Node")]
    [NodeTint("#702b59")]
    [NodeWidth(300)]
    public class CheckInternFameNode : NodeBase
    {
        [Input] 
        [SerializeField] private Node input;

        [Serializable]
        public struct CheckVariable
        {
            [SerializeField] private int fameAmount;
            [SerializeField] private VariableCheckOperation operation;

            public int FameAmount => fameAmount;
            public VariableCheckOperation Operation => operation;
        }
        
        [Output(dynamicPortList = true)]
        [SerializeField] private List<CheckVariable> variants = new();

        public List<CheckVariable> Variants => variants;
    }
}