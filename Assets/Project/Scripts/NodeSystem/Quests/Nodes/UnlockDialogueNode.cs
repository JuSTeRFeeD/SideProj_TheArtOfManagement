using System;
using System.Linq;
using Project.Scripts.NodeSystem.Dialogues.Nodes;
using Project.Scripts.Scriptable;
using Sirenix.OdinInspector;
using UnityEngine;
using XNode;

namespace Project.Scripts.NodeSystem.Quests.Nodes
{
    [CreateNodeMenu("Quest/UnlockDialogue")]
    [NodeWidth(300)]
    public class UnlockDialogueNode : NodeBase
    {
        [Input]
        [SerializeField] private Node input;
        
        [Output]
        [ShowIf("DialogueHasSuccessNode")]
        [SerializeField] private Node outputSuccess;
        
        [Output]
        [ShowIf("DialogueHasFailNode")]
        [SerializeField] private Node outputFail;

        [SerializeField] private DialogueGraph dialogueGraph;
        [SerializeField] private NpcData npcData;

        public DialogueGraph DialogueGraph => dialogueGraph;
        public NpcData NpcData => npcData;
        
        private bool DialogueHasFailNode => dialogueGraph && dialogueGraph.nodes.Any(i => i is FailNode);
        private bool DialogueHasSuccessNode => dialogueGraph && dialogueGraph.nodes.Any(i => i is SuccessNode);
        
        private void OnValidate()
        {
            if (!DialogueHasFailNode) outputFail = null;
            if (!DialogueHasSuccessNode) outputSuccess = null;
        }
    }
}