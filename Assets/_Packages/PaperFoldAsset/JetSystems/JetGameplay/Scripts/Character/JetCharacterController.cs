using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JetSystems
{
    public class JetCharacterController : MonoBehaviour
    {
        public delegate void OnCharacterStartedMoving();
        public OnCharacterStartedMoving onCharacterStartedMoving;

        public delegate void OnCharacterMoving(Vector3 movementVector);
        public OnCharacterMoving onCharacterMoving;

        public delegate void OnCharacterStoppedMoving();
        public OnCharacterStoppedMoving onCharacterStoppedMoving;

        public enum MovementType { Physics, Controlled }
        [SerializeField] private MovementType characterMovementType;

        public enum ControlType { Tap, Slide, Joystick }
        [SerializeField] private ControlType characterControlType;

        [Header(" General Settings ")]
        private bool canMove;

        [Header(" Physics Settings ")]
        [SerializeField] private float moveSpeed;
        private Rigidbody rig;

        private void Awake()
        {
            ConfigureControlType();
        }

        private void ConfigureControlType()
        {
            switch(characterMovementType)
            {
                case MovementType.Physics:
                    rig = GetComponent<Rigidbody>();
                    break;

                case MovementType.Controlled:
                    break;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            canMove = true;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void StartMoving()
        {
            onCharacterStartedMoving?.Invoke();
        }

        public void MoveCharacter(Vector3 inputVector)
        {
            if (!CanMove()) return;

            onCharacterMoving?.Invoke(inputVector);

            switch (characterMovementType)
            {
                case MovementType.Physics:
                    MovePhysicsCharacter(inputVector);
                    break;

                case MovementType.Controlled:
                    MoveControlledCharacter(inputVector);
                    break;
            }

        }

        public void StopMoving()
        {
            onCharacterStoppedMoving?.Invoke();
        }

        private void MovePhysicsCharacter(Vector3 inputVector)
        {
            rig.velocity = inputVector * moveSpeed;
        }

        private void MoveControlledCharacter(Vector3 inputVector)
        {

        }

        private bool CanMove()
        {
            return canMove;
        }
    }
}