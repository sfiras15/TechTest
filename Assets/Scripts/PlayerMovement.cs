using System.Collections;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float sprintConsumptionRate = 2f;
    private float groundedSpeedMultiplier = 1f;
    private float moveSpeed;
    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;

    // the direction the player is going to move to 
    private Vector3 moveDirection;
    // to check if the player reached the goal or not
    private bool canMove;

    //speed in the air
    [SerializeField] private float airMinSpeed;
    [SerializeField] private float speedIncreaseMultiplier;


    [SerializeField] private float groundDrag;
    [SerializeField] private float rotateSpeed;

    [Header("Jumping")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float airMultiplier;
    [SerializeField] private bool readyToJump;

    [Header("Audio")]
    [SerializeField] private AudioClip LandingAudioClip;
    [SerializeField] private AudioClip[] FootstepAudioClips;
    [Range(0, 1)][SerializeField] private float FootstepAudioVolume = 0.5f;


    [Header("Keybinds")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Ground Check")]
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask whatIsGround;
    private bool grounded;
    [SerializeField] private float GroundedOffset = -0.14f;

    [SerializeField] private float GroundedRadius = 0.28f;

    [Header("References")]
    private CapsuleCollider capsuleCollider;
    private Animator animator;
    private Camera cam;

    private Rigidbody rb;

    public Vector3 forward;
    public Vector3 right;

    // tracking the state of the player 
    public MovementState state;
    public enum MovementState
    {
        freeze,
        walking,
        sprinting,
        attacking,
        air
    }

    public bool jumping;
    public bool attacking;
    public bool freeze;
    public bool restricted;


    //Testing

    private InputPlayer inputPlayer;

    private void Awake()
    {
        inputPlayer = GetComponent<InputPlayer>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        animator = GetComponent<Animator>();
        cam = Camera.main;

    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
    }

    private void Update()
    {
        // ground check

        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
        grounded = Physics.CheckSphere(spherePosition, GroundedRadius, whatIsGround,
            QueryTriggerInteraction.Ignore);

        MyInput();

        transform.forward = Vector3.Lerp(transform.forward, moveDirection, rotateSpeed * Time.deltaTime);

        SpeedControl();
        StateHandler();

        // handle drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
        animator.SetFloat("Speed", rb.velocity.magnitude);
    }


    private void FixedUpdate()
    {
        MovePlayer();
    }


    // Move the player in the camera's forward/backward direction and right/ left direction
    private void MyInput()
    {
        // Get the camera's forward and right vectors in world space
        forward = cam.transform.forward;
        right = cam.transform.right;

        // Flatten the vectors to disregard the y-component
        forward.y = 0f;
        right.y = 0f;

        // Normalize the vectors to make sure they have a length of 1
        forward.Normalize();
        right.Normalize();

        // Calculate the movement direction based on input keys and camera orientation
        Vector3 desiredMoveDirection = (Input.GetKey(KeyCode.Z) ? forward : Vector3.zero) +
                                        (Input.GetKey(KeyCode.S) ? -forward : Vector3.zero) +
                                        (Input.GetKey(KeyCode.Q) ? -right : Vector3.zero) +
                                        (Input.GetKey(KeyCode.D) ? right : Vector3.zero);

        // Normalize the movement direction to make sure it has a length of 1
        if (desiredMoveDirection.magnitude > 1f)
        {
            desiredMoveDirection.Normalize();
        }

        // Assign the calculated movement direction to moveDirection
        moveDirection = desiredMoveDirection;

        // when to jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded && !attacking)
        {
            jumping = true;
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

    }

    bool keepMomentum;

    private void StateHandler()
    {
        // Mode - Freeze
        if (freeze)
        {
            state = MovementState.freeze;
            rb.velocity = Vector3.zero;
            desiredMoveSpeed = 0f;
        }

        // Mode - attacking
        else if (attacking)
        {
            state = MovementState.attacking;
            desiredMoveSpeed = 0f;
        }
        // Mode - Sprinting
        else if (grounded && Input.GetKey(sprintKey) && inputPlayer.GetPlayerStamina.GetCurrentStamina >= sprintConsumptionRate &&
            rb.velocity.magnitude >= 0.2f)
        {
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed * groundedSpeedMultiplier;
        }

        // Mode - Walking
        else if (grounded)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed * groundedSpeedMultiplier;
        }

        // Mode - Air
        else
        {
            state = MovementState.air;

            if (moveSpeed < airMinSpeed)
                desiredMoveSpeed = airMinSpeed;

            //// Stop the player after landing on the ground if he passed the target position in the air
            //if (Vector3.Distance(transform.position, mousePosition) <= 1.5f)
            //{
            //    mousePosition = transform.position;
            //}
        }
        bool desiredMoveSpeedHasChanged = desiredMoveSpeed != lastDesiredMoveSpeed;

        if (desiredMoveSpeedHasChanged)
        {
            if (keepMomentum)
            {
                StopAllCoroutines();
                StartCoroutine(SmoothlyLerpMoveSpeed());
            }
            else
            {
                moveSpeed = desiredMoveSpeed;
            }
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;

        // deactivate keepMomentum
        if (Mathf.Abs(desiredMoveSpeed - moveSpeed) < 0.1f) keepMomentum = false;
    }

    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        // smoothly lerp movementSpeed to desired value
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);


            time += Time.deltaTime * speedIncreaseMultiplier;

            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
    }

    private void MovePlayer()
    {
        if (restricted) return;

        // on ground
        if (grounded)
            rb.AddForce(moveDirection * moveSpeed * 10f, ForceMode.Force);
        //// in air
        else if (!grounded)
        {
            if (rb.velocity.magnitude < 0.1f)
            {
                rb.AddForce(moveDirection * moveSpeed * 10f * airMultiplier, ForceMode.Force);
            }
        }

    }

    private void SpeedControl()
    {

        // limiting speed on ground or in air
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
        jumping = false;

    }
    public static float Round(float value, int digits)
    {
        float mult = Mathf.Pow(10.0f, (float)digits);
        return Mathf.Round(value * mult) / mult;
    }

    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (FootstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(capsuleCollider.center), FootstepAudioVolume);
            }
        }
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.3f)
        {
            AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(capsuleCollider.center), FootstepAudioVolume);
        }
    }
    public void IncreaseSpeed(float multiplier)
    {
        groundedSpeedMultiplier = multiplier;
    }

    public bool GetGroundedState
    {
        get { return grounded; }
    }
    public float GetSprintConsumptionRate
    {
        get { return sprintConsumptionRate; }
    }

    public float GetSpeedMultiplier
    {
        get { return groundedSpeedMultiplier;}
    }
}
