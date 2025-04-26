using System;
using UnityEngine;

namespace Project.Scripts
{
    public class PlayTime : MonoBehaviour
    {
        private float _elapsedTime;

        public static PlayTime Instance { get; private set; }
        
        public float GetElapsedTime() => _elapsedTime;
        
        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            _elapsedTime += Time.deltaTime;
        }
    }
}