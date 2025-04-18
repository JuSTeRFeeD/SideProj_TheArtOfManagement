using UnityEngine;

namespace Project.Scripts.Fame
{
    public class PlayerFame : MonoBehaviour
    {
        public static PlayerFame Instance { get; private set; }

        public readonly Fame mainFame = new();
        public readonly Fame internFame = new();

        private void Awake()
        {
            Instance = this;
            mainFame.Reset();
            internFame.Reset();
        }
    }
}