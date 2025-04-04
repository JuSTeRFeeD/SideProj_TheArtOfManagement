using System.Collections.Generic;
using Project.Scripts.Scriptable;
using UnityEngine;

namespace Project.Scripts.Inventory
{
    public class PlayerInventory : MonoBehaviour
    {
        private readonly List<ItemData> _items = new();

        public delegate void HandleInventoryChange(List<ItemData> item);
        public HandleInventoryChange OnInventoryChangeEvent;

        public void AddItem(ItemData item)
        {
            _items.Add(item);
            OnInventoryChangeEvent?.Invoke(_items);
        }

        public bool HasItem(ItemData item)
        {
            return _items.Contains(item);
        }

        public ItemData TakeItem(ItemData item)
        {
            var res = _items.Remove(item) ? item : null;
            OnInventoryChangeEvent?.Invoke(_items);
            return res;
        }
    }
}