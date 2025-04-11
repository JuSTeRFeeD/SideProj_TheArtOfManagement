using Project.Scripts.NodeSystem;
using Project.Scripts.NodeSystem.Dialogues;
using UnityEngine;
using UnityEngine.AI;

namespace Project.Scripts.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float walkSpeed = 3.5f;
        [SerializeField] private float sprintSpeed = 7f;
        
        [Header("References")]
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Animator animator;

        private Camera _mainCamera;
        private static readonly int IsWalk = Animator.StringToHash("isWalk");
        private static readonly int MoveSpeed = Animator.StringToHash("moveSpeed");
        private static readonly int IsRun = Animator.StringToHash("isRun");

        private void Start()
        {
            _mainCamera = Camera.main;
            agent.speed = walkSpeed;
            if (_mainCamera == null)
            {
                Debug.Log("Main Camera not found.");
            }
        }

        private void Update()
        {
            if (DialogueManager.Instance.IsDialogueActive) return;
            MovePlayer();
        }

        private void MovePlayer()
        {
            var moveDirection = Vector3.zero;
        
            if (Input.GetKey(KeyCode.W)) moveDirection += Vector3.forward;
            if (Input.GetKey(KeyCode.S)) moveDirection += Vector3.back;
            if (Input.GetKey(KeyCode.A)) moveDirection += Vector3.left;
            if (Input.GetKey(KeyCode.D)) moveDirection += Vector3.right;
        
            if (moveDirection != Vector3.zero)
            {
                moveDirection.Normalize();
                moveDirection = _mainCamera.transform.TransformDirection(moveDirection);
                moveDirection.y = 0;
                moveDirection.Normalize();
                agent.destination = transform.position + moveDirection;
            }
            var isRun = Input.GetKey(KeyCode.LeftShift);
            agent.speed = isRun ? sprintSpeed : walkSpeed;
            
            animator.SetBool(IsWalk, agent.velocity.magnitude > 0);
            animator.SetBool(IsRun, isRun);
            animator.SetFloat(MoveSpeed, agent.velocity.magnitude, 0.1f, Time.deltaTime);
        
        }
    }
}