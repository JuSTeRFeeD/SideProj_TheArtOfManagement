using System.Collections;
using System.Collections.Generic;
using Project.Scripts.NodeSystem.Quests;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts
{
    public class QuestsDisplay : MonoBehaviour
    {
        [SerializeField] private QuestsManager questsManager;
        [Space]
        [SerializeField] private Image containerBackroundImage;
        [SerializeField] private TextMeshProUGUI textPrefab;
        [SerializeField] private RectTransform container;

        private readonly Dictionary<QuestGraphProcessor, TextMeshProUGUI> _processorsTexts = new();
        
        private void Start()
        {
            questsManager.OnQuestsChange += OnQuestsChange;
            questsManager.OnTimedQuestUpdate += OnTimedQuestUpdate;
        }

        private void OnTimedQuestUpdate(QuestGraphProcessor processor)
        {
            if (_processorsTexts.TryGetValue(processor, out var txt))
            {
                txt.SetText(processor.GetQuestDescription());
            }
        }

        private void OnQuestsChange()
        {
            foreach (var keyValue in _processorsTexts)
            {
                Destroy(keyValue.Value.gameObject);
            }
            _processorsTexts.Clear();
            
            var displayActive = false;
            foreach (var questsManagerProcessor in questsManager.processors)
            {
                var desc = questsManagerProcessor.GetQuestDescription();
                if (string.IsNullOrEmpty(desc)) continue;

                var txt = Instantiate(textPrefab, container);
                txt.SetText(desc);
                displayActive = true;
                _processorsTexts.Add(questsManagerProcessor, txt);
            }
            
            containerBackroundImage.enabled = displayActive;

            StartCoroutine(RebuildUI());
        }

        private IEnumerator RebuildUI()
        {
            yield return new WaitForEndOfFrame();
            LayoutRebuilder.ForceRebuildLayoutImmediate(container);
        }

        private void OnDestroy()
        {
            questsManager.OnQuestsChange -= OnQuestsChange;
            questsManager.OnTimedQuestUpdate -= OnTimedQuestUpdate;
        }
    }
}