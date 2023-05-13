using UnityEngine;

namespace Core
{
    public class PlayerInput
    {
        public Vector2 Look { get; private set; }
        public Vector2 Move { get; private set; }
        public Vector2 LastMove { get; private set; }
        public bool Sprint { get; private set; }
        public bool Jump { get; private set; }

        public void ResetJump()
            => Jump = false;

        public void SetLook(Vector2 look)
            => Look = look;

        public void SetMove(Vector2 move)
            => Move = move;

        public void SetLastMove(Vector2 move)
            => LastMove = move;
        
        public void SetSprint(bool state)
            => Sprint = state;

        public void SetJump(bool state)
            => Jump = state;
    }
}