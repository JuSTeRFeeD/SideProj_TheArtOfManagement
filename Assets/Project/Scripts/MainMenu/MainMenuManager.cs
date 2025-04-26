using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Project.Scripts.MainMenu
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private Button startButton;
        [SerializeField] private Button exitButton;

        private void Start()
        {
            exitButton.onClick.AddListener(Exit);
            startButton.onClick.AddListener(ToTheGame);
        }

        private void ToTheGame()
        {
            SceneManager.LoadSceneAsync("GameScene");
        }

        private void Exit()
        {
            Application.Quit();
        }
    }
}