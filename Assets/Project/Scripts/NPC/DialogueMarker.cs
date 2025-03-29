using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.NPC
{
    public class DialogueMarker : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private Image icon;
        [SerializeField] private Sprite baseSprite;
        [SerializeField] private Sprite canTalkSprite;

        private void Awake()
        {
            SetMarkerActive(false);
            SetCanTalk(false);
        }

        public void SetMarkerActive(bool isActive)
        {
            canvas.enabled = isActive;
        }
        
        public void SetCanTalk(bool value)
        {
            icon.sprite = value ? canTalkSprite : baseSprite;
        }
    }
}