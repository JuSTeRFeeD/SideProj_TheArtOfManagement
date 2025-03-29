using Project.Scripts.NodeSystem;
using Project.Scripts.NPC;
using UnityEngine;

namespace Project.Scripts.Player
{
    public class PlayerDialogueController : MonoBehaviour
    {
        [SerializeField] private float talkDistance = 3f;
        
        private readonly Collider[] _results = new Collider[10];
        
        private DialogueCompanion _curNearestDialogueCompanion;
        
        private void Update()
        {
            FindNearestDialogueCompanion();
            HandleStartDialogue();
        }

        private void HandleStartDialogue()
        {
            if (!_curNearestDialogueCompanion) return;
            if (DialogueManager.Instance.IsDialogueActive) return;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                DialogueManager.Instance.StartDialogue(_curNearestDialogueCompanion);
            }
        }

        private void FindNearestDialogueCompanion()
        {
            var count = Physics.OverlapSphereNonAlloc(transform.position, talkDistance, _results);
            if (count == 0) return;
            
            var nearestSqrDist = Mathf.Infinity;
            DialogueCompanion nearestDialogueCompanion = null;
            for (var index = 0; index < count; index++)
            {
                var result = _results[index];
                if (result.TryGetComponent(out DialogueCompanion dialogueCompanion))
                {
                    if (!dialogueCompanion.HasAvailableDialogues) continue;

                    var sqrDist = (dialogueCompanion.transform.position - transform.position).sqrMagnitude;
                    if (sqrDist < talkDistance && sqrDist < nearestSqrDist)
                    {
                        nearestSqrDist = sqrDist; 
                        nearestDialogueCompanion = dialogueCompanion;
                    }
                }
            }

            if (_curNearestDialogueCompanion) _curNearestDialogueCompanion.DialogueMarker.SetCanTalk(false);
            _curNearestDialogueCompanion = nearestDialogueCompanion;
            if (_curNearestDialogueCompanion) _curNearestDialogueCompanion.DialogueMarker.SetCanTalk(true);
        }
    }
}