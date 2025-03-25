using Project.Scripts.NodeSystem;
using Project.Scripts.Scriptable;
using UnityEngine;

namespace Project.Scripts.NPC
{
    public class DialogueCompanion : MonoBehaviour
    {
        [SerializeField] private EmployerData employerData;
        [SerializeField] private DialogueGraph dialogue;

        public EmployerData EmployerData => employerData;
        public DialogueGraph Dialogue => dialogue;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                DialogueManager.Instance.StartDialogue(this);
            }
        }
    }
}