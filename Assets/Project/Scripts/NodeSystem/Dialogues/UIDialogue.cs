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

        [Header("Dialogue")]
        [SerializeField] private Canvas canvas;

        [Space] 
        [SerializeField] private float charsPerSecond = 20f;
        [SerializeField] private TextMeshProUGUI skipDialogueAnimText;
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
        [SerializeField] private RectTransform fameUpImage;
        [SerializeField] private RectTransform fameDownImage;
        
        private readonly List<Button> _choiceButtons = new();
        private NpcData _npcNpcData;

        private Coroutine _displayDialogueTextCoroutine;

        private string _fullTargetText;
        private bool _skipDialoguePressed;

        public bool IsAnimationPlaying { get; private set; }

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
            PlayerFame.Instance.mainFame.OnChangeEvent += OnFameChanged;
            
            fameDownImage.gameObject.SetActive(false);
            fameUpImage.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            PlayerFame.Instance.mainFame.OnChangeEvent -= OnFameChanged;
        }

        public void StartDialogue(NpcData npcNpcData)
        {
            _npcNpcData = npcNpcData;
            canvas.enabled = true;

            dialogueContainer.DOKill();
            dialogueContainer.anchoredPosition = _dialogueContainerDefaultPosition;
            _fullTargetText = string.Empty;
        }

        public void ShowDialogue(string text, bool isPlayerSpeaker, NpcData overrideSpeakerNpcData = null)
        {
            if (IsAnimationPlaying) return;
            
            DialogueMoveOnChoicesActive(false);

            if (overrideSpeakerNpcData)
            {
                speakerIcon.sprite = overrideSpeakerNpcData.Icon;
                speakerNameText.text = overrideSpeakerNpcData.NpcName;
            }
            else
            {
                speakerIcon.sprite = _npcNpcData.Icon;
                speakerNameText.text = _npcNpcData.NpcName;
            }
            
            playerAvatarCanvasGroup.alpha = isPlayerSpeaker ? 1f : 0f;
            npcAvatarCanvasGroup.alpha = !isPlayerSpeaker ? 1f : 0f;
            
            IsAnimationPlaying = true;
            _displayDialogueTextCoroutine = StartCoroutine(DisplayDialogueText(text, () =>
            {
                IsAnimationPlaying = false;
            }));
        }

        public void ShowChoices(string question, List<string> choices, Action<int> callback)
        {
            DialogueMoveOnChoicesActive(false);

            choicesCanvas.DOKill();
            choicesCanvas.alpha = 0f;
            
            playerAvatarCanvasGroup.alpha = 0f;
            npcAvatarCanvasGroup.alpha = 1f;
            
            speakerIcon.sprite = _npcNpcData.Icon;
            speakerNameText.text = _npcNpcData.NpcName;

            _displayDialogueTextCoroutine = StartCoroutine(DisplayDialogueText(question, () =>
            {
                for (var index = 0; index < choices.Count; index++)
                {
                    var choice = choices[index];
                
                    var btn = Instantiate(choiceButtonPrefab, choicesContainer);
                    btn.GetComponentInChildren<TextMeshProUGUI>().text = choice;
                    _choiceButtons.Add(btn);
                
                    var choiceIndex = index;
                    btn.onClick.AddListener(() =>
                    {
                        ClearChoices(() =>
                        {
                            callback.Invoke(choiceIndex);
                        });
                    });
                }

                StartCoroutine(RebuildLayout());
                
                StartCoroutine(DisplayChoices());
            }));
        }

        private IEnumerator DisplayChoices()
        {
            skipDialogueAnimText.text = string.Empty;
            yield return new WaitForSeconds(0.15f);
            DialogueMoveOnChoicesActive(true);
            choicesCanvas
                .DOFade(1f, 0.3f)
                .SetDelay(0.15f)
                .SetLink(choicesCanvas.gameObject)
                .OnComplete(() =>
                {
                    IsAnimationPlaying = false;
                });
        }

        private void DialogueMoveOnChoicesActive(bool isActive)
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

        private void ClearChoices(Action onComplete = null)
        {
            foreach (var choiceButton in _choiceButtons)
            {
                choiceButton.interactable = false;
            }
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
                    onComplete?.Invoke();
                });
        }

        public void EndDialogue()
        {
            ClearChoices();
            canvas.enabled = false;
        }

        private void OnFameChanged(int prev, int newVal)
        {
            if (prev == newVal) return;
            StartCoroutine(AnimateFame(prev < newVal));
        }

        private IEnumerator AnimateFame(bool isUp)
        {
            IsAnimationPlaying = true;
            fameDownImage.gameObject.SetActive(!isUp);
            fameUpImage.gameObject.SetActive(isUp);
            fameDownImage.localScale = Vector3.zero;
            fameUpImage.localScale = Vector3.zero;

            const float duration = 0.4f;
            
            if (isUp)
            {
                yield return DOTween.Sequence()
                    .Append(fameUpImage.DOScale(1f, duration))
                    .Append(fameUpImage.DOShakeAnchorPos(0.2f, 0.2f, 1, 1f))
                    .Append(fameUpImage.DOScale(0f, duration))
                    .SetLink(fameUpImage.gameObject)
                    .WaitForCompletion();
            }
            else
            {
                yield return DOTween.Sequence()
                    .Append(fameDownImage.DOScale(1f, duration))
                    .Append(fameDownImage.DOShakeAnchorPos(0.2f, 0.2f, 1, 1f))
                    .Append(fameDownImage.DOScale(0f, duration))
                    .SetLink(fameDownImage.gameObject)
                    .WaitForCompletion();
            }

            IsAnimationPlaying = false;
        }

        private IEnumerator DisplayDialogueText(string text, Action onComplete = null)
        {
            _skipDialoguePressed = false;
            skipDialogueAnimText.text = "Нажмите [Space] чтобы пропустить";
            _fullTargetText = text;
            
            var delay = 1f / charsPerSecond;
            var totalChars = text.Length;
            var currentChar = 0;

            while (currentChar <= totalChars)
            {
                if (_skipDialoguePressed) break;
                
                dialogueText.text = text[..currentChar];
                currentChar++;
                yield return new WaitForSeconds(delay);
            }
            SkipTyping();
            onComplete?.Invoke();
        }

        private void SkipTyping()
        {
            skipDialogueAnimText.text = "Нажмите [Space] для продолжения";
            dialogueText.text = _fullTargetText;
            IsAnimationPlaying = false;
            _skipDialoguePressed = false;
        }

        private void Update()
        {
            if (_displayDialogueTextCoroutine != null)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    _skipDialoguePressed = true;
                }
            }
        }
    }
}