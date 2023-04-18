using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float speed;
    public float rotationSpeed;
    public float jumpSpeed;
    public float jumpGrace;

    [SerializeField]
    private Transform cameraTransform;

    private Animator animator;
    private CharacterController cC;
    private float ySpeed;
    private float originalStepOffset;
    private float? lastGroundedTime;
    private float? jumpPressed;

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

        float magnitude = Mathf.Clamp01(moveDirection.magnitude)*speed;
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
            if (Time.time - jumpPressed <= jumpGrace)
            {
                ySpeed = jumpSpeed;
                jumpPressed = null;
                lastGroundedTime = null;
            }
        }
        else
        {
            cC.stepOffset = 0;
        }
        
        Vector3 velocity = moveDirection * magnitude;
        velocity.y = ySpeed;
        cC.Move(velocity * Time.deltaTime);

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
}
