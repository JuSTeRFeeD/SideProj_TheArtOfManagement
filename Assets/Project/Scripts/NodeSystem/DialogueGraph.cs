using Project.Scripts.NodeSystem.Nodes;
using UnityEngine;
using XNode;

namespace Project.Scripts.NodeSystem
{
    [CreateAssetMenu(menuName = "Game/DialogueGraph")]
    public class DialogueGraph : NodeGraph
    {
        public StartNode startNode;
    }
}