using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JetSystems
{
    [RequireComponent(typeof(JetCharacterController))]
    public class JetCharacterAnimator : MonoBehaviour
    {
        [Header(" Managers ")]
        private JetCharacterController jetCharacterController;

        [Header(" Components ")]
        [SerializeField] private Animator animator;
        [SerializeField] private Transform characterRendererTransform;

        private void Awake()
        {
            jetCharacterController = GetComponent<JetCharacterController>();
            jetCharacterController.onCharacterMoving += OnCharacterMoving;
            jetCharacterController.onCharacterStartedMoving += PlayRunAnimation;
            jetCharacterController.onCharacterStoppedMoving += PlayIdleAnimation;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnCharacterMoving(Vector3 movementVector)
        {
            movementVector.y = 0;
            ManageRendererForward(movementVector);
        }

        private void ManageRendererForward(Vector3 movementDirection)
        {
            characterRendererTransform.forward = -movementDirection.normalized;
        }

        private void PlayIdleAnimation()
        {
            animator.Play("Idle");
        }

        private void PlayRunAnimation()
        {
            animator.Play("Run");
        }
    }
}