using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace WildIsland.Core
{
    public class PlayerInput : MonoBehaviour
    {
        public Vector2 Move { get; private set; }
        public Vector2 Look { get; private set; }
        public bool Jump { get; private set; }
        public bool Sprint { get; private set; }

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

        public void ResetJump()
            => Jump = false;

        private void MoveInput(Vector2 newMoveDirection)
            => Move = newMoveDirection;

        private void LookInput(Vector2 newLookDirection)
            => Look = newLookDirection;

        private void JumpInput(bool newJumpState)
            => Jump = newJumpState;

        private void SprintInput(bool newSprintState)
            => Sprint = newSprintState;

        private void OnApplicationFocus(bool hasFocus)
            => SetCursorState(true);

        private static void SetCursorState(bool newState)
            => Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
}