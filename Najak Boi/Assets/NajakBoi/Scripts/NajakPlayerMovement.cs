using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NajakBoi.Scripts
{
    public class NajakPlayerMovement : MonoBehaviour
    {
        public NajakPlayerController2D controller;
        public Animator anim;

        private float _horizontalMove = 0.0f;
        private bool _jump = false;
        private bool _crouch = false;
        public float runSpeed = 40.0f;

        public float jumpChargeMultiplier = 200.0f;
        public float chargeTimeLimit = 1.25f;
        public float defaultJumpForce = 500.0f;
        private float _chargedForce;
        private bool _isCharging;
        private static readonly int HasLanded = Animator.StringToHash("hasLanded");
        private static readonly int IsGrounded = Animator.StringToHash("isGrounded");
        private static readonly int IsCrouching = Animator.StringToHash("isCrouching");
        private static readonly int IsJumping = Animator.StringToHash("isJumping");
        private static readonly int IsCharging = Animator.StringToHash("isCharging");
        private static readonly int Speed = Animator.StringToHash("Speed");


        private void Awake()
        {
            controller = GetComponent<NajakPlayerController2D>();
            anim = GetComponent<Animator>();
        }

        // Update is called once per frame
        private void Update()
        {
            //TODO IMPLEMENT NEW INPUT SYSTEM
            var kb = Keyboard.current;
            _horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

            if (kb.spaceKey.wasPressedThisFrame && controller.m_Grounded)
            {
                _isCharging = true;
                _chargedForce += Time.deltaTime;

            }
            
            if(kb.spaceKey.wasReleasedThisFrame)
            {
                controller.m_JumpForce = Mathf.Max(defaultJumpForce + _chargedForce * jumpChargeMultiplier, 1000.0f);
                _chargedForce = 0.0f;
                _isCharging = false;
                if(controller.m_Grounded)
                    _jump = true;
            }
        
            if (kb.ctrlKey.wasPressedThisFrame)
            {
                _crouch = true;
            } 
            else if (kb.ctrlKey.wasReleasedThisFrame)
            {
                _crouch = false;
            }

            anim.SetFloat(Speed, Mathf.Abs(_horizontalMove));
            anim.SetBool(IsCharging, _isCharging);
            anim.SetBool(IsJumping, _jump);
            anim.SetBool(IsCrouching, _crouch);
            anim.SetBool(IsGrounded, controller.m_Grounded);
        
            if(_isCharging == false && _jump == false)
                controller.m_JumpForce = defaultJumpForce;
        }

        public void OnLanding()
        {
            anim.SetTrigger(HasLanded);
        }


        private IEnumerator ChargeJump()
        {
            var chargeTime = 0.0f;
            _isCharging = true;

            var kb = Keyboard.current;
            while (!kb.spaceKey.wasReleasedThisFrame)
            {
            
            
                chargeTime += Time.deltaTime;
                var force = defaultJumpForce + jumpChargeMultiplier * chargeTime;
                Debug.Log(defaultJumpForce + jumpChargeMultiplier * chargeTime);

                if (force > 1000.0f)
                    force = 1000.0f;

                controller.m_JumpForce = force;
            
                if (chargeTime > chargeTimeLimit)
                {
                    _jump = true;
                    _isCharging = false;
                    yield return null;
                }

            
                yield return null;
            }

            _jump = true;
            _isCharging = false;
        }

        private void FixedUpdate()
        {
            controller.Move(_horizontalMove * Time.fixedDeltaTime, _crouch, _jump);
            _jump = false;
        }
    }
}
