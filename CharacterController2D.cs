using System;
using UnityEngine;

namespace AoOkami.CharacterController
{
    public class CharacterController2D : MonoBehaviour
    {
        public static event Action OnLanded;
        public static event Action OnFalling;
        public static event Action OnJump;
        public static event Action OnDoubleJump;
        public static event Action<bool> OnCrouch;

        [Header("Movement")]
        [Range(0, 1)] [SerializeField] float crouchSpeed = .36f;
        [SerializeField] float jumpForce = 12f;
        [SerializeField] bool airControl = false;
        [SerializeField] bool canDoubleJump = false;

        [Header("Collision")]
        [SerializeField] LayerMask groundMask;
        [SerializeField] Transform groundCheck;
        [SerializeField] Transform ceilingCheck;
        [SerializeField] Vector2 groundCheckSize = new Vector2(0.32f, 0.03f);

        [Header("Rigidbody")]
        [SerializeField] Rigidbody2D characterRb;

        public bool IsGrounded { get; private set; }

        private bool _facingRight = true;
        private bool _isCrouching = false;
        private bool _isCheckingCeiling = false;
        private bool _canResetVelocity = true;
        private float _movementMultiplier = 1f;
        private int _jumpsCount = 0;

        private const float CEILING_RADIUS = .2f;

        private void OnDrawGizmos()
        {
            if (groundCheck == null) return;

            Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
        }

        public void Move(float moveAmount)
        {
            if ((IsGrounded || airControl) && moveAmount != 0)
            {
                if (_isCheckingCeiling) CeilingCheck(); //check if the player can stand up after releasing crouch key

                moveAmount *= _movementMultiplier;
                characterRb.velocity = new Vector2(moveAmount, characterRb.velocity.y);

                if (moveAmount > 0 && !_facingRight || moveAmount < 0 && _facingRight)
                    FlipCharacter();

                _canResetVelocity = true;
            }
            else
            {
                if (_canResetVelocity)
                {
                    characterRb.velocity = new Vector2(0, characterRb.velocity.y);
                    _canResetVelocity = false;
                }
            }
        }

        public void Crouch(bool isCrouching)
        {
            if (isCrouching)
            {
                _movementMultiplier = crouchSpeed;
                _isCrouching = true;
                OnCrouch?.Invoke(true);
            }
            else
            {
                _isCheckingCeiling = true;
            }
        }

        public void Jump()
        {
            if (IsGrounded && !_isCrouching)
            {
                IsGrounded = false;
                OnJump?.Invoke();
                characterRb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                _jumpsCount++;
            }
            else if (!IsGrounded && canDoubleJump && _jumpsCount < 2)
            {
                OnDoubleJump?.Invoke();
                characterRb.velocity = new Vector2(characterRb.velocity.x, 0);
                characterRb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                _jumpsCount++;
            }
        }

        private void FlipCharacter()
        {
            _facingRight = !_facingRight;
            transform.Rotate(0f, 180f, 0f);
        }

        private bool IsObjectsMaskSameAsGrounds(GameObject obj) => (groundMask.value & (1 << obj.layer)) > 0;

        private bool IsGroundBeneath() => Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, groundMask);

        private void CeilingCheck()
        {
            _isCrouching = Physics2D.OverlapCircle(ceilingCheck.position, CEILING_RADIUS, groundMask) != null;

            if (!_isCrouching)
            {
                _movementMultiplier = 1f;
                _isCheckingCeiling = false;
                OnCrouch?.Invoke(false);
            }
        }

        private bool CanCheckBeingGrounded() => !IsGrounded && characterRb.velocity.y <= 0.001f;

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (CanCheckBeingGrounded() && IsObjectsMaskSameAsGrounds(collision.gameObject) && IsGroundBeneath() && collision.enabled)
            {
                IsGrounded = true;
                _jumpsCount = 0;
                OnLanded?.Invoke();
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (IsGrounded && IsObjectsMaskSameAsGrounds(collision.gameObject) && !IsGroundBeneath())
            {
                IsGrounded = false;
                OnFalling?.Invoke();
            }
        }
    }
}
