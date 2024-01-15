using UnityEngine;
using static PlayerMovement;


/// <summary>
/// Handles the basic movement animation, still needs work
/// </summary>

public class CharacterAnimation : MonoBehaviour
{

    [SerializeField] private Animator animator;

    [SerializeField] private PlayerMovement playerMovement;

    [SerializeField] private float fallDistanceThreshold;
    [SerializeField] private LayerMask whatIsGround;
    private Vector3 highestPoint;
    private bool isFalling = false;

    private bool havePistol = false;
    private bool haveRiffle = false;

    private void OnEnable()
    {
        EquipementManager.OnPistolStateChange += PistolState;
        EquipementManager.OnRiffleStateChange += RiffleState;
    }
    private void OnDisable()
    {
        EquipementManager.OnPistolStateChange -= PistolState;
        EquipementManager.OnRiffleStateChange -= RiffleState;
    }
    public void PistolState(bool state)
    {
        havePistol = state;
    }
    public void RiffleState(bool state)
    {
        haveRiffle = state;
    }
    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }
    // Start is called before the first frame update
    void Start()
    {
        animator.SetFloat("MotionSpeed", 1);
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("PistolMode", havePistol);
        animator.SetBool("RiffleMode", haveRiffle);
        if (playerMovement.jumping && !playerMovement.attacking)
        {
            animator.SetBool("Jump", true);
            animator.SetBool("Grounded", playerMovement.GetGroundedState);
        }
        else animator.SetBool("Jump", false);
        if (playerMovement.GetGroundedState)
        {
            animator.SetBool("Grounded", true);
        }
        FreeFallAnimation();



        
    }
   
    private void FreeFallAnimation()
    {
        if (!playerMovement.GetGroundedState)
        {
            if (transform.position.y > highestPoint.y)
            {
                highestPoint = transform.position;
            }

            //Once the player reachs peak height he will start falling
            else
            {
                if (!isFalling)
                {
                    isFalling = true;
                    //Calculates the distance to the ground
                    Ray ray = new Ray(highestPoint, Vector3.down);
                    if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f, whatIsGround))
                    {
                        float distanceToGround = highestPoint.y - hitInfo.point.y;
                        if (distanceToGround >= fallDistanceThreshold)
                        {
                            animator.SetBool("FreeFall", true);
                        }
                        Debug.Log("Distance to Ground: " + distanceToGround);
                    }
                }
            }
        }
        else
        {
            isFalling = false;
            // Reset animator parameter when grounded
            animator.SetBool("FreeFall", false);
            highestPoint = transform.position; // Reset highest point when grounded
        }
    }
}
