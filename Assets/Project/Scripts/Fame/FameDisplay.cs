using System;
using TMPro;
using UnityEngine;

namespace Project.Scripts.Fame
{
    public class FameDisplay : MonoBehaviour
    {
        [SerializeField] private bool mainFame = true;
        [SerializeField] private TextMeshProUGUI fameText;
        
        private void Start()
        {
            FameChanged(0, 0);
            if (mainFame) PlayerFame.Instance.mainFame.OnChangeEvent += FameChanged;
            else PlayerFame.Instance.internFame.OnChangeEvent += FameChanged;
        }

        private void FameChanged(int prev, int newVal)
        {
            fameText.SetText(newVal.ToString());
        }

        private void OnDestroy()
        {
            if (mainFame) PlayerFame.Instance.mainFame.OnChangeEvent -= FameChanged;
            else PlayerFame.Instance.internFame.OnChangeEvent -= FameChanged;
        }
    }
}