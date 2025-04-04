using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.Interactables
{
    public class InteractProgress : MonoBehaviour
    {
        [SerializeField] private Image progressBar;

        public void SetProgress(float value)
        {
            progressBar.fillAmount = value;
        }
    }
}