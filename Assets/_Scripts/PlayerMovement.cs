using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float rotationSpeed;
    public float jumpSpeed;
    public float jumpGrace;

    [SerializeField]
    private float jumpHorizontalSpeed;

    [SerializeField]
    private Transform cameraTransform;

    private Animator animator;
    private CharacterController cC;
    private float ySpeed;
    private float originalStepOffset;
    private float? lastGroundedTime;
    private float? jumpPressed;

    private bool isJumping;
    private bool isGrounded;

    // Start is called before the first frame update
    void Start()
    {
        cC = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        originalStepOffset = cC.stepOffset;
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontalInput, 0, verticalInput);

        moveDirection = Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up)* moveDirection;

        float inputMagnitude = Mathf.Clamp01(moveDirection.magnitude);

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            inputMagnitude /= 2;
        }
        animator.SetFloat("InputMagnitude", inputMagnitude, 0.05f, Time.deltaTime);

        moveDirection.Normalize();

        ySpeed += Physics.gravity.y * Time.deltaTime;

        if (cC.isGrounded)
        {
            lastGroundedTime = Time.time;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpPressed = Time.time;
        }


        if (Time.time - lastGroundedTime <= jumpGrace)
        {
            cC.stepOffset = originalStepOffset;
            ySpeed = -0.5f;

            animator.SetBool("isGrounded", true);
            isGrounded = true;
            animator.SetBool("isJumping", false);
            isJumping = false;
            animator.SetBool("isFalling", false);

            if (Time.time - jumpPressed <= jumpGrace)
            {
                ySpeed = jumpSpeed;

                animator.SetBool("isJumping", true);
                isJumping = true;

                jumpPressed = null;
                lastGroundedTime = null;
            }
        }
        else
        {
            cC.stepOffset = 0;
            animator.SetBool("isGrounded", false);
            isGrounded = false;

            if ((isJumping && ySpeed < 0) || ySpeed < -2)
            {
                animator.SetBool("isFalling", true);
            }
        }
        
        

        if(moveDirection != Vector3.zero)
        {
            animator.SetBool("isMoving", true);

            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }

        if (isGrounded == false){
            Vector3 velocity = moveDirection * inputMagnitude * jumpHorizontalSpeed;
            velocity.y = ySpeed;

            cC.Move(velocity * Time.deltaTime);
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void OnAnimatorMove()
    {
        if (isGrounded)
        {
            Vector3 velocity = animator.deltaPosition;   
            velocity.y = ySpeed * Time.deltaTime;
            
            cC.Move(velocity);
        }
    }
}
