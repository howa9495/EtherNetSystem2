using UnityEngine;
using ECM.Controllers;
using UnityEngine.Events;
using System.Collections;

namespace PlayerSystems
{
    public class ExtraCharacterController : BaseFirstPersonController
    {
        StaminaManager staminaManager;

        private bool canRun = true;
        private bool canMove = true;
        private float speedDamping = 1f;
        protected const float breathingTime = 1.5f;
        private bool breathing = false;

        private float dampedSpeed;

        [Header("Event")]
        [Tooltip("Event of Jump.")]
        [SerializeField]
        public UnityEvent JumpEvent;

        [Tooltip("Event of Run.")]
        [SerializeField]
        public UnityEvent RunEvent;

        private void Start()
        {
            staminaManager = FindAnyObjectByType<StaminaManager>();
        }

        protected override void MidAirJump()
        {
            // Reset mid-air jumps counter

            if (_midAirJumpCount > 0 && movement.isGrounded)
                _midAirJumpCount = 0;

            // If jump button not pressed, or still not released, return

            if (!_jump || !_canJump)
                return;

            // If grounded, return

            if (movement.isGrounded)
                return;

            // Have mid-air jumps?

            if (_midAirJumpCount >= _maxMidAirJumps)
                return;

            _midAirJumpCount++;         // Increase mid-air jumps counter

            _canJump = false;           // Halt jump until jump button is released
            _isJumping = true;          // Update isJumping flag
            _updateJumpTimer = true;    // Allow mid-air jump to be variable height

            // Apply jump impulse

            movement.ApplyVerticalImpulse(jumpImpulse);

            JumpEvent?.Invoke();///

            // 'Pause' grounding, allowing character to safely leave the 'ground'

            movement.DisableGrounding();
        }

        protected virtual void StaminaRun()
        {

            if (breathing) 
                
            //Debug.Log("Breathing");
            //Debug.Log("Stamina: " + staminaManager.GetCurrentStamina());
            //Debug.Log("RigidSpeed: " + movement.cachedRigidbody.linearVelocity.magnitude);


            //Debug.Log("Damping " + dampedSpeed);
           // Debug.Log("BaseSpeed " + base.speed);
            //Debug.Log("Speed: " + speed);
            if (breathing) return;
            run = canRun ? run : false;



            speedDamping = staminaManager.GetCurrentStamina() / staminaManager.GetMaxStamina() <= 0.5 ? ((staminaManager.GetCurrentStamina() / staminaManager.GetMaxStamina()) * 2f) : 1;

            if (run && canRun && (moveDirection.x != 0f || moveDirection.z != 0f))
            {
                staminaManager.DecreaseStamina(12.5f * Time.deltaTime, out bool success);
                canRun = success;

                if (!success)
                {
                    Breathing();
                }
                RunEvent?.Invoke();
            }
            else
            {
                staminaManager.IncreaseStamina(8f * Time.deltaTime);
                if (canRun == false)
                {
                    if (staminaManager.GetCurrentStamina() > 50)
                    {
                        canRun = true;
                    }
                }
            }
            dampedSpeed = base.speed * speedDamping;
        }

        void Breathing(float time = breathingTime)
        {
            breathing = true;
            canMove = false;
            canRun = false;

            StartCoroutine(Recover(breathingTime));
        }

        IEnumerator Recover(float recoverTime)
        {
            yield return new WaitForSeconds(recoverTime);
            breathing = false;
            canMove = true;
            canRun = true;
        }
        protected override void Move()
        {
            StaminaRun();


                // Apply movement

                // If using root motion and root motion is being applied (eg: grounded),
                // move without acceleration / deceleration, let the animation takes full control

                var desiredVelocity = breathing ? Vector3.zero:CalcDesiredVelocity();



            if (useRootMotion && applyRootMotion)
            {
                movement.Move(desiredVelocity, dampedSpeed, !allowVerticalMovement);
            }
            else
            {
                // Move with acceleration and friction

                var currentFriction = isGrounded ? groundFriction : airFriction;
                var currentBrakingFriction = useBrakingFriction ? brakingFriction : currentFriction;


                movement.Move(desiredVelocity, dampedSpeed, acceleration, deceleration, currentFriction,
                currentBrakingFriction, !allowVerticalMovement);


            }

            // Jump logic

            Jump();
            MidAirJump();
            UpdateJumpTimer();

            // Update root motion state,
            // should animator root motion be enabled? (eg: is grounded)

            applyRootMotion = useRootMotion && movement.isGrounded;

        }

    }

}
