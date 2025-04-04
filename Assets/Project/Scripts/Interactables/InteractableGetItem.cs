using System.Collections;
using Project.Scripts.Inventory;
using Project.Scripts.NPC;
using Project.Scripts.Player;
using Project.Scripts.Scriptable;
using UnityEngine;

namespace Project.Scripts.Interactables
{
    public class InteractableGetItem : MonoBehaviour, IInteractable
    {
        [SerializeField] private ItemData giveItemOnInteract;
        [SerializeField] private float interactTime = 1.5f;
        [Space]
        [SerializeField] private InteractProgress interactProgress;
        [SerializeField] private DialogueMarker dialogueMarker;

        private bool _isCanInteract = true;

        public Vector3 Position() => transform.position;
        public bool CanInteract() => _isCanInteract;

        private void Start()
        {
            interactProgress.gameObject.SetActive(false);
            SetIsNearest(false);
        }

        public void Interact(PlayerInteractController playerInteractController)
        {
            if (!playerInteractController.gameObject.TryGetComponent(out PlayerInventory playerInventory))
            {
                return;
            }
            StartCoroutine(InteractProgress(playerInventory));
        }

        public void SetIsNearest(bool value)
        {
            dialogueMarker.SetMarkerActive(value);
            dialogueMarker.SetCanInteract(value);
        }

        public IEnumerator InteractProgress(PlayerInventory playerInventory)
        {
            interactProgress.gameObject.SetActive(true);
            dialogueMarker.SetMarkerActive(false);
            _isCanInteract = false;
            
            var elapsed = 0f;
            while (elapsed < interactTime)
            {
                elapsed += Time.deltaTime;
                interactProgress.SetProgress(elapsed / interactTime);
                yield return null;
            }
            interactProgress.gameObject.SetActive(false);
            _isCanInteract = true;
            
            playerInventory.AddItem(giveItemOnInteract);
        }
    }
}