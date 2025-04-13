using UnityEngine;
using XNode;

namespace Project.Scripts.NodeSystem.Quests
{
    [CreateAssetMenu(menuName = "Game/New Quest Graph")]
    public class QuestGraph : NodeGraph
    {
        public bool isQuestActive = true;
    }
}