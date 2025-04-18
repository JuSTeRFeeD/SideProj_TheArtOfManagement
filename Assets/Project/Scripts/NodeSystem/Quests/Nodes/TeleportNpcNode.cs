using Project.Scripts.Scriptable;
using UnityEngine;
using XNode;

namespace Project.Scripts.NodeSystem.Quests.Nodes
{
    [CreateNodeMenu("Quest/TeleportNpc")]
    [NodeWidth(300)]
    public class TeleportNpcNode : NodeBase
    {
        [Input]
        [SerializeField] private Node input;
        
        [Output]
        [SerializeField] private Node output;

        [SerializeField] private NpcData npcData;
        [SerializeField] private Vector3 tpToPosition;
        
        public NpcData NpcData => npcData;
        public Vector3 TpToPosition => tpToPosition;
    }
}