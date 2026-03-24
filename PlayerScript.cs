using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerScript : MonoBehaviour
{

    [Header ("Player Health Things")]
    private float playerHealth = 1000f;
    private float presentHealth;

    [Header("Player Movement")]
    public float playerSpeed = 1.9f;

    public float currentPlayerSpeed = 0f;

    public float playerSprint = 3f;

    public float currentPlayerSprint = 0f;
    public HealthBar healthBar;

    //follow camera
    [Header("Player Camera")]
    public Transform playerCamera;

    [Header("Player Animator and Gravity")]
    public CharacterController cC;
    public float gravity = -9.81f;

    public Animator animator;

    [Header("Player Jumping & velocity")]
    public float jumpRange = 1f;
    public float turnCalmtime = 0.1f;
    float turnCalmVelocity;
    Vector3 velocity;
    public Transform surfaceCheck;
    bool onSurface;
    public float surfaceDistance = 0.4f;
    public LayerMask surfaceMask;


    public bool mobileInputs;
    public FixedJoystick joystick;
    public FixedJoystick Sprintjoystick;    


    public void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        presentHealth = playerHealth;
        healthBar.GiveFullHealth(playerHealth);
    }


    void Update()
    {

        if(currentPlayerSpeed > 0)
        {
            Sprintjoystick = null;
        }
        else
        {
            FixedJoystick sprintJS = GameObject.Find("PlayerSprintJoystick").GetComponent<FixedJoystick>();
            Sprintjoystick = sprintJS;
        }

        onSurface = Physics.CheckSphere(surfaceCheck.position, surfaceDistance, surfaceMask);

        if(onSurface && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        //gravity
        //deltatime is due to time dependency
        velocity.y += gravity * Time.deltaTime;
        cC.Move(velocity * Time.deltaTime);

        playerMove();

        Jump();

        Sprint();
        
    }

    void playerMove()
    {
        if(mobileInputs == true)
        {
            float horizontal_axis = joystick.Horizontal;

            float vertical_axis = joystick.Vertical;



            Vector3 direction = new Vector3(horizontal_axis, 0f, vertical_axis).normalized;

            if(direction.magnitude >= 0.1f)
            {

                animator.SetBool("Walk", true);
                animator.SetBool("Running", false);
                animator.SetBool("Idle", false);
                animator.SetTrigger("Jump");
                animator.SetBool("AimWalk", false);
                animator.SetBool("IdleAim", false);

                //change player facing & move by following our camera
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + playerCamera.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnCalmVelocity, turnCalmtime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                //change player facing & move
                Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                cC.Move(moveDirection.normalized * playerSpeed * Time.deltaTime);

                //sprint
                currentPlayerSpeed = playerSpeed;
            }

            else
            {
                animator.SetBool("Idle", true);
                animator.SetTrigger("Jump");
                animator.SetBool("Walk", false);
                animator.SetBool("Running", false);
                animator.SetBool("AimWalk", false);
                currentPlayerSpeed = 0f;
            }
        }

        else
        {
            float horizontal_axis = Input.GetAxisRaw("Horizontal");

            float vertical_axis = Input.GetAxisRaw("Vertical");



            Vector3 direction = new Vector3(horizontal_axis, 0f, vertical_axis).normalized;

            if(direction.magnitude >= 0.1f)
            {

                animator.SetBool("Walk", true);
                animator.SetBool("Running", false);
                animator.SetBool("Idle", false);
                animator.SetTrigger("Jump");
                animator.SetBool("AimWalk", false);
                animator.SetBool("IdleAim", false);

                //change player facing & move by following our camera
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + playerCamera.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnCalmVelocity, turnCalmtime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                //change player facing & move
                Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                cC.Move(moveDirection.normalized * playerSpeed * Time.deltaTime);

                //sprint
                currentPlayerSpeed = playerSpeed;
            }

            else
            {
                animator.SetBool("Idle", true);
                animator.SetTrigger("Jump");
                animator.SetBool("Walk", false);
                animator.SetBool("Running", false);
                animator.SetBool("AimWalk", false);
                currentPlayerSpeed = 0f;
            }
        }
    }

    //jump
    void Jump()
    {
        if(mobileInputs == true)
        {
            if(CrossPlatformInputManager.GetButtonDown("Jump") && onSurface)
            {
                animator.SetBool("Walk", false);
                animator.SetTrigger("Jump");
                velocity.y = Mathf.Sqrt(jumpRange * -2 * gravity);
                //animator.ResetTrigger("Jump");
            }
            else
            {
                animator.ResetTrigger("Jump");
            }
        }
        else
        {
            if(Input.GetButtonDown("Jump") && onSurface)
            {
                animator.SetBool("Walk", false);
                animator.SetTrigger("Jump");
                velocity.y = Mathf.Sqrt(jumpRange * -2 * gravity);
                //animator.ResetTrigger("Jump");
            }
            else
            {
                animator.ResetTrigger("Jump");
            }
        }
    }


    //sprint animation
    void Sprint()
    {

        if(mobileInputs == true)
        {
            float horizontal_axis = Sprintjoystick.Horizontal;

            float vertical_axis = Sprintjoystick.Vertical;



            Vector3 direction = new Vector3(horizontal_axis, 0f, vertical_axis).normalized;

            if(direction.magnitude >= 0.1f)
            {

                animator.SetBool("Running", true);
                animator.SetBool("Idle", false);
                //animator.SetTrigger("Jump");
                animator.SetBool("Walk", false);
                animator.SetBool("IdleAim", false);

                //change player facing & move by following our camera
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + playerCamera.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnCalmVelocity, turnCalmtime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                //change player facing & move
                Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                cC.Move(moveDirection.normalized * playerSprint * Time.deltaTime);

                //sprint
                currentPlayerSprint = playerSprint;
            }

            else
            {
                
                animator.SetBool("Idle", false);
                //animator.SetTrigger("Jump");
                animator.SetBool("Walk", false);
                
                currentPlayerSprint = 0f;
            }
        }

        else
        {
            float horizontal_axis = Input.GetAxisRaw("Horizontal");

            float vertical_axis = Input.GetAxisRaw("Vertical");



            Vector3 direction = new Vector3(horizontal_axis, 0f, vertical_axis).normalized;

            if(direction.magnitude >= 0.1f)
            {

                animator.SetBool("Running", true);
                animator.SetBool("Idle", false);
                //animator.SetTrigger("Jump");
                animator.SetBool("Walk", false);
                animator.SetBool("AimWalk", false);

                //change player facing & move by following our camera
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + playerCamera.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnCalmVelocity, turnCalmtime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                //change player facing & move
                Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                cC.Move(moveDirection.normalized * playerSprint * Time.deltaTime);

                //sprint
                currentPlayerSprint = playerSprint;
            }

            else
            {
                animator.SetBool("Idle", false);
                //animator.SetTrigger("Jump");
                animator.SetBool("Walk", false);
                currentPlayerSprint = 0f;
            }
        }
        
    }

    //playerhitdamage
    public void playerHitDamage(float takeDamage)
    {
        presentHealth = presentHealth - takeDamage;
        healthBar.SetHealth(presentHealth);

        if(presentHealth <= 0)
        {
            PlayerDie();
        }
    }

    private void PlayerDie()
    {
        Cursor.lockState = CursorLockMode.None;

        Object.Destroy(gameObject);
    }

}