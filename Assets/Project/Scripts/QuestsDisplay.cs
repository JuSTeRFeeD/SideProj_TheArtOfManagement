using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Project.Scripts.Scriptable;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts
{
    public class QuestsDisplay : MonoBehaviour
    {
        [SerializeField] private Image containerBackroundImage;
        [SerializeField] private TextMeshProUGUI textPrefab;
        [SerializeField] private RectTransform container;
        
        private void Start()
        {
            QuestsSystem.Instance.OnQuestsChangedEvent += OnQuestsChange;
            OnQuestsChange(null);
        }

        private void OnQuestsChange(List<QuestData> activeQuests)
        {
            for (var i = 0; i < container.childCount; i++)
            {
                var child = container.GetChild(i);
                Destroy(child.gameObject);
            }

            if (activeQuests != null)
            {
                var sb = new StringBuilder();
                for (var index = 0; index < activeQuests.Count; index++)
                {
                    var activeQuest = activeQuests[index];
                    var text = Instantiate(textPrefab, container);
                    sb.Append(index + 1);
                    sb.Append(". ");
                    sb.Append(activeQuest.QuestName);
                    sb.Append("\n");
                    if (activeQuest.BringItem)
                    {
                        sb.Append("\n");
                        sb.Append("- Принести: ");
                        sb.Append(activeQuest.BringItem.Description);
                    }
                    sb.Append("\n- ");
                    sb.Append(activeQuest.Description);

                    text.SetText(sb.ToString());
                    sb.Clear();
                }

                containerBackroundImage.enabled = activeQuests.Count > 0;
            }
            else
            {
                containerBackroundImage.enabled = false;
            }

            StartCoroutine(RebuildUI());
        }

        private IEnumerator RebuildUI()
        {
            yield return new WaitForEndOfFrame();
            LayoutRebuilder.ForceRebuildLayoutImmediate(container);
        }

        private void OnDestroy()
        {
            QuestsSystem.Instance.OnQuestsChangedEvent -= OnQuestsChange;
        }
    }
}