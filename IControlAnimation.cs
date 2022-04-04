
namespace AoOkami.CharacterController2D
{
    public interface IControlAnimation
    {
        public void OnLanding();
        public void OnFalling();
        public void OnCrouching(bool isCrouching);
        public void OnJumping();
        public void OnDoubleJump();
    }
}
