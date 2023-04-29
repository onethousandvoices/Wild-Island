using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    public class PlayerInput : MonoBehaviour
    {
        public Vector2 move;
        public Vector2 look;
        public bool jump;
        public bool sprint;

        public bool analogMovement;

#if ENABLE_INPUT_SYSTEM
        public void OnMove(InputValue value)
            => MoveInput(value.Get<Vector2>());

        public void OnLook(InputValue value)
            => LookInput(value.Get<Vector2>());

        public void OnJump(InputValue value)
            => JumpInput(value.isPressed);

        public void OnSprint(InputValue value)
            => SprintInput(value.isPressed);
#endif

        public void MoveInput(Vector2 newMoveDirection)
            => move = newMoveDirection;

        public void LookInput(Vector2 newLookDirection)
            => look = newLookDirection;

        public void JumpInput(bool newJumpState)
            => jump = newJumpState;

        public void SprintInput(bool newSprintState)
            => sprint = newSprintState;

        private void OnApplicationFocus(bool hasFocus)
            => SetCursorState(true);

        private static void SetCursorState(bool newState)
            => Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
}