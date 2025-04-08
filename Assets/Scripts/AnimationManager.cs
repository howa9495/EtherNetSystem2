using UnityEngine;
using UnityEngine.EventSystems;
using ECM.Controllers;
using ECM.Components;

public class AnimationManager : MonoBehaviour
{
    private Animator animator;
    public BaseCharacterController characterController;
    public MouseLook mouseLook;

    private float previousYRotation;
    public float moveSpeed = 5.0f;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        characterController = GetComponent<BaseCharacterController>();
        //rigidbody = GetComponent<Rigidbody>();

        moveSpeed = characterController.speed;
    }

    // Update is called once per frame
    void Update()
    {
        Animate();
    }
    void Animate()
    {
        // If no animator, return

        if (animator == null)
            return;

        // Compute move vector in local space

        //var move = transform.InverseTransformDirection(characterController.moveDirection);
        var move = transform.InverseTransformDirection(characterController.movement.cachedRigidbody.linearVelocity);

        // Update the animator parameters
        /*
        var forwardAmount = animator.applyRootMotion
            ? Mathf.InverseLerp(0.0f, runSpeed, move.z * speed)
            : Mathf.InverseLerp(0.0f, runSpeed, movement.forwardSpeed);
        */
        animator.SetFloat("Horizontal", move.x/ moveSpeed, 0.1f, Time.deltaTime);
        animator.SetFloat("Vertical", move.z / moveSpeed, 0.1f, Time.deltaTime);
        //animator.SetFloat("Forward", forwardAmount, 0.1f, Time.deltaTime);
        var rotate = transform.InverseTransformDirection(characterController.movement.cachedRigidbody.angularVelocity);
        rotate.y = 0;
        animator.SetFloat("Rotation", DetectRotation());

        animator.SetBool("OnGround", characterController.movement.isGrounded);

        animator.SetBool("Crouch", characterController.isCrouching);

        if (!characterController.movement.isGrounded)
        {
            animator.SetFloat("Jump", characterController.movement.velocity.y, 0.1f, Time.deltaTime);
        }
        else
        {
            animator.SetFloat("Jump", 0);
        }
    }
    float DetectRotation()
    {

        // 獲取 Rigidbody 的當前旋轉
        Quaternion currentRotation = characterController.movement.cachedRigidbody.rotation;

        // 獲取 Y 軸旋轉角度
        float currentYRotation = currentRotation.eulerAngles.y;

        // 計算 Y 軸旋轉變化
        float yRotationChange = CalculateYRotationChange(currentYRotation, previousYRotation);

        // 更新前一幀的 Y 軸旋轉
        previousYRotation = currentYRotation;

        // 獲取 Y 軸旋轉幅度
        float yRotationChangeInRadians = yRotationChange * Mathf.Deg2Rad;
        return yRotationChangeInRadians * 10f;
    }
    float CalculateYRotationChange(float currentYRotation, float previousYRotation)
    {
        float yRotationChange = currentYRotation - previousYRotation;

        if (yRotationChange < -180)
        {
            yRotationChange += 360;
        }
        else if (yRotationChange > 180)
        {
            yRotationChange -= 360;
        }

        return yRotationChange;
    }
}
