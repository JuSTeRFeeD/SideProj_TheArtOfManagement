using System;
using TMPro;
using UnityEngine;

namespace Project.Scripts.Fame
{
    public class FameDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI fameText;
        
        private void Start()
        {
            FameChanged(0);
            PlayerFame.Instance.FameChanged += FameChanged;
        }

        private void FameChanged(int points)
        {
            fameText.SetText(points.ToString());
        }

        private void OnDestroy()
        {
            PlayerFame.Instance.FameChanged -= FameChanged;
        }
    }
}