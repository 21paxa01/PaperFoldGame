using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace JetSystems
{

    public abstract class JetCharacterInput : MonoBehaviour
    {
        public delegate void SendInput(Vector3 input);
        public SendInput sendInput;

        [SerializeField] private UnityEvent onMousePressed;
        [SerializeField] private MouseDragEvent onMouseDragged;
        [SerializeField] private UnityEvent onMouseReleased;

        protected Vector3 mouseDragVector;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            ManageInput();
        }

        protected void ManageInput()
        {
            if (Input.GetMouseButtonDown(0))
                OnMousePressed();
            else if (Input.GetMouseButton(0))
                OnMouseDragged();
            else if (Input.GetMouseButtonUp(0))
                OnMouseReleased();
        }

        protected virtual void OnMousePressed()
        {
            onMousePressed?.Invoke();
        }

        protected virtual void OnMouseDragged()
        {
            onMouseDragged?.Invoke(mouseDragVector);
        }

        protected virtual void OnMouseReleased()
        {
            onMouseReleased?.Invoke();
        }
    }

    [System.Serializable]
    public class MouseDragEvent : UnityEvent<Vector3>
    {

    }
}