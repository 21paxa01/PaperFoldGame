using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JetSystems
{

    public class JoystickInput : JetCharacterInput
    {
        private Vector2 pressedPosition;
        private Vector2 currentPosition;


        protected override void OnMousePressed()
        {
            base.OnMousePressed();
            StorePressedMousePosition();
        }

        protected override void OnMouseDragged()
        {
            base.OnMouseDragged();
            CalculateMouseDifference();
        }

        protected override void OnMouseReleased()
        {
            base.OnMouseReleased();
        }




        private void StorePressedMousePosition()
        {
            pressedPosition = Input.mousePosition;
        }

        private void CalculateMouseDifference()
        {
            currentPosition = Input.mousePosition;
            Vector3 difference = currentPosition - pressedPosition;

            difference.z = difference.y;

            mouseDragVector = difference;
        }
    }
}