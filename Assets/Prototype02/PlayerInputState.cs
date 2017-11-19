using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype02
{
    class PlayerInputState
    {
        public ContinousInput Continous { get; set; }
        public SingleFrameInput SingleFrame { get; set; }

        public PlayerInputState()
        {
            Continous = new ContinousInput();
            SingleFrame = new SingleFrameInput();
        }

        public void UpdateInput()
        {
            Continous.Horizontal = Input.GetAxisRaw("Horizontal");
            Continous.Vertical = Input.GetAxisRaw("Vertical");
            Continous.Jump = Input.GetButton("Jump"); ;
            Continous.Attack = Input.GetButton("Attack"); ;

            SingleFrame.Jump = getSingleFrameButtonState("Jump");
            SingleFrame.Attack = getSingleFrameButtonState("Attack");
        }

        private SingleFrameInput.ButtonState getSingleFrameButtonState(string buttonName)
        {
            if (Input.GetButtonDown(buttonName))
            {
                return SingleFrameInput.ButtonState.DOWN;
            }
            if (Input.GetButtonUp(buttonName))
            {
                return SingleFrameInput.ButtonState.UP;
            }
            return SingleFrameInput.ButtonState.UNCHANGED;
        }

        public class ContinousInput
        {
            public float Horizontal { get; set; }
            public float Vertical { get; set; }
            public bool Jump { get; set; }
            public bool Attack { get; set; }
        }

        public class SingleFrameInput
        {
            public ButtonState Jump { get; set; }
            public ButtonState Attack { get; set; }

            public enum ButtonState
            {
                UNCHANGED, UP, DOWN
            }
        }
    }
}
