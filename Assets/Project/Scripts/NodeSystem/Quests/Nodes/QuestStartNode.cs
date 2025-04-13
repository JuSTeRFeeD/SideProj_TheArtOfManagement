using System.Text;
using Project.Scripts.Scriptable;
using Sirenix.OdinInspector;
using UnityEngine;
using XNode;

namespace Project.Scripts.NodeSystem.Quests.Nodes
{
    [CreateNodeMenu("Quest/Start Quest Node")]
    [NodeTint("#735914")]
    [NodeWidth(240)]
    public class QuestStartNode : NodeBase
    {
        [Input] 
        [SerializeField] private Node input;
        
        [Output]
        [SerializeField] private Node outputSuccess;
        
        [Output]
        [ShowIf("FailableQuest")]
        [SerializeField] private Node outputFail;
        
        [SerializeField] private QuestData questData;

        [TextArea]
        [ReadOnly]
        [HideLabel]
        public string questName;
        
        public QuestData QuestData => questData;
        
        private bool FailableQuest => questData != null && questData.DurationSec > 0;

        private void OnValidate()
        {
            if (!FailableQuest)
            {
                outputFail = null;
            }

            if (QuestData)
            {
                var sb = new StringBuilder();
                sb.Append(QuestData.name);
                if (QuestData.IsMainQuest)
                {
                    sb.Append("\n* ");
                    sb.Append(QuestData.QuestName);
                }
                else
                {
                    sb.Append("\n- ");
                    sb.Append(QuestData.Description);
                }
                questName = sb.ToString();
            }
        }
    }
}