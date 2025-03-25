using UnityEngine;

namespace Project.Scripts
{
    public class QuestManager : MonoBehaviour
    {
        public static QuestManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }
    }
}