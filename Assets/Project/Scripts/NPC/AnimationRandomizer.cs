using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Project.Scripts.NPC
{
    public class AnimationRandomizer : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        // Idle randomizer
        [SerializeField] private bool isIdleStay;
        private float _delay;
        private static readonly int RandomIdle = Animator.StringToHash("randomIdle");

        private void OnValidate()
        {
            animator = GetComponentInChildren<Animator>();
        }

        private void Start()
        {
            SetRandDelay();
            if (isIdleStay) SetRandomIdle();
        }

        private void SetRandDelay() => _delay = Random.Range(1, 5);

        private void Update()
        {
            _delay -= Time.deltaTime;
            if (_delay > 0) return;
            SetRandDelay();

            if (isIdleStay)
            {
                SetRandomIdle();
            }
        }

        private void SetRandomIdle()
        {
            animator.SetInteger(RandomIdle, Random.Range(0, 2));
        }
    }
}