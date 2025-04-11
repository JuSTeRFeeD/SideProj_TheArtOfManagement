using System.Collections;
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
        
        private void Start()
        {
            questsManager.OnQuestsChange += OnQuestsChange;
        }

        private void OnQuestsChange()
        {
            for (var i = 0; i < container.childCount; i++)
            {
                Destroy(container.GetChild(i).gameObject);
            }
            
            var displayActive = false;
            foreach (var questsManagerProcessor in questsManager.processors)
            {
                var desc = questsManagerProcessor.GetQuestDescription();
                if (string.IsNullOrEmpty(desc)) continue;
                
                Instantiate(textPrefab, container).SetText(desc);
                displayActive = true;
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
        }
    }
}