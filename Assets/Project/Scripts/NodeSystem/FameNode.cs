using Project.Scripts.NodeSystem.Dialogues.Nodes;
using Sirenix.OdinInspector;
using UnityEngine;
using XNode;

namespace Project.Scripts.NodeSystem
{
    [CreateNodeMenu("Fame Node")]
    [NodeTint("#5b2b70")]
    public class FameNode : NodeBase
    {
        [Input] 
        [SerializeField] private Node input;

        [Output] 
        [SerializeField] private Node output;
        
        [SerializeField] private int fameAmount;

        public int FameAmount => fameAmount;

        // Вроде поправил Fame анимацию. Удалить если не понадобится более
        // [ShowIf("IsError")]
        // [LabelText("WARN", SdfIconType.X)]
        // [ReadOnly]
        // [TextArea]
        // public string ERROR = "Fame долен изменяться до диалога!";
        //
        // private bool IsError;
        //
        // private void OnValidate()
        // {
        //     IsError = false;
        //     var outputPort = GetOutputPort("output");
        //     if (outputPort.IsConnected)
        //     {
        //         IsError = outputPort.node is not DialogueNode or ChoiceNode;
        //     }
        // }
    }
}