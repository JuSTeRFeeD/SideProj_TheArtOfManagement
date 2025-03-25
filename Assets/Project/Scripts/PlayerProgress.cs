using UnityEngine;

namespace Project.Scripts
{
    public class PlayerProgress : MonoBehaviour
    {
        public int Fame { get; private set; }

        public delegate void OnFameChanged(int newFame);
        public static event OnFameChanged OnFameChangedEvent;

        private void Start()
        {
            Reset();
        }

        private void Reset()
        {
            Fame = 0;
            OnFameChangedEvent?.Invoke(Fame);
        }

        public void AddFame(int amount)
        {
            Fame += amount;
            OnFameChangedEvent?.Invoke(Fame);
        }
        
        public void TakeFame(int amount)
        {
            Fame -= amount;
            if (Fame < 0) Fame = 0;
            OnFameChangedEvent?.Invoke(Fame);
        }
    }
}