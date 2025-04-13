using System;
using UnityEngine;

namespace Project.Scripts.Fame
{
    public class PlayerFame : MonoBehaviour
    {
        public int CurrentPoints { get; set; }

        public delegate void FameChangedHandler(int points);
        public event FameChangedHandler FameChanged;

        public delegate void FameChangedDirectionHandler(bool isUp);
        public event FameChangedDirectionHandler FameChangedDirection;
        
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
            var previousPoints = CurrentPoints;
            CurrentPoints += points;
            FameChangedDirection?.Invoke(CurrentPoints > previousPoints);
            FameChanged?.Invoke(CurrentPoints);
        }
    }
}