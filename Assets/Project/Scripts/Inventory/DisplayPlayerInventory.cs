using System.Collections.Generic;
using Project.Scripts.Scriptable;
using UnityEngine;

namespace Project.Scripts.Inventory
{
    public class DisplayPlayerInventory : MonoBehaviour
    {
        [SerializeField] private PlayerInventory playerInventory;
        [SerializeField] private UIItemSlot uiItemSlotPrefab;
        [SerializeField] private RectTransform slotsContainer;

        private readonly List<UIItemSlot> _slots = new();
        
        private void Start()
        {
            playerInventory.OnInventoryChangeEvent += UpdateInventory;
            for (var i = 0; i < 5; i++)
            {
                _slots.Add(Instantiate(uiItemSlotPrefab, slotsContainer));
            }
            UpdateInventory(new List<ItemData>());
        }

        private void UpdateInventory(List<ItemData> items)
        {
            var i = 0;
            for (i = 0; i < items.Count; i++)
            {
                if (i >= _slots.Count) break;
                _slots[i].SetItem(items[i]);
            }

            for (; i < _slots.Count; i++)
            {
                _slots[i].Clear();
            }
        }
    }
}