using Project.Scripts.Scriptable;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.Inventory
{
    public class UIItemSlot : MonoBehaviour
    {
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image iconImage;

        public void SetItem(ItemData item)
        {
            backgroundImage.enabled = true;
            iconImage.enabled = true;
            iconImage.sprite = item.Icon;
        }

        public void Clear()
        {
            backgroundImage.enabled = false;
            iconImage.enabled = false;
        }
    }
}