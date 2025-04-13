using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Project.Scripts.Fame;
using Project.Scripts.Scriptable;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.NodeSystem.Dialogues
{
    public class UIDialogue : MonoBehaviour
    {
        public static UIDialogue Instance { get; private set; }

        [SerializeField] private Canvas canvas;
        [Space]
        [SerializeField] private NpcData playerNpcData;
        [SerializeField] private TextMeshProUGUI speakerNameText;
        [SerializeField] private TextMeshProUGUI playerSpeakerNameText;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private Image speakerIcon;
        [SerializeField] private Image playerSpeakerIcon;
        
        [Header("Choices")]
        [SerializeField] private CanvasGroup choicesCanvas;
        [SerializeField] private Button choiceButtonPrefab;
        [SerializeField] private RectTransform choicesContainer;

        [Header("Avatars")]
        [SerializeField] private CanvasGroup playerAvatarCanvasGroup;
        [SerializeField] private CanvasGroup npcAvatarCanvasGroup;
        
        [Header("State")]
        [SerializeField] private RectTransform dialogueContainer;
        [SerializeField] private Vector3 dialogueContainerOnChoicesOffset = new(-260f, 0f, 0f);
        private Vector3 _dialogueContainerDefaultPosition;
        
        [Header("Fame")]
        [SerializeField] private Image fameUpImage;
        [SerializeField] private Image fameDownImage;
        
        private readonly List<Button> _choiceButtons = new();
        private NpcData _npcNpcData;

        private void Awake()
        {
            Instance = this;
            canvas.enabled = false;
            
            playerSpeakerIcon.sprite = playerNpcData.Icon;
            playerSpeakerNameText.text = playerNpcData.NpcName;
            
            _dialogueContainerDefaultPosition = dialogueContainer.anchoredPosition;
        }

        private void Start()
        {
            PlayerFame.Instance.FameChangedDirection += OnFameChanged;
        }

        public void StartDialogue(NpcData npcNpcData)
        {
            _npcNpcData = npcNpcData;
            canvas.enabled = true;
            
            speakerIcon.sprite = _npcNpcData.Icon;

            dialogueContainer.DOKill();
            dialogueContainer.anchoredPosition = _dialogueContainerDefaultPosition; 
        }

        public void ShowDialogue(string text, bool isPlayerSpeaker)
        {
            dialogueText.text = text;
            speakerNameText.text = _npcNpcData.NpcName;
            
            playerAvatarCanvasGroup.alpha = isPlayerSpeaker ? 1f : 0f;
            npcAvatarCanvasGroup.alpha = !isPlayerSpeaker ? 1f : 0f;
        }

        public void ShowChoices(string question, List<string> choices, Action<int> callback)
        {
            SetChoicesActive(true);

            choicesCanvas.DOKill();
            choicesCanvas.alpha = 0f;
            choicesCanvas
                .DOFade(1f, 0.3f)
                .SetDelay(0.15f)
                .SetLink(choicesCanvas.gameObject);
            
            playerAvatarCanvasGroup.alpha = 0f;
            npcAvatarCanvasGroup.alpha = 1f;
            speakerNameText.text = _npcNpcData.NpcName;
            
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
                    ClearChoices();
                    callback.Invoke(choiceIndex);
                });
            }

            StartCoroutine(RebuildLayout());
        }

        private void SetChoicesActive(bool isActive)
        {
            dialogueContainer.DOKill();
            if (isActive)
            {
                dialogueContainer
                    .DOAnchorPos(_dialogueContainerDefaultPosition + dialogueContainerOnChoicesOffset, 0.3f)
                    .SetLink(dialogueContainer.gameObject);
            }
            else
            {
                dialogueContainer
                    .DOAnchorPos(_dialogueContainerDefaultPosition, 0.3f)
                    .SetLink(dialogueContainer.gameObject);
            }
        }

        private IEnumerator RebuildLayout()
        {
            yield return new WaitForEndOfFrame();
            LayoutRebuilder.ForceRebuildLayoutImmediate(choicesContainer); 
        }

        private void ClearChoices()
        {
            choicesCanvas
                .DOFade(0f, 0.3f)
                .SetLink(choicesCanvas.gameObject)
                .OnComplete(() =>
                {
                    for (var index = 0; index < _choiceButtons.Count; index++)
                    {
                        var choiceButton = _choiceButtons[index];
                        Destroy(choiceButton.gameObject);
                    }
                    _choiceButtons.Clear(); 
                });
            SetChoicesActive(false);
        }

        public void EndDialogue()
        {
            ClearChoices();
            canvas.enabled = false;

            dialogueContainer.DOKill();
            choicesCanvas.DOKill();
        }

        private void OnFameChanged(bool isup)
        {
            
        }

        private IEnumerator AnimateFame(bool isUp)
        {
            fameDownImage.enabled = !isUp;
            fameUpImage.enabled = isUp;
 
            if (isUp)
            {
                fameUpImage.transform.localScale = Vector3.zero;
                yield return fameUpImage.transform
                    .DOLocalJump(fameUpImage.transform.position, 0.2f, 1, 0.3f)
                    .WaitForCompletion();
            }
            else
            {
                fameDownImage.transform.localScale = Vector3.zero;
                yield return fameDownImage.transform
                    .DOLocalJump(fameUpImage.transform.position, 0.2f, 1, 0.3f)
                    .WaitForCompletion();
            }
        }
    }
}