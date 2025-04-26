using System;
using System.Collections.Generic;
using System.Text;
using DG.Tweening;
using Project.Scripts.Fame;
using Project.Scripts.NodeSystem;
using Project.Scripts.NodeSystem.Quests;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Project.Scripts
{
    public class GameFinished : MonoBehaviour
    {
        [SerializeField] private QuestsManager questsManager;
        [Space] 
        [SerializeField] private Canvas gameFinishedCanvas;
        [SerializeField] private CanvasGroup gameFinishedCanvasGroup;
        [SerializeField] private Button menuButton;
        [Header("Text")]
        [SerializeField] private TextMeshProUGUI elapsedTime;
        [Space]
        [SerializeField] private TextMeshProUGUI rightAnswersMain;
        [SerializeField] private TextMeshProUGUI wrongAnswersMain;
        [Space]
        [SerializeField] private TextMeshProUGUI rightAnswersIntern;
        [SerializeField] private TextMeshProUGUI wrongAnswersIntern;

        [Serializable]
        public class ResultText
        {
            [TextArea] public string text;
            [Header("Range")] public int leftBound;
            public VariableCheckOperation leftOperation = VariableCheckOperation.GreaterOrEqual;
            public int rightBound;
            public VariableCheckOperation rightOperation = VariableCheckOperation.Less;
        }

        [Header("Main Result Text")]
        [SerializeField] private TextMeshProUGUI totalMainQuests;
        [SerializeField] private ResultText[] mainFameResultTexts;
        
        [Header("Intern Result Text")]
        [SerializeField] private TextMeshProUGUI totalInternQuests;
        [SerializeField] private ResultText[] internFameResultTexts;

        private void Start()
        {
            questsManager.AllQuestsCompleted += ShowGameFinished;

            gameFinishedCanvas.gameObject.SetActive(true);
            gameFinishedCanvas.enabled = false;
            gameFinishedCanvasGroup.alpha = 0;
            menuButton.interactable = false;
            menuButton.onClick.AddListener(ToMainMenu);
        }

        private void ToMainMenu()
        {
            SceneManager.LoadSceneAsync("MainMenu");
        }

        private void ShowGameFinished()
        {
            CalculateScore();

            gameFinishedCanvas.enabled = true;
            gameFinishedCanvasGroup
                .DOFade(1f, 0.5f)
                .SetLink(gameFinishedCanvasGroup.gameObject)
                .OnComplete(() => { menuButton.interactable = true; });
        }

        private void CalculateScore()
        {
            var totalTimeInSeconds = PlayTime.Instance.GetElapsedTime();
            var minutes = Mathf.FloorToInt(totalTimeInSeconds / 60);
            var seconds = Mathf.FloorToInt(totalTimeInSeconds % 60);
            elapsedTime.text = $"Время прохождения: {minutes:00}:{seconds:00}";
            
            rightAnswersMain.text = $"Правильных ответов: {PlayerFame.Instance.mainFame.RightAnswersCount}";
            wrongAnswersMain.text = $"Неправильных ответов: {PlayerFame.Instance.mainFame.WrongAnswersCount}";
            rightAnswersIntern.text = $"Правильных ответов стажёру: {PlayerFame.Instance.internFame.RightAnswersCount}";
            wrongAnswersIntern.text =
                $"Неправильных ответов стажёру: {PlayerFame.Instance.internFame.WrongAnswersCount}";

            var sb = new StringBuilder();

            var mainFameValue = PlayerFame.Instance.mainFame.Value;
            FindResultText("<b>Общая статистика:</b>\n", mainFameResultTexts, mainFameValue, sb, totalMainQuests);

            var internFameValue = PlayerFame.Instance.internFame.Value;
            FindResultText("<b>Общая статистика по стажёру:</b>\n", internFameResultTexts, internFameValue, sb,
                totalInternQuests);
        }

        private static void FindResultText(string label, IEnumerable<ResultText> results, int mainFameValue,
            StringBuilder sb,
            TMP_Text outputText)
        {
            foreach (var res in results)
            {
                switch (res.leftOperation)
                {
                    case VariableCheckOperation.Equal:
                        if (!(mainFameValue == res.leftBound)) continue;
                        break;
                    case VariableCheckOperation.Less:
                        if (!(mainFameValue < res.leftBound)) continue;
                        break;
                    case VariableCheckOperation.Greater:
                        if (!(mainFameValue > res.leftBound)) continue;
                        break;
                    case VariableCheckOperation.LessOrEqual:
                        if (!(mainFameValue <= res.leftBound)) continue;
                        break;
                    case VariableCheckOperation.GreaterOrEqual:
                        if (!(mainFameValue >= res.leftBound)) continue;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                switch (res.rightOperation)
                {
                    case VariableCheckOperation.Equal:
                        if (!(mainFameValue == res.rightBound)) continue;
                        break;
                    case VariableCheckOperation.Less:
                        if (!(mainFameValue < res.rightBound)) continue;
                        break;
                    case VariableCheckOperation.Greater:
                        if (!(mainFameValue > res.rightBound)) continue;
                        break;
                    case VariableCheckOperation.LessOrEqual:
                        if (!(mainFameValue <= res.rightBound)) continue;
                        break;
                    case VariableCheckOperation.GreaterOrEqual:
                        if (!(mainFameValue >= res.rightBound)) continue;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                sb.Clear();
                sb.Append(label);
                sb.Append(res.text);
                outputText.text = sb.ToString();
                break;
            }
        }
    }
}