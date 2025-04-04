using System.Collections.Generic;
using Project.Scripts.Scriptable;
using UnityEngine;

namespace Project.Scripts
{
    public class QuestsSystem : MonoBehaviour
    {
        public static QuestsSystem Instance { get; private set; }

        private readonly List<QuestData> _activeQuests = new();
        private readonly List<QuestData> _completedQuests = new();

        public delegate void OnQuestsChanged(List<QuestData> activeQuests);
        public event OnQuestsChanged OnQuestsChangedEvent;
        
        private void Awake()
        {
            Instance = this;
        }

        public void OnStartDialogue(DialogueData dialogueData)
        {
            for (var index = 0; index < _activeQuests.Count; index++)
            {
                var activeQuest = _activeQuests[index];
                if (activeQuest.TargetDialogue == dialogueData)
                {
                    _completedQuests.Add(activeQuest);
                    _activeQuests.Remove(activeQuest);
                }
            }

            OnQuestsChangedEvent?.Invoke(_activeQuests);
        }

        public void StartQuest(QuestData questData)
        {
            _activeQuests.Add(questData);
            OnQuestsChangedEvent?.Invoke(_activeQuests);
        }
    }
}