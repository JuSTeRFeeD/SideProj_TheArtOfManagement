using Project.Scripts.Interactables;
using UnityEngine;

namespace Project.Scripts.Player
{
    public class PlayerInteractController : MonoBehaviour
    {
        [SerializeField] private float interactDistance = 4.0f;
        [SerializeField] private LayerMask interactableMask;
        
        private readonly Collider[] _results = new Collider[64];
        private IInteractable _currentInteractable;

        private float _delay;

        public bool inProgress;

        private void Update()
        {
            Interact();
            FindNearestInteractable();
        }

        private void Interact()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                _currentInteractable?.Interact(this);
            }
        }

        private void FindNearestInteractable()
        {
            _delay -= Time.deltaTime;
            if (_delay > 0) return;
            _delay = 0.1f;
            
            var count = Physics.OverlapSphereNonAlloc(transform.position, interactDistance, _results, interactableMask);
            if (count == 0) return;
            
            var nearestSqrDist = Mathf.Infinity;
            IInteractable nearestInteractable = null;
            for (var index = 0; index < count; index++)
            {
                var result = _results[index];
                if (!result.TryGetComponent(out IInteractable interactable)) continue;
                if (!interactable.CanInteract()) continue;

                var sqrDist = (interactable.Position() - transform.position).sqrMagnitude;
                if (sqrDist < interactDistance && sqrDist < nearestSqrDist)
                {
                    nearestSqrDist = sqrDist; 
                    nearestInteractable = interactable;
                }
            }

            if (nearestInteractable == _currentInteractable) return;

            _currentInteractable?.SetIsNearest(false);
            _currentInteractable = nearestInteractable;
            _currentInteractable?.SetIsNearest(true);
        }
    }
}