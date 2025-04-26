using System;
using System.Collections;
using Project.Scripts.Inventory;
using Project.Scripts.NPC;
using Project.Scripts.Player;
using Project.Scripts.Scriptable;
using UnityEngine;

namespace Project.Scripts.Interactables
{
    public class InteractDance : MonoBehaviour, IInteractable
    {
        [SerializeField] private ItemData giveItemOnInteract;
        
        [SerializeField] private float interactTime = 1.5f;
        [Space]
        [SerializeField] private InteractProgress interactProgress;
        [SerializeField] private DialogueMarker dialogueMarker;
        
        public ItemData GiveItemAfterInteract => giveItemOnInteract;
        
        private bool _isCanInteract;
        private static readonly int IsDancing = Animator.StringToHash("isDancing");

        public Vector3 Position() => transform.position;

        public bool CanInteract() => _isCanInteract;
        public void SetCanInteract(bool value) => _isCanInteract = value;

        private void Start()
        {
            interactProgress.gameObject.SetActive(false);
            SetIsNearest(false);
        }

        public void SetIsNearest(bool value)
        {
            dialogueMarker.SetMarkerActive(value);
            dialogueMarker.SetCanInteract(value);
        }


        public void Interact(PlayerInteractController playerInteractController)
        {
            if (!playerInteractController.gameObject.TryGetComponent(out PlayerInventory playerInventory))
            {
                return;
            }

            StartCoroutine(InteractProgress(playerInventory));
        }

        public IEnumerator InteractProgress(PlayerInventory playerInventory)
        {
            var playerAnimation = playerInventory.GetComponentInChildren<Animator>();
            playerAnimation.SetBool(IsDancing, true);
            
            var playerInteractController = playerInventory.GetComponent<PlayerInteractController>();
            playerInteractController.inProgress = true;
            
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
            
            playerInventory.AddItem(giveItemOnInteract);
            
            playerInteractController.inProgress = false;
            playerAnimation.SetBool(IsDancing, false);
        }
    }
}