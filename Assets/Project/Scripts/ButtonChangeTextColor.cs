using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Project.Scripts
{
    public class ButtonChangeTextColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Color hoverColor = Color.white;
        [SerializeField] private Color normalColor = Color.black;

        private TextMeshProUGUI _textMesh;

        private void Start()
        {
            _textMesh = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _textMesh.color = hoverColor;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _textMesh.color = normalColor;
        }
    }
}