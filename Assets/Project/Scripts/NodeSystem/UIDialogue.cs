using System;
using System.Collections;
using System.Collections.Generic;
using Project.Scripts.Scriptable;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.NodeSystem
{
    public class UIDialogue : MonoBehaviour
    {
        public static UIDialogue Instance { get; private set; }

        [SerializeField] private Canvas canvas;
        [Space]
        [SerializeField] private EmployerData playerEmployerData;
        [SerializeField] private TextMeshProUGUI speakerNameText;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private Image speakerIcon;
        [SerializeField] private Image playerSpeakerIcon;
        [Space]
        [SerializeField] private Button choiceButtonPrefab;
        [SerializeField] private RectTransform choicesContainer;
        
        private readonly List<Button> _choiceButtons = new();
        private EmployerData _npcEmployerData;

        private void Awake()
        {
            Instance = this;
            canvas.enabled = false;
        }

        public void StartDialogue(EmployerData npcEmployerData)
        {
            _npcEmployerData = npcEmployerData;
            canvas.enabled = true;
            
            playerSpeakerIcon.sprite = playerEmployerData.Icon;
            speakerIcon.sprite = _npcEmployerData.Icon;
        }

        public void ShowDialogue(string text, bool isPlayerSpeaker)
        {
            Debug.Log("Show dialogue: " + text);
            dialogueText.text = text;
            speakerNameText.text = isPlayerSpeaker ? playerEmployerData.EmployerName : _npcEmployerData.EmployerName;

            speakerIcon.enabled = !isPlayerSpeaker;
            playerSpeakerIcon.enabled = isPlayerSpeaker;
        }

        public void ShowChoices(string question, List<string> choices, Action<int> callback)
        {
            speakerIcon.enabled = true;
            playerSpeakerIcon.enabled = false;
            
            dialogueText.text = question;
            
            for (var index = 0; index < choices.Count; index++)
            {
                var choice = choices[index];
                
                var btn = Instantiate(choiceButtonPrefab, choicesContainer);
                btn.GetComponentInChildren<TextMeshProUGUI>().text = choice;
                _choiceButtons.Add(btn);
                
                var choiceIndex = index;
                btn.onClick.AddListener(() =>
                {
                    callback.Invoke(choiceIndex);
                    ClearChoices();
                });
            }

            StartCoroutine(RebuildLayout());
        }
        
        private IEnumerator RebuildLayout()
        {
            yield return new WaitForEndOfFrame();
            LayoutRebuilder.ForceRebuildLayoutImmediate(choicesContainer); 
        }

        private void ClearChoices()
        {
            for (var index = 0; index < _choiceButtons.Count; index++)
            {
                var choiceButton = _choiceButtons[index];
                Destroy(choiceButton.gameObject);
            }
            _choiceButtons.Clear();
        }

        public void EndDialogue()
        {
            ClearChoices();
            canvas.enabled = false;
        }
    }
}