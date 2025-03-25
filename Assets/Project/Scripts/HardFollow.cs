using UnityEngine;

namespace Project.Scripts
{
    public class HardFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
        
        private void Update()
        {
            if (!target) return;
            transform.position = target.position;
        }
    }
}