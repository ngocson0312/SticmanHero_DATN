using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
    public class InputHandle : MonoBehaviour
    {
        [SerializeField] InputType inputType;
        public TouchButton jumpButton;
        public SkillButton primaryButton;
        public SkillButton secondaryButton;
        public TouchButton skillButton;
        public TouchButton dashButton;
        public StaticThumb staticThumb;
        public float horizontal;
        public bool jumpPressed;
        public bool primaryAttackPressed;
        public bool secondaryAttackPressed;
        public bool skillPressed;
        public bool dashPressed;
        bool readyToClear;
        bool isActive;
        private void Start()
        {
            inputType = InputType.KEYBOARD;
            if (Application.isMobilePlatform)
            {
                inputType = InputType.TOUCH;
            }
            ActiveInput();
        }
        public void ResetInput()
        {
            if (staticThumb != null)
            {
                staticThumb.InputExit();
            }
            horizontal = 0;
            jumpPressed = false;
            readyToClear = false;
            skillPressed = false;
            primaryAttackPressed = false;
            secondaryAttackPressed = false;
            primaryButton.ResetButton();
            secondaryButton.ResetButton();
        }
        public void ActiveInput()
        {
            isActive = true;
        }
        public void Deactive()
        {
            isActive = false;
        }
        private void Update()
        {
            //GameplayCtrl.Instance.gameState != GAME_STATE.GS_PLAYING ||
            if (!isActive)
            {
                ClearInputs();
                return;
            }
            ClearInputs();
            ProcessInput();
        }
        void ProcessInput()
        {
            if (inputType == InputType.KEYBOARD)
            {
                horizontal = Input.GetAxisRaw("Horizontal");
                jumpPressed = jumpPressed || Input.GetKeyDown(KeyCode.Space);
                skillPressed = skillPressed || Input.GetKeyDown(KeyCode.R);
                dashPressed = dashPressed || Input.GetKeyDown(KeyCode.LeftShift);
                primaryAttackPressed = primaryAttackPressed || Input.GetKey(KeyCode.J); /*|| Input.GetMouseButton(0);*/
                secondaryAttackPressed = secondaryAttackPressed || Input.GetMouseButton(1);
            }
            else if (inputType == InputType.TOUCH)
            {
                horizontal = staticThumb.horizontal;
                jumpPressed = jumpPressed || jumpButton.GetButtonDown();
                skillPressed = skillPressed || skillButton.GetButtonDown();
                dashPressed = dashPressed || dashButton.GetButtonDown();
                primaryAttackPressed = primaryAttackPressed || primaryButton.GetButton();
                secondaryAttackPressed = secondaryAttackPressed || secondaryButton.GetButton();
            }
        }
        public void ClearInputs()
        {
            if (!readyToClear)
            {
                return;
            }
            horizontal = 0;
            primaryAttackPressed = false;
            secondaryAttackPressed = false;
            jumpPressed = false;
            readyToClear = false;
            skillPressed = false;
            dashPressed = false;
        }
        private void FixedUpdate()
        {
            readyToClear = true;
        }
    }

    public enum InputType
    {
        KEYBOARD, TOUCH
    }
}

