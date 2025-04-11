using UnityEngine;

namespace Project.Scripts.Fame
{
    public class PlayerFame : MonoBehaviour
    {
        public int CurrentPoints { get; set; }

        public delegate void FameChangedHandler(int points);
        public event FameChangedHandler FameChanged;
        
        public static PlayerFame Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void AddPoints(int points)
        {
            Debug.Log(points);
            CurrentPoints += points;
            FameChanged?.Invoke(CurrentPoints);
        }
    }
}