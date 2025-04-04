using Project.Scripts.Player;
using UnityEngine;

namespace Project.Scripts.Interactables
{
    public interface IInteractable
    {
        public Vector3 Position();
        public bool CanInteract();
        public void Interact(PlayerInteractController playerInteractController);
        public void SetIsNearest(bool value);
    }
}